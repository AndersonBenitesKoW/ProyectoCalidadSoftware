use proyectocalidad
CREATE OR ALTER PROCEDURE sp_BuscarUsuario
(
    @IdUsuario INT  -- Parámetro de entrada que espera tu C#
)
AS
BEGIN
    -- Mejora el rendimiento al no contar filas afectadas
    SET NOCOUNT ON;

    -- Esta consulta es la clave.
    -- Seleccionamos los datos del usuario
    -- y usamos LEFT JOIN para traer los datos del Rol (si existen).
    SELECT 
        u.IdUsuario,
        u.NombreUsuario,
        u.ClaveHash,
        u.Email,
        u.Estado,
        r.IdRol,         -- Esta columna será NULL si el usuario no tiene rol
        r.NombreRol      -- Esta columna también será NULL si no tiene rol
    FROM 
        Usuario u
    LEFT JOIN 
        UsuarioRol ur ON u.IdUsuario = ur.IdUsuario
    LEFT JOIN 
        Rol r ON ur.IdRol = r.IdRol
    WHERE 
        u.IdUsuario = @IdUsuario; -- Filtramos por el ID de entrada
            
END
GO
CREATE OR ALTER PROCEDURE sp_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.IdUsuario,
        u.NombreUsuario,
        u.Email,
        u.Estado,
        
        -- Usamos ISNULL para evitar errores de NULL en C#
        ISNULL(r.IdRol, 0) AS IdRol, 
        ISNULL(r.NombreRol, 'Sin Rol') AS NombreRol 
    
    FROM 
        Usuario u
    LEFT JOIN 
        UsuarioRol ur ON u.IdUsuario = ur.IdUsuario
    LEFT JOIN 
        Rol r ON ur.IdRol = r.IdRol
    ORDER BY 
        u.NombreUsuario;
END
GO
CREATE OR ALTER PROCEDURE sp_EliminarUsuario (
    @IdUsuario INT
)
AS
BEGIN
    -- 1. Deshabilitar los triggers de la tabla Usuario
    --    para evitar el error que tenías.
    DISABLE TRIGGER ALL ON Usuario;

    BEGIN TRY
        -- 2. Ejecutar la lógica de negocio (el "borrado")
        
        -- 2a. Poner al usuario como Inactivo
        UPDATE Usuario
        SET Estado = 0
        WHERE IdUsuario = @IdUsuario;

        -- 2b. Limpiar sus roles (recomendado)
        DELETE FROM UsuarioRol
        WHERE IdUsuario = @IdUsuario;

        -- 3. Si todo salió bien, reactivar los triggers
        ENABLE TRIGGER ALL ON Usuario;
        
    END TRY
    BEGIN CATCH
        -- 4. Si algo falló, ¡ASEGÚRATE de reactivar los triggers
        --    antes de relanzar el error!
        ENABLE TRIGGER ALL ON Usuario;
        
        -- 5. Relanzar el error (método antiguo para SQL 2008)
        DECLARE @ErrorMessage NVARCHAR(4000);
        DECLARE @ErrorSeverity INT;
        DECLARE @ErrorState INT;

        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarPaciente (
    -- Datos de Paciente
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(50),
    @DNI NVARCHAR(15), -- DNI ahora es obligatorio
    @FechaNacimiento DATE = NULL,

    -- Datos de Contacto Principal
    @EmailPrincipal NVARCHAR(100),
    @TelefonoPrincipal NVARCHAR(20),

    @TipoTelefono NVARCHAR(20) = 'Celular',
    @IdUsuario INT = NULL -- Nuevo parámetro opcional para usar usuario existente
)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewIdPaciente INT;
    DECLARE @NewIdUsuario INT;
    DECLARE @IdRolPaciente INT;
    DECLARE @ClaveHash NVARCHAR(500); -- Coincide con tu C# (hex string)

    -- Variables para el manejo de errores
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    -- --- INICIO DE VALIDACIONES ---
    IF @DNI IS NULL OR LEN(@DNI) = 0
    BEGIN
        RAISERROR('El DNI es obligatorio para crear la cuenta de usuario.', 16, 1);
        RETURN;
    END
    
    IF @EmailPrincipal IS NULL OR LEN(@EmailPrincipal) = 0
    BEGIN
        RAISERROR('El email principal es obligatorio.', 16, 1);
        RETURN;
    END

    -- Buscar el IdRol de "PACIENTE"
    SELECT @IdRolPaciente = IdRol FROM Rol WHERE NombreRol = 'PACIENTE';
    
    IF @IdRolPaciente IS NULL
    BEGIN
        RAISERROR('El rol "PACIENTE" no existe. No se puede crear el usuario.', 16, 1);
        RETURN;
    END
    -- --- FIN DE VALIDACIONES ---

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Determinar IdUsuario
        IF @IdUsuario IS NOT NULL
        BEGIN
            SET @NewIdUsuario = @IdUsuario;
        END
        ELSE
        BEGIN
            -- 1. Encriptar la clave (usamos el DNI como clave)
            --    Convertimos el hash binario (HASHBYTES) a un string hexadecimal
            --    para que coincida con tu C# (NVARCHAR(500))
            SET @ClaveHash = (SELECT LOWER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @DNI), 2)));

            -- 2. Insertar el Usuario
            --    Usamos DNI como NombreUsuario y EmailPrincipal como email
            INSERT INTO Usuario (NombreUsuario, ClaveHash, email, Estado)
            VALUES (@DNI, @ClaveHash, @EmailPrincipal, 1);

            -- 3. Obtener el ID del Usuario recién creado
            SET @NewIdUsuario = SCOPE_IDENTITY();

            -- 4. Asignar el Rol "PACIENTE" al nuevo Usuario
            INSERT INTO UsuarioRol (IdUsuario, IdRol)
            VALUES (@NewIdUsuario, @IdRolPaciente);
        END

        -- 5. Insertar el Paciente, vinculándolo al Usuario
        INSERT INTO Paciente (IdUsuario, Nombres, Apellidos, DNI, FechaNacimiento, Estado)
        VALUES (@NewIdUsuario, @Nombres, @Apellidos, @DNI, @FechaNacimiento, 1);

        -- 6. Obtener el ID del Paciente recién creado
        SET @NewIdPaciente = SCOPE_IDENTITY();

        -- 7. Insertar el email principal
        INSERT INTO PacienteEmail (IdPaciente, Email, EsPrincipal)
        VALUES (@NewIdPaciente, @EmailPrincipal, 1);

        -- 8. Insertar el teléfono principal
        INSERT INTO PacienteTelefono (IdPaciente, Telefono, Tipo, EsPrincipal)
        VALUES (@NewIdPaciente, @TelefonoPrincipal, @TipoTelefono, 1);

        -- 9. Confirmar la transacción
        COMMIT TRANSACTION;
        
        -- 10. Devolver el nuevo ID de Paciente (para C# ExecuteScalar)
        SELECT @NewIdPaciente AS NewIdPaciente;

    END TRY
    BEGIN CATCH
        -- Si algo falló, revertir todo
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        -- Relanzar el error
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_InhabilitarPaciente (
    @IdPaciente INT
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON para que ExecuteNonQuery() > 0 funcione
    
    DECLARE @IdUsuarioParaInhabilitar INT;

    -- Variables de error (para SQL 2008)
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Obtener el IdUsuario ANTES de inhabilitar al paciente
        SELECT @IdUsuarioParaInhabilitar = IdUsuario
        FROM Paciente
        WHERE IdPaciente = @IdPaciente;

        -- 2. Inhabilitar al Paciente
        UPDATE Paciente
        SET Estado = 0
        WHERE IdPaciente = @IdPaciente;

        -- 3. Inhabilitar la cuenta de Usuario asociada (si existe)
        IF @IdUsuarioParaInhabilitar IS NOT NULL
        BEGIN
            UPDATE Usuario
            SET Estado = 0
            WHERE IdUsuario = @IdUsuarioParaInhabilitar;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Relanzar error
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarPacientesActivos
AS
BEGIN
    SELECT 
        p.IdPaciente, 
        p.Nombres, 
        p.Apellidos, 
        p.DNI,
        p.FechaNacimiento, -- (Asegúrate de incluir todos los campos que C# lee)
        p.IdUsuario,       -- (Asegúrate de incluir todos los campos que C# lee)
        p.Estado           -- <-- ¡LA COLUMNA QUE FALTABA!
    FROM 
        Paciente p
    WHERE 
        p.Estado = 1; -- (Asumo que este SP solo lista los activos)
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarPaciente (
    @IdPaciente INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdPaciente,
        p.IdUsuario,
        p.Nombres,
        p.Apellidos,
        p.DNI,
        p.FechaNacimiento,
        p.Estado,

        -- Usamos ISNULL para evitar errores en C#
        ISNULL(pe.Email, '') AS EmailPrincipal,
        ISNULL(pt.Telefono, '') AS TelefonoPrincipal
    FROM
        Paciente p
    -- Unir con email principal
    LEFT JOIN
        PacienteEmail pe ON p.IdPaciente = pe.IdPaciente AND pe.EsPrincipal = 1
    -- Unir con teléfono principal
    LEFT JOIN
        PacienteTelefono pt ON p.IdPaciente = pt.IdPaciente AND pt.EsPrincipal = 1
    WHERE
        p.IdPaciente = @IdPaciente;
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarPacientePorDNI (
    @DNI NVARCHAR(15),
    @IdUsuario INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdPaciente,
        p.IdUsuario,
        p.Nombres,
        p.Apellidos,
        p.DNI,
        p.FechaNacimiento,
        p.Estado,

        -- Usamos ISNULL para evitar errores en C#
        ISNULL(pe.Email, '') AS EmailPrincipal,
        ISNULL(pt.Telefono, '') AS TelefonoPrincipal
    FROM
        Paciente p
    -- Unir con email principal
    LEFT JOIN
        PacienteEmail pe ON p.IdPaciente = pe.IdPaciente AND pe.EsPrincipal = 1
    -- Unir con teléfono principal
    LEFT JOIN
        PacienteTelefono pt ON p.IdPaciente = pt.IdPaciente AND pt.EsPrincipal = 1
    WHERE
        p.DNI = @DNI
        AND (@IdUsuario IS NULL OR p.IdUsuario = @IdUsuario);
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarProfesionalPorCMP (
    @CMP NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdProfesional,
        p.IdUsuario,
        p.Nombres,
        p.Apellidos,
        p.CMP,
        p.Especialidad,
        p.Estado,

        -- Usamos ISNULL para evitar errores en C#
        ISNULL(pe.Email, '') AS EmailPrincipal,
        ISNULL(pt.Telefono, '') AS TelefonoPrincipal
    FROM
        ProfesionalSalud p
    -- Unir con email principal
    LEFT JOIN
        ProfesionalEmail pe ON p.IdProfesional = pe.IdProfesional AND pe.EsPrincipal = 1
    -- Unir con teléfono principal
    LEFT JOIN
        ProfesionalTelefono pt ON p.IdProfesional = pt.IdProfesional AND pt.EsPrincipal = 1
    WHERE
        p.CMP = @CMP;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarPaciente (
    @IdPaciente INT,
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(50),
    @DNI NVARCHAR(15) = NULL,
    @FechaNacimiento DATE = NULL,
    @Estado BIT,
    
    -- Contacto (pueden ser nulos si se borran)
    @EmailPrincipal NVARCHAR(100) = NULL,
    @TelefonoPrincipal NVARCHAR(20) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON

    -- Variables de error (para SQL 2008)
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Actualizar tabla Paciente
        UPDATE Paciente
        SET
            Nombres = @Nombres,
            Apellidos = @Apellidos,
            DNI = @DNI,
            FechaNacimiento = @FechaNacimiento,
            Estado = @Estado
        WHERE
            IdPaciente = @IdPaciente;

        -- 2. Actualizar Email Principal (Lógica UPSERT)
        -- Primero intenta actualizar el email principal existente
        UPDATE PacienteEmail
        SET Email = @EmailPrincipal
        WHERE IdPaciente = @IdPaciente AND EsPrincipal = 1;

        -- Si no actualizó ninguna fila (porque no existía), la inserta
        IF @@ROWCOUNT = 0 AND @EmailPrincipal IS NOT NULL AND LEN(@EmailPrincipal) > 0
        BEGIN
            INSERT INTO PacienteEmail (IdPaciente, Email, EsPrincipal)
            VALUES (@IdPaciente, @EmailPrincipal, 1);
        END

        -- 3. Actualizar Teléfono Principal (Lógica UPSERT)
        UPDATE PacienteTelefono
        SET Telefono = @TelefonoPrincipal
        WHERE IdPaciente = @IdPaciente AND EsPrincipal = 1;

        IF @@ROWCOUNT = 0 AND @TelefonoPrincipal IS NOT NULL AND LEN(@TelefonoPrincipal) > 0
        BEGIN
            -- Asumimos 'Celular' por defecto si es nuevo
            INSERT INTO PacienteTelefono (IdPaciente, Telefono, Tipo, EsPrincipal)
            VALUES (@IdPaciente, @TelefonoPrincipal, 'Celular', 1);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Relanzar error
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarProfesionalSalud (
    @Estado BIT = 1 -- Parámetro para filtrar por estado
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.IdProfesional,
        p.IdUsuario,
        p.CMP,
        p.Especialidad,
        p.Nombres,
        p.Apellidos,
        p.Estado,
        ISNULL(pe.Email, '') AS EmailPrincipal,
        ISNULL(pt.Telefono, '') AS TelefonoPrincipal
    FROM 
        ProfesionalSalud p
    LEFT JOIN 
        ProfesionalEmail pe ON p.IdProfesional = pe.IdProfesional AND pe.EsPrincipal = 1
    LEFT JOIN 
        ProfesionalTelefono pt ON p.IdProfesional = pt.IdProfesional AND pt.EsPrincipal = 1
    WHERE
        p.Estado = @Estado -- Usamos el parámetro que tu C# espera
    ORDER BY
        p.Apellidos, p.Nombres;
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarProfesionalSalud (
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(50),
    @CMP NVARCHAR(20),
    @Especialidad NVARCHAR(80) = NULL,
    
    -- Parámetros que faltaban en tu C#
    @EmailPrincipal NVARCHAR(100),
    @TelefonoPrincipal NVARCHAR(20),
    @TipoTelefono NVARCHAR(20) = 'Celular' 
)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewIdProfesional INT;
    DECLARE @NewIdUsuario INT;
    DECLARE @IdRolProfesional INT;
    DECLARE @ClaveHash NVARCHAR(500);

    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    -- --- Validaciones ---
    IF @CMP IS NULL OR LEN(@CMP) = 0 BEGIN
        RAISERROR('El CMP es obligatorio para crear la cuenta de usuario.', 16, 1); RETURN;
    END
    IF @EmailPrincipal IS NULL OR LEN(@EmailPrincipal) = 0 BEGIN
        RAISERROR('El email principal es obligatorio.', 16, 1); RETURN;
    END

    SELECT @IdRolProfesional = IdRol FROM Rol WHERE NombreRol = 'PERSONAL_SALUD';
    IF @IdRolProfesional IS NULL BEGIN
        RAISERROR('El rol "PERSONAL_SALUD" no existe.', 16, 1); RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Encriptar la clave (usamos el CMP)
        SET @ClaveHash = (SELECT LOWER(CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', @CMP), 2)));

        -- 2. Insertar el Usuario
        INSERT INTO Usuario (NombreUsuario, ClaveHash, email, Estado)
        VALUES (@CMP, @ClaveHash, @EmailPrincipal, 1);
        SET @NewIdUsuario = SCOPE_IDENTITY();

        -- 3. Asignar el Rol
        INSERT INTO UsuarioRol (IdUsuario, IdRol)
        VALUES (@NewIdUsuario, @IdRolProfesional);

        -- 4. Insertar el Profesional
        INSERT INTO ProfesionalSalud (IdUsuario, Nombres, Apellidos, CMP, Especialidad, Estado)
        VALUES (@NewIdUsuario, @Nombres, @Apellidos, @CMP, @Especialidad, 1);
        SET @NewIdProfesional = SCOPE_IDENTITY();

        -- 5. Insertar el email principal
        INSERT INTO ProfesionalEmail (IdProfesional, Email, EsPrincipal)
        VALUES (@NewIdProfesional, @EmailPrincipal, 1);

        -- 6. Insertar el teléfono principal
        INSERT INTO ProfesionalTelefono (IdProfesional, Telefono, Tipo, EsPrincipal)
        VALUES (@NewIdProfesional, @TelefonoPrincipal, @TipoTelefono, 1);

        COMMIT TRANSACTION;
        
        -- Devolver el nuevo ID (para C# ExecuteScalar)
        SELECT @NewIdProfesional AS NewIdProfesional;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarProfesionalSalud (
    @IdProfesional INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.IdProfesional,
        p.IdUsuario,
        p.CMP,
        p.Especialidad,
        p.Nombres,
        p.Apellidos,
        p.Estado,
        ISNULL(pe.Email, '') AS EmailPrincipal,
        ISNULL(pt.Telefono, '') AS TelefonoPrincipal
    FROM 
        ProfesionalSalud p
    LEFT JOIN 
        ProfesionalEmail pe ON p.IdProfesional = pe.IdProfesional AND pe.EsPrincipal = 1
    LEFT JOIN 
        ProfesionalTelefono pt ON p.IdProfesional = pt.IdProfesional AND pt.EsPrincipal = 1
    WHERE 
        p.IdProfesional = @IdProfesional;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarProfesionalSalud (
    @IdProfesional INT,
    @Nombres NVARCHAR(100),
    @Apellidos NVARCHAR(50),
    @CMP NVARCHAR(20) = NULL, -- Usamos @CMP
    @Especialidad NVARCHAR(80) = NULL,
    @Estado BIT,
    
    -- Parámetros que faltaban
    @EmailPrincipal NVARCHAR(100) = NULL,
    @TelefonoPrincipal NVARCHAR(20) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE ProfesionalSalud SET
            Nombres = @Nombres,
            Apellidos = @Apellidos,
            CMP = @CMP,
            Especialidad = @Especialidad,
            Estado = @Estado
        WHERE IdProfesional = @IdProfesional;

        UPDATE ProfesionalEmail SET Email = @EmailPrincipal
        WHERE IdProfesional = @IdProfesional AND EsPrincipal = 1;
        IF @@ROWCOUNT = 0 AND @EmailPrincipal IS NOT NULL AND LEN(@EmailPrincipal) > 0 BEGIN
            INSERT INTO ProfesionalEmail (IdProfesional, Email, EsPrincipal)
            VALUES (@IdProfesional, @EmailPrincipal, 1);
        END

        UPDATE ProfesionalTelefono SET Telefono = @TelefonoPrincipal
        WHERE IdProfesional = @IdProfesional AND EsPrincipal = 1;
        IF @@ROWCOUNT = 0 AND @TelefonoPrincipal IS NOT NULL AND LEN(@TelefonoPrincipal) > 0 BEGIN
            INSERT INTO ProfesionalTelefono (IdProfesional, Telefono, Tipo, EsPrincipal)
            VALUES (@IdProfesional, @TelefonoPrincipal, 'Celular', 1);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_InhabilitarProfesionalSalud (
    @IdProfesional INT
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @IdUsuarioParaInhabilitar INT;
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        SELECT @IdUsuarioParaInhabilitar = IdUsuario
        FROM ProfesionalSalud
        WHERE IdProfesional = @IdProfesional;

        UPDATE ProfesionalSalud SET Estado = 0
        WHERE IdProfesional = @IdProfesional;

        IF @IdUsuarioParaInhabilitar IS NOT NULL
        BEGIN
            UPDATE Usuario SET Estado = 0
            WHERE IdUsuario = @IdUsuarioParaInhabilitar;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_CerrarEmbarazo (
    @IdEmbarazo INT
)
AS
BEGIN
    -- !! IMPORTANTE !!
    -- NO usar 'SET NOCOUNT ON' aquí, para que
    -- ExecuteNonQuery() > 0 funcione en C#.
    
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    BEGIN TRY
        -- Cerramos el embarazo (Estado=0) y ponemos la fecha de cierre
        UPDATE Embarazo
        SET 
            Estado = 0,
            FechaCierre = GETDATE()
        WHERE 
            IdEmbarazo = @IdEmbarazo
            AND Estado = 1; -- Solo cerrar si está activo
            
    END TRY
    BEGIN CATCH
        -- Relanzar el error (método antiguo para SQL 2008)
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarCitas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.IdCita,
        c.IdPaciente,
        ISNULL(p.Nombres + ' ' + p.Apellidos, 'Paciente no encontrado') AS NombrePaciente,
        c.IdProfesional,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        c.IdEmbarazo,
        c.FechaCita,
        c.Motivo,
        c.IdEstadoCita,
        
        -- ==== CORRECCIÓN AQUÍ ====
        -- La columna se llama 'Descripcion', no 'Nombre'
        ISNULL(ec.Descripcion, 'Desconocido') AS NombreEstado, 
        
        c.Observacion
    FROM 
        Cita c
    LEFT JOIN 
        Paciente p ON c.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON c.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        EstadoCita ec ON c.IdEstadoCita = ec.IdEstadoCita
    ORDER BY
        c.FechaCita DESC;
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarCita (
    @IdPaciente INT,
    @IdRecepcionista INT = NULL,
    @IdProfesional INT = NULL,
    @IdEmbarazo INT = NULL,
    @FechaCita DATETIME2,
    @Motivo NVARCHAR(150) = NULL,
    @IdEstadoCita SMALLINT, -- El SP asume que el C# le da el default (1)
    @Observacion NVARCHAR(300) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

    BEGIN TRY
        INSERT INTO Cita (
            IdPaciente, IdRecepcionista, IdProfesional, IdEmbarazo, 
            FechaCita, Motivo, IdEstadoCita, Observacion
        )
        VALUES (
            @IdPaciente, @IdRecepcionista, @IdProfesional, @IdEmbarazo, 
            @FechaCita, @Motivo, @IdEstadoCita, @Observacion
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_EliminarCita (
    @IdCita INT,
    @MotivoAnulacion NVARCHAR(200) 
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    DECLARE @IdEstadoAnulada SMALLINT;
    
    -- ==== CORRECCIÓN AQUÍ ====
    -- Buscamos la descripción exacta de tu tabla: "Cita anulada"
    SELECT @IdEstadoAnulada = IdEstadoCita 
    FROM EstadoCita 
    WHERE Descripcion = 'Cita anulada'; -- <-- Esta es la corrección
    
    IF @IdEstadoAnulada IS NULL BEGIN
        RAISERROR('El estado "Cita anulada" no existe en EstadoCita (columna Descripcion).', 16, 1);
        RETURN;
    END

    IF @MotivoAnulacion IS NULL OR LTRIM(RTRIM(@MotivoAnulacion)) = ''
    BEGIN
        RAISERROR('Se requiere un motivo para la anulación.', 16, 1);
        RETURN;
    END

    BEGIN TRY
        UPDATE Cita
        SET 
            IdEstadoCita = @IdEstadoAnulada,
            FechaAnulacion = GETDATE(),
            MotivoAnulacion = @MotivoAnulacion
        WHERE
            IdCita = @IdCita
            AND IdEstadoCita != @IdEstadoAnulada; 
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarCita (
    @IdCita INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.IdCita,
        c.IdPaciente,
        ISNULL(p.Nombres + ' ' + p.Apellidos, 'Paciente no encontrado') AS NombrePaciente,
        c.IdRecepcionista,
        c.IdProfesional,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        c.IdEmbarazo,
        c.FechaCita,
        c.Motivo,
        c.IdEstadoCita,
        
        -- ==== CORRECCIÓN AQUÍ ====
        -- La columna se llama 'Descripcion', no 'Nombre'
        ISNULL(ec.Descripcion, 'Desconocido') AS NombreEstado, 
        
        c.Observacion,
        c.FechaAnulacion,
        c.MotivoAnulacion
    FROM 
        Cita c
    LEFT JOIN 
        Paciente p ON c.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON c.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        EstadoCita ec ON c.IdEstadoCita = ec.IdEstadoCita
    WHERE
        c.IdCita = @IdCita;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarCita (
    @IdCita INT,
    @IdPaciente INT,
    @IdRecepcionista INT = NULL,
    @IdProfesional INT = NULL,
    @IdEmbarazo INT = NULL,
    @FechaCita DATETIME2,
    @Motivo NVARCHAR(150) = NULL,
    @IdEstadoCita SMALLINT,
    @Observacion NVARCHAR(300) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

    BEGIN TRY
        UPDATE Cita SET
            IdPaciente = @IdPaciente,
            IdRecepcionista = @IdRecepcionista,
            IdProfesional = @IdProfesional,
            IdEmbarazo = @IdEmbarazo,
            FechaCita = @FechaCita,
            Motivo = @Motivo,
            IdEstadoCita = @IdEstadoCita,
            Observacion = @Observacion
            -- No actualizamos los campos de anulación aquí
        WHERE
            IdCita = @IdCita;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarEncuentros
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.IdEncuentro,
        e.IdEmbarazo,
        e.IdProfesional,
        e.IdTipoEncuentro,
        e.FechaHoraInicio,
        e.FechaHoraFin,
        e.Estado,
        e.Notas,
        -- Nombres de las tablas unidas
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        te.Descripcion AS TipoEncuentroDesc
    FROM 
        Encuentro e
    INNER JOIN 
        Embarazo em ON e.IdEmbarazo = em.IdEmbarazo
    INNER JOIN 
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON e.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        TipoEncuentro te ON e.IdTipoEncuentro = te.IdTipoEncuentro
    ORDER BY
        e.FechaHoraInicio DESC;
END
GO


CREATE OR ALTER PROCEDURE sp_InsertarEncuentro
(
    @IdEmbarazo INT,
    @IdProfesional INT = NULL,
    @IdTipoEncuentro SMALLINT,
    @FechaHoraInicio DATETIME2,
    @FechaHoraFin DATETIME2 = NULL,
    @Estado NVARCHAR(20),
    @Notas NVARCHAR(500) = NULL
)
AS
BEGIN
    INSERT INTO Encuentro (
        IdEmbarazo,
        IdProfesional,
        IdTipoEncuentro,
        FechaHoraInicio,
        FechaHoraFin,
        Estado,
        Notas
    )
    VALUES (
        @IdEmbarazo,
        @IdProfesional,
        @IdTipoEncuentro,
        @FechaHoraInicio,
        @FechaHoraFin,
        @Estado,
        @Notas
    );

    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarEncuentro (
    @IdEncuentro INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        e.IdEncuentro,
        e.IdEmbarazo,
        e.IdProfesional,
        e.IdTipoEncuentro,
        e.FechaHoraInicio,
        e.FechaHoraFin,
        e.Estado,
        e.Notas,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        te.Descripcion AS TipoEncuentroDesc
    FROM 
        Encuentro e
    INNER JOIN 
        Embarazo em ON e.IdEmbarazo = em.IdEmbarazo
    INNER JOIN 
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON e.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        TipoEncuentro te ON e.IdTipoEncuentro = te.IdTipoEncuentro
    WHERE
        e.IdEncuentro = @IdEncuentro;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarEncuentro (
    @IdEncuentro INT,
    @IdEmbarazo INT,
    @IdProfesional INT = NULL,
    @IdTipoEncuentro SMALLINT,
    @FechaHoraFin DATETIME2 = NULL,
    @Estado NVARCHAR(20),
    @Notas NVARCHAR(500) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

    BEGIN TRY
        UPDATE Encuentro SET
            IdEmbarazo = @IdEmbarazo,
            IdProfesional = @IdProfesional,
            IdTipoEncuentro = @IdTipoEncuentro,
            FechaHoraFin = @FechaHoraFin,
            Estado = @Estado,
            Notas = @Notas
        WHERE
            IdEncuentro = @IdEncuentro;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_EliminarEncuentro (
    @IdEncuentro INT
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

    BEGIN TRY
        UPDATE Encuentro
        SET 
            Estado = 'Anulado',
            FechaHoraFin = GETDATE()
        WHERE
            IdEncuentro = @IdEncuentro
            AND Estado != 'Anulado'; -- Solo anular si no está ya anulado
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarTipoEncuentro
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTipoEncuentro, Codigo, Descripcion
    FROM TipoEncuentro
    ORDER BY Descripcion;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarControlPrenatal
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.IdControl,
        c.IdEmbarazo,
        c.IdEncuentro,
        c.IdProfesional,
        c.Fecha,
        c.NumeroControl,
        c.EdadGestSemanas,
        c.EdadGestDias,
        c.MetodoEdadGest,
        c.PesoKg,
        c.PesoPreGestacionalKg,
        c.TallaM,
        c.IMCPreGestacional,
        c.PA_Sistolica,
        c.PA_Diastolica,
        c.Pulso,
        c.FrecuenciaRespiratoria,
        c.Temperatura,
        c.AlturaUterina_cm,
        c.DinamicaUterina,
        c.Presentacion,
        c.TipoEmbarazo,
        c.FCF_bpm,
        c.LiquidoAmniotico,
        c.IndiceLiquidoAmniotico,
        c.PerfilBiofisico,
        c.Proteinuria,
        c.Edemas,
        c.Reflejos,
        c.Hemoglobina,
        c.ResultadoVIH,
        c.ResultadoSifilis,
        c.GrupoSanguineoRh,
        c.EcografiaRealizada,
        c.FechaEcografia,
        c.LugarEcografia,
        c.PlanPartoEntregado,
        c.MicronutrientesEntregados,
        c.ViajoUltSemanas,
        c.ReferenciaObstetrica,
        c.Consejerias,
        c.Observaciones,
        c.ProximaCitaFecha,
        c.EstablecimientoAtencion,
        c.Estado,
        -- JOINs para nombres
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional
    FROM
        ControlPrenatal c
    INNER JOIN
        Embarazo em ON c.IdEmbarazo = em.IdEmbarazo
    INNER JOIN
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN
        ProfesionalSalud ps ON c.IdProfesional = ps.IdProfesional
    ORDER BY
        c.Fecha DESC;
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarControlPrenatal (
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @NumeroControl INT = NULL,
    @EdadGestSemanas INT = NULL,
    @EdadGestDias INT = NULL,
    @MetodoEdadGest VARCHAR(50) = NULL,
    @PesoKg DECIMAL(5,2) = NULL,
    @PesoPreGestacionalKg DECIMAL(5,2) = NULL,
    @TallaM DECIMAL(3,2) = NULL,
    @IMCPreGestacional DECIMAL(5,2) = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Pulso SMALLINT = NULL,
    @FrecuenciaRespiratoria SMALLINT = NULL,
    @Temperatura DECIMAL(4,1) = NULL,
    @AlturaUterina_cm DECIMAL(5,1) = NULL,
    @DinamicaUterina VARCHAR(10) = NULL,
    @Presentacion NVARCHAR(50) = NULL,
    @TipoEmbarazo VARCHAR(20) = NULL,
    @FCF_bpm TINYINT = NULL,
    @LiquidoAmniotico VARCHAR(20) = NULL,
    @IndiceLiquidoAmniotico DECIMAL(4,1) = NULL,
    @PerfilBiofisico VARCHAR(10) = NULL,
    @Proteinuria VARCHAR(10) = NULL,
    @Edemas VARCHAR(10) = NULL,
    @Reflejos VARCHAR(10) = NULL,
    @Hemoglobina DECIMAL(4,1) = NULL,
    @ResultadoVIH VARCHAR(20) = NULL,
    @ResultadoSifilis VARCHAR(20) = NULL,
    @GrupoSanguineoRh VARCHAR(5) = NULL,
    @EcografiaRealizada BIT = NULL,
    @FechaEcografia DATE = NULL,
    @LugarEcografia NVARCHAR(100) = NULL,
    @PlanPartoEntregado BIT = NULL,
    @MicronutrientesEntregados NVARCHAR(100) = NULL,
    @ViajoUltSemanas BIT = NULL,
    @ReferenciaObstetrica BIT = NULL,
    @Consejerias NVARCHAR(200) = NULL,
    @Observaciones NVARCHAR(300) = NULL,
    @ProximaCitaFecha DATE = NULL,
    @EstablecimientoAtencion NVARCHAR(100) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        INSERT INTO ControlPrenatal (
            IdEmbarazo, IdEncuentro, IdProfesional, Fecha, NumeroControl, EdadGestSemanas,
            EdadGestDias, MetodoEdadGest, PesoKg, PesoPreGestacionalKg, TallaM, IMCPreGestacional,
            PA_Sistolica, PA_Diastolica, Pulso, FrecuenciaRespiratoria, Temperatura, AlturaUterina_cm,
            DinamicaUterina, Presentacion, TipoEmbarazo, FCF_bpm, LiquidoAmniotico, IndiceLiquidoAmniotico,
            PerfilBiofisico, Proteinuria, Edemas, Reflejos, Hemoglobina, ResultadoVIH, ResultadoSifilis,
            GrupoSanguineoRh, EcografiaRealizada, FechaEcografia, LugarEcografia, PlanPartoEntregado,
            MicronutrientesEntregados, ViajoUltSemanas, ReferenciaObstetrica, Consejerias, Observaciones,
            ProximaCitaFecha, EstablecimientoAtencion, Estado
        )
        VALUES (
            @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @NumeroControl, @EdadGestSemanas,
            @EdadGestDias, @MetodoEdadGest, @PesoKg, @PesoPreGestacionalKg, @TallaM, @IMCPreGestacional,
            @PA_Sistolica, @PA_Diastolica, @Pulso, @FrecuenciaRespiratoria, @Temperatura, @AlturaUterina_cm,
            @DinamicaUterina, @Presentacion, @TipoEmbarazo, @FCF_bpm, @LiquidoAmniotico, @IndiceLiquidoAmniotico,
            @PerfilBiofisico, @Proteinuria, @Edemas, @Reflejos, @Hemoglobina, @ResultadoVIH, @ResultadoSifilis,
            @GrupoSanguineoRh, @EcografiaRealizada, @FechaEcografia, @LugarEcografia, @PlanPartoEntregado,
            @MicronutrientesEntregados, @ViajoUltSemanas, @ReferenciaObstetrica, @Consejerias, @Observaciones,
            @ProximaCitaFecha, @EstablecimientoAtencion, @Estado
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarControlPrenatal (
    @IdControl INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.IdControl, c.IdEmbarazo, c.IdEncuentro, c.IdProfesional, c.Fecha,
        c.NumeroControl, c.EdadGestSemanas, c.EdadGestDias, c.MetodoEdadGest,
        c.PesoKg, c.PesoPreGestacionalKg, c.TallaM, c.IMCPreGestacional,
        c.PA_Sistolica, c.PA_Diastolica, c.Pulso, c.FrecuenciaRespiratoria, c.Temperatura,
        c.AlturaUterina_cm, c.DinamicaUterina, c.Presentacion, c.TipoEmbarazo, c.FCF_bpm,
        c.LiquidoAmniotico, c.IndiceLiquidoAmniotico, c.PerfilBiofisico, c.Proteinuria,
        c.Edemas, c.Reflejos, c.Hemoglobina, c.ResultadoVIH, c.ResultadoSifilis,
        c.GrupoSanguineoRh, c.EcografiaRealizada, c.FechaEcografia, c.LugarEcografia,
        c.PlanPartoEntregado, c.MicronutrientesEntregados, c.ViajoUltSemanas, c.ReferenciaObstetrica,
        c.Consejerias, c.Observaciones, c.ProximaCitaFecha, c.EstablecimientoAtencion, c.Estado,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional
    FROM
        ControlPrenatal c
    INNER JOIN
        Embarazo em ON c.IdEmbarazo = em.IdEmbarazo
    INNER JOIN
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN
        ProfesionalSalud ps ON c.IdProfesional = ps.IdProfesional
    WHERE
        c.IdControl = @IdControl;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarControlPrenatal (
    @IdControl INT,
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @NumeroControl INT = NULL,
    @EdadGestSemanas INT = NULL,
    @EdadGestDias INT = NULL,
    @MetodoEdadGest VARCHAR(50) = NULL,
    @PesoKg DECIMAL(5,2) = NULL,
    @PesoPreGestacionalKg DECIMAL(5,2) = NULL,
    @TallaM DECIMAL(3,2) = NULL,
    @IMCPreGestacional DECIMAL(5,2) = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Pulso SMALLINT = NULL,
    @FrecuenciaRespiratoria SMALLINT = NULL,
    @Temperatura DECIMAL(4,1) = NULL,
    @AlturaUterina_cm DECIMAL(5,1) = NULL,
    @DinamicaUterina VARCHAR(10) = NULL,
    @Presentacion NVARCHAR(50) = NULL,
    @TipoEmbarazo VARCHAR(20) = NULL,
    @FCF_bpm TINYINT = NULL,
    @LiquidoAmniotico VARCHAR(20) = NULL,
    @IndiceLiquidoAmniotico DECIMAL(4,1) = NULL,
    @PerfilBiofisico VARCHAR(10) = NULL,
    @Proteinuria VARCHAR(10) = NULL,
    @Edemas VARCHAR(10) = NULL,
    @Reflejos VARCHAR(10) = NULL,
    @Hemoglobina DECIMAL(4,1) = NULL,
    @ResultadoVIH VARCHAR(20) = NULL,
    @ResultadoSifilis VARCHAR(20) = NULL,
    @GrupoSanguineoRh VARCHAR(5) = NULL,
    @EcografiaRealizada BIT = NULL,
    @FechaEcografia DATE = NULL,
    @LugarEcografia NVARCHAR(100) = NULL,
    @PlanPartoEntregado BIT = NULL,
    @MicronutrientesEntregados NVARCHAR(100) = NULL,
    @ViajoUltSemanas BIT = NULL,
    @ReferenciaObstetrica BIT = NULL,
    @Consejerias NVARCHAR(200) = NULL,
    @Observaciones NVARCHAR(300) = NULL,
    @ProximaCitaFecha DATE = NULL,
    @EstablecimientoAtencion NVARCHAR(100) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE ControlPrenatal SET
            IdEmbarazo = @IdEmbarazo,
            IdEncuentro = @IdEncuentro,
            IdProfesional = @IdProfesional,
            Fecha = @Fecha,
            NumeroControl = @NumeroControl,
            EdadGestSemanas = @EdadGestSemanas,
            EdadGestDias = @EdadGestDias,
            MetodoEdadGest = @MetodoEdadGest,
            PesoKg = @PesoKg,
            PesoPreGestacionalKg = @PesoPreGestacionalKg,
            TallaM = @TallaM,
            IMCPreGestacional = @IMCPreGestacional,
            PA_Sistolica = @PA_Sistolica,
            PA_Diastolica = @PA_Diastolica,
            Pulso = @Pulso,
            FrecuenciaRespiratoria = @FrecuenciaRespiratoria,
            Temperatura = @Temperatura,
            AlturaUterina_cm = @AlturaUterina_cm,
            DinamicaUterina = @DinamicaUterina,
            Presentacion = @Presentacion,
            TipoEmbarazo = @TipoEmbarazo,
            FCF_bpm = @FCF_bpm,
            LiquidoAmniotico = @LiquidoAmniotico,
            IndiceLiquidoAmniotico = @IndiceLiquidoAmniotico,
            PerfilBiofisico = @PerfilBiofisico,
            Proteinuria = @Proteinuria,
            Edemas = @Edemas,
            Reflejos = @Reflejos,
            Hemoglobina = @Hemoglobina,
            ResultadoVIH = @ResultadoVIH,
            ResultadoSifilis = @ResultadoSifilis,
            GrupoSanguineoRh = @GrupoSanguineoRh,
            EcografiaRealizada = @EcografiaRealizada,
            FechaEcografia = @FechaEcografia,
            LugarEcografia = @LugarEcografia,
            PlanPartoEntregado = @PlanPartoEntregado,
            MicronutrientesEntregados = @MicronutrientesEntregados,
            ViajoUltSemanas = @ViajoUltSemanas,
            ReferenciaObstetrica = @ReferenciaObstetrica,
            Consejerias = @Consejerias,
            Observaciones = @Observaciones,
            ProximaCitaFecha = @ProximaCitaFecha,
            EstablecimientoAtencion = @EstablecimientoAtencion,
            Estado = @Estado
        WHERE
            IdControl = @IdControl;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_InhabilitarControlPrenatal (
    @IdControl INT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE ControlPrenatal
        SET Estado = 0
        WHERE 
            IdControl = @IdControl
            AND Estado = 1;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE  OR ALTER PROCEDURE sp_ListarSeguimientoPuerperio (
    @Estado BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sp.IdPuerperio,
        sp.IdEmbarazo,
        sp.IdProfesional,
        sp.IdMetodoPF,
        sp.Fecha,
        sp.DiasPosparto,
        sp.PA_Sistolica,
        sp.PA_Diastolica,
        sp.Temp_C,
        sp.AlturaUterinaPP_cm,
        sp.InvolucionUterina,
        sp.Loquios,
        sp.HemorragiaResidual,          -- BIT ✔
        sp.Lactancia,
        sp.ApoyoLactancia,
        sp.SignosInfeccion,
        sp.TamizajeDepresion,
        sp.ConsejoPlanificacion,  -- Nombre real ✔
        sp.VisitaDomiciliariaFecha,
        sp.SeguroTipo,
        sp.ComplicacionesMaternas,
        sp.Derivacion,
        sp.EstablecimientoAtencion,
        sp.Observaciones,
        sp.Estado,

        -- JOINs
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(mpf.Nombre, 'No especificado') AS NombreMetodoPF
    FROM 
        SeguimientoPuerperio sp
        INNER JOIN Embarazo em ON sp.IdEmbarazo = em.IdEmbarazo
        INNER JOIN Paciente p ON em.IdPaciente = p.IdPaciente
        LEFT JOIN ProfesionalSalud ps ON sp.IdProfesional = ps.IdProfesional
        LEFT JOIN MetodoPF mpf ON sp.IdMetodoPF = mpf.IdMetodoPF
    WHERE
        sp.Estado = @Estado
    ORDER BY
        sp.Fecha DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarSeguimientoPuerperio (
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @DiasPosparto INT = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Temp_C DECIMAL(4,1) = NULL,
    @AlturaUterinaPP_cm DECIMAL(5,1) = NULL,
    @InvolucionUterina VARCHAR(50) = NULL,
    @Loquios NVARCHAR(20) = NULL,
    @HemorragiaResidual BIT = NULL,
    @Lactancia NVARCHAR(20) = NULL,
    @ApoyoLactancia BIT = NULL,
    @SignosInfeccion BIT = NULL,
    @TamizajeDepresion NVARCHAR(20) = NULL,
    @IdMetodoPF SMALLINT = NULL,
    @ConsejoPlanificacion BIT = NULL,
    @VisitaDomiciliariaFecha DATE = NULL,
    @SeguroTipo NVARCHAR(50) = NULL,
    @ComplicacionesMaternas NVARCHAR(300) = NULL,
    @Derivacion BIT = NULL,
    @EstablecimientoAtencion NVARCHAR(100) = NULL,
    @Observaciones NVARCHAR(500) = NULL,
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        INSERT INTO SeguimientoPuerperio (
            IdEmbarazo, IdEncuentro, IdProfesional, Fecha, DiasPosparto,
            PA_Sistolica, PA_Diastolica, Temp_C, AlturaUterinaPP_cm,
            InvolucionUterina, Loquios, HemorragiaResidual, Lactancia,
            ApoyoLactancia, SignosInfeccion, TamizajeDepresion,
            IdMetodoPF, ConsejoPlanificacion, VisitaDomiciliariaFecha,
            SeguroTipo, ComplicacionesMaternas, Derivacion,
            EstablecimientoAtencion, Observaciones, Estado
        )
        VALUES (
            @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @DiasPosparto,
            @PA_Sistolica, @PA_Diastolica, @Temp_C, @AlturaUterinaPP_cm,
            @InvolucionUterina, @Loquios, @HemorragiaResidual, @Lactancia,
            @ApoyoLactancia, @SignosInfeccion, @TamizajeDepresion,
            @IdMetodoPF, @ConsejoPlanificacion, @VisitaDomiciliariaFecha,
            @SeguroTipo, @ComplicacionesMaternas, @Derivacion,
            @EstablecimientoAtencion, @Observaciones, @Estado
        );
    END TRY
    BEGIN CATCH
        DECLARE @err NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@err, 16, 1);
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarSeguimientoPuerperio (
    @IdPuerperio INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        sp.IdPuerperio, sp.IdEmbarazo, sp.IdEncuentro, sp.IdProfesional, sp.Fecha,
        sp.DiasPosparto, sp.PA_Sistolica, sp.PA_Diastolica, sp.Temp_C, sp.AlturaUterinaPP_cm,
        sp.InvolucionUterina, sp.Loquios, sp.HemorragiaResidual, sp.Lactancia, sp.ApoyoLactancia,
        sp.SignosInfeccion, sp.TamizajeDepresion, sp.IdMetodoPF, sp.ConsejoPlanificacion,
        sp.VisitaDomiciliariaFecha, sp.SeguroTipo, sp.ComplicacionesMaternas, sp.Derivacion,
        sp.EstablecimientoAtencion, sp.Observaciones, sp.Estado,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(mpf.Nombre, 'No especificado') AS NombreMetodoPF
    FROM
        SeguimientoPuerperio sp
    INNER JOIN
        Embarazo em ON sp.IdEmbarazo = em.IdEmbarazo
    INNER JOIN
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN
        ProfesionalSalud ps ON sp.IdProfesional = ps.IdProfesional
    LEFT JOIN
        MetodoPF mpf ON sp.IdMetodoPF = mpf.IdMetodoPF
    WHERE
        sp.IdPuerperio = @IdPuerperio;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarSeguimientoPuerperio (
    @IdPuerperio INT,
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @DiasPosparto INT = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Temp_C DECIMAL(4,1) = NULL,
    @AlturaUterinaPP_cm DECIMAL(5,1) = NULL,
    @InvolucionUterina VARCHAR(50) = NULL,
    @Loquios NVARCHAR(20) = NULL,
    @HemorragiaResidual BIT = NULL,
    @Lactancia NVARCHAR(20) = NULL,
    @ApoyoLactancia BIT = NULL,
    @SignosInfeccion BIT = NULL,
    @TamizajeDepresion NVARCHAR(20) = NULL,
    @IdMetodoPF SMALLINT = NULL,
    @ConsejoPlanificacion BIT = NULL,
    @VisitaDomiciliariaFecha DATE = NULL,
    @SeguroTipo NVARCHAR(50) = NULL,
    @ComplicacionesMaternas NVARCHAR(300) = NULL,
    @Derivacion BIT = NULL,
    @EstablecimientoAtencion NVARCHAR(100) = NULL,
    @Observaciones NVARCHAR(500) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE SeguimientoPuerperio SET
            IdEmbarazo = @IdEmbarazo,
            IdEncuentro = @IdEncuentro,
            IdProfesional = @IdProfesional,
            Fecha = @Fecha,
            DiasPosparto = @DiasPosparto,
            PA_Sistolica = @PA_Sistolica,
            PA_Diastolica = @PA_Diastolica,
            Temp_C = @Temp_C,
            AlturaUterinaPP_cm = @AlturaUterinaPP_cm,
            InvolucionUterina = @InvolucionUterina,
            Loquios = @Loquios,
            HemorragiaResidual = @HemorragiaResidual,
            Lactancia = @Lactancia,
            ApoyoLactancia = @ApoyoLactancia,
            SignosInfeccion = @SignosInfeccion,
            TamizajeDepresion = @TamizajeDepresion,
            IdMetodoPF = @IdMetodoPF,
            ConsejoPlanificacion = @ConsejoPlanificacion,
            VisitaDomiciliariaFecha = @VisitaDomiciliariaFecha,
            SeguroTipo = @SeguroTipo,
            ComplicacionesMaternas = @ComplicacionesMaternas,
            Derivacion = @Derivacion,
            EstablecimientoAtencion = @EstablecimientoAtencion,
            Observaciones = @Observaciones,
            Estado = @Estado
        WHERE
            IdPuerperio = @IdPuerperio;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_InhabilitarSeguimientoPuerperio (
    @IdPuerperio INT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE SeguimientoPuerperio
        SET Estado = 0
        WHERE IdPuerperio = @IdPuerperio AND Estado = 1;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarMetodoPF
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdMetodoPF, Nombre
    FROM MetodoPF
    ORDER BY Nombre;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarTipoAyuda
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdTipoAyuda, Nombre
    FROM TipoAyudaDiagnostica
    ORDER BY Nombre;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarAyudaDiagnostica
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        a.IdAyuda,
        a.IdPaciente,
        a.IdEmbarazo,
        a.IdProfesional,
        a.IdTipoAyuda,
        a.Descripcion,
        a.Urgente,
        a.FechaOrden,
        a.Estado,
        -- JOINs para nombres
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(ta.Nombre, 'Tipo no especificado') AS NombreTipoAyuda
    FROM 
        AyudaDiagnosticaOrden a
    INNER JOIN 
        Paciente p ON a.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON a.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        TipoAyudaDiagnostica ta ON a.IdTipoAyuda = ta.IdTipoAyuda
    ORDER BY
        a.FechaOrden DESC;
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarAyudaDiagnostica (
    @IdPaciente INT,
    @IdEmbarazo INT = NULL,
    @IdProfesional INT = NULL,
    @IdTipoAyuda SMALLINT = NULL,
    @Descripcion NVARCHAR(200) = NULL,
    @Urgente BIT = 0
    -- FechaOrden y Estado usan defaults de la tabla
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        INSERT INTO AyudaDiagnosticaOrden (
            IdPaciente, IdEmbarazo, IdProfesional, 
            IdTipoAyuda, Descripcion, Urgente
        )
        VALUES (
            @IdPaciente, @IdEmbarazo, @IdProfesional, 
            @IdTipoAyuda, @Descripcion, @Urgente
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarAyudaDiagnostica (
    @IdAyuda INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        a.IdAyuda, a.IdPaciente, a.IdEmbarazo, a.IdProfesional, a.IdTipoAyuda,
        a.Descripcion, a.Urgente, a.FechaOrden, a.Estado,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(ta.Nombre, 'Tipo no especificado') AS NombreTipoAyuda
    FROM 
        AyudaDiagnosticaOrden a
    INNER JOIN 
        Paciente p ON a.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON a.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        TipoAyudaDiagnostica ta ON a.IdTipoAyuda = ta.IdTipoAyuda
    WHERE
        a.IdAyuda = @IdAyuda;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarAyudaDiagnostica (
    @IdAyuda INT,
    @IdPaciente INT,
    @IdEmbarazo INT = NULL,
    @IdProfesional INT = NULL,
    @IdTipoAyuda SMALLINT = NULL,
    @Descripcion NVARCHAR(200) = NULL,
    @Urgente BIT,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE AyudaDiagnosticaOrden SET
            IdPaciente = @IdPaciente,
            IdEmbarazo = @IdEmbarazo,
            IdProfesional = @IdProfesional,
            IdTipoAyuda = @IdTipoAyuda,
            Descripcion = @Descripcion,
            Urgente = @Urgente,
            Estado = @Estado
            -- No editamos FechaOrden
        WHERE
            IdAyuda = @IdAyuda;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_AnularAyudaDiagnostica (
    @IdAyuda INT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE AyudaDiagnosticaOrden
        SET Estado = 'Anulada'
        WHERE IdAyuda = @IdAyuda AND Estado != 'Anulada';
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarViaParto
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdViaParto, Descripcion FROM ViaParto ORDER BY IdViaParto;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarLiquidoAmniotico
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdLiquido, Descripcion FROM LiquidoAmniotico ORDER BY IdLiquido;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarPartos (
    @Estado BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        pa.IdParto, pa.IdEmbarazo, pa.Fecha, pa.Estado,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(vp.Descripcion, 'N/A') AS NombreViaParto,
        ISNULL(la.Descripcion, 'N/A') AS NombreLiquido
    FROM 
        Parto pa
    INNER JOIN 
        Embarazo em ON pa.IdEmbarazo = em.IdEmbarazo
    INNER JOIN 
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud ps ON pa.IdProfesional = ps.IdProfesional
    LEFT JOIN 
        ViaParto vp ON pa.IdViaParto = vp.IdViaParto
    LEFT JOIN 
        LiquidoAmniotico la ON pa.IdLiquido = la.IdLiquido
    WHERE
        pa.Estado = @Estado
    ORDER BY
        pa.Fecha DESC;
END
GO
CREATE OR ALTER PROCEDURE sp_InsertarParto
(
    @IdEmbarazo                 INT,
    @IdEncuentro                INT,          -- requerido, creado por la lógica
    @IdProfesional              INT = NULL,
    @Fecha                      DATE,

    @HoraIngreso                DATETIME2 = NULL,
    @HoraInicioTrabajo          DATETIME2 = NULL,
    @HoraExpulsion              DATETIME2 = NULL,
    @TipoParto                  NVARCHAR(50) = NULL,

    @Membranas                  NVARCHAR(10) = NULL,
    @TiempoRoturaMembranasHoras INT = NULL,
    @IdLiquido                  SMALLINT = NULL,
    @AspectoLiquido             NVARCHAR(50) = NULL,
    @Analgesia                  NVARCHAR(50) = NULL,
    @PosicionMadre              NVARCHAR(50) = NULL,
    @Acompanante                BIT = NULL,

    @IdViaParto                 SMALLINT = NULL,
    @IndicacionCesarea          NVARCHAR(150) = NULL,
    @LugarNacimiento            NVARCHAR(100) = NULL,

    @DuracionSegundaEtapaMinutos INT = NULL,
    @PerdidasML                  INT = NULL,
    @Desgarro                    NVARCHAR(10) = NULL,
    @Episiotomia                 BIT = NULL,

    @ComplicacionesMaternas      NVARCHAR(300) = NULL,
    @Derivacion                  BIT = NULL,
    @SeguroTipo                  NVARCHAR(50) = NULL,

    @NumeroHijosPrevios          INT = NULL,
    @NumeroCesareasPrevias       INT = NULL,
    @EmbarazoMultiple            BIT = NULL,
    @NumeroGemelos               INT = NULL,

    @Observaciones               NVARCHAR(500) = NULL,
    @Estado                      BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ErrorMessage NVARCHAR(4000),
            @ErrorSeverity INT,
            @ErrorState INT;

    BEGIN TRY
        ---------------------------------------
        -- Insertar PARTO
        ---------------------------------------
        INSERT INTO Parto (
            IdEmbarazo,
            IdEncuentro,
            IdProfesional,
            Fecha,
            HoraIngreso,
            HoraInicioTrabajo,
            HoraExpulsion,
            TipoParto,
            IdViaParto,
            IndicacionCesarea,
            Membranas,
            TiempoRoturaMembranasHoras,
            IdLiquido,
            AspectoLiquido,
            Analgesia,
            PosicionMadre,
            Acompanante,
            LugarNacimiento,
            DuracionSegundaEtapaMinutos,
            PerdidasML,
            Desgarro,
            Episiotomia,
            ComplicacionesMaternas,
            Derivacion,
            SeguroTipo,
            NumeroHijosPrevios,
            NumeroCesareasPrevias,
            EmbarazoMultiple,
            NumeroGemelos,
            Observaciones,
            Estado
        )
        VALUES (
            @IdEmbarazo,
            @IdEncuentro,
            @IdProfesional,
            @Fecha,
            @HoraIngreso,
            @HoraInicioTrabajo,
            @HoraExpulsion,
            @TipoParto,
            @IdViaParto,
            @IndicacionCesarea,
            @Membranas,
            @TiempoRoturaMembranasHoras,
            @IdLiquido,
            @AspectoLiquido,
            @Analgesia,
            @PosicionMadre,
            @Acompanante,
            @LugarNacimiento,
            @DuracionSegundaEtapaMinutos,
            @PerdidasML,
            @Desgarro,
            @Episiotomia,
            @ComplicacionesMaternas,
            @Derivacion,
            @SeguroTipo,
            @NumeroHijosPrevios,
            @NumeroCesareasPrevias,
            @EmbarazoMultiple,
            @NumeroGemelos,
            @Observaciones,
            @Estado
        );

        -- devolver IdParto para tu ExecuteScalar()
        SELECT SCOPE_IDENTITY() AS IdParto;
    END TRY
    BEGIN CATCH
        SELECT  @ErrorMessage = ERROR_MESSAGE(),
                @ErrorSeverity = ERROR_SEVERITY(),
                @ErrorState = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarParto (
    @IdParto INT
)
AS
BEGIN
    SET NOCOUNT ON;
    -- 1. Obtener los datos del Parto
    SELECT
        pa.*, -- Traemos todos los campos de Parto
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ISNULL(ps.Nombres + ' ' + ps.Apellidos, 'No Asignado') AS NombreProfesional,
        ISNULL(vp.Descripcion, 'N/A') AS NombreViaParto,
        ISNULL(la.Descripcion, 'N/A') AS NombreLiquido
    FROM
        Parto pa
    INNER JOIN
        Embarazo em ON pa.IdEmbarazo = em.IdEmbarazo
    INNER JOIN
        Paciente p ON em.IdPaciente = p.IdPaciente
    LEFT JOIN
        ProfesionalSalud ps ON pa.IdProfesional = ps.IdProfesional
    LEFT JOIN
        ViaParto vp ON pa.IdViaParto = vp.IdViaParto
    LEFT JOIN
        LiquidoAmniotico la ON pa.IdLiquido = la.IdLiquido
    WHERE
        pa.IdParto = @IdParto;
    -- 2. Obtener las intervenciones asociadas a ese Parto
    SELECT
        IdPartoIntervencion,
        Intervencion
    FROM
        PartoIntervencion
    WHERE
        IdParto = @IdParto;
    -- 3. Obtener los bebés asociados a ese Parto
    SELECT
        IdBebe,
        NumeroBebe,
        Sexo,
        FechaHoraNacimiento,
        Apgar1,
        Apgar5,
        PesoGr,
        TallaCm,
        PerimetroCefalico,
        EG_Semanas,
        Reanimacion,
        Observaciones,
        Estado
    FROM
        Bebe
    WHERE
        IdParto = @IdParto;
END
GO
CREATE OR ALTER PROCEDURE sp_EditarParto (
    @IdParto INT,
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @HoraIngreso DATETIME2 = NULL,
    @HoraInicioTrabajo DATETIME2 = NULL,
    @HoraExpulsion DATETIME2 = NULL,
    @TipoParto NVARCHAR(50) = NULL,
    @Membranas NVARCHAR(10) = NULL,
    @TiempoRoturaMembranasHoras INT = NULL,
    @IdLiquido SMALLINT = NULL,
    @AspectoLiquido NVARCHAR(50) = NULL,
    @Analgesia NVARCHAR(50) = NULL,
    @PosicionMadre NVARCHAR(50) = NULL,
    @Acompanante BIT = NULL,
    @IdViaParto SMALLINT = NULL,
    @IndicacionCesarea NVARCHAR(150) = NULL,
    @LugarNacimiento NVARCHAR(100) = NULL,
    @DuracionSegundaEtapaMinutos INT = NULL,
    @PerdidasML INT = NULL,
    @Desgarro NVARCHAR(10) = NULL,
    @Episiotomia BIT = NULL,
    @Complicaciones NVARCHAR(300) = NULL,
    @ComplicacionesMaternas NVARCHAR(300) = NULL,
    @Derivacion BIT = NULL,
    @SeguroTipo NVARCHAR(50) = NULL,
    @NumeroHijosPrevios INT = NULL,
    @NumeroCesareasPrevias INT = NULL,
    @EmbarazoMultiple BIT = NULL,
    @NumeroGemelos INT = NULL,
    @Observaciones NVARCHAR(500) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE Parto SET
            IdEmbarazo = @IdEmbarazo,
            IdEncuentro = @IdEncuentro,
            IdProfesional = @IdProfesional,
            Fecha = @Fecha,
            HoraIngreso = @HoraIngreso,
            HoraInicioTrabajo = @HoraInicioTrabajo,
            HoraExpulsion = @HoraExpulsion,
            TipoParto = @TipoParto,
            Membranas = @Membranas,
            TiempoRoturaMembranasHoras = @TiempoRoturaMembranasHoras,
            IdLiquido = @IdLiquido,
            AspectoLiquido = @AspectoLiquido,
            Analgesia = @Analgesia,
            PosicionMadre = @PosicionMadre,
            Acompanante = @Acompanante,
            IdViaParto = @IdViaParto,
            IndicacionCesarea = @IndicacionCesarea,
            LugarNacimiento = @LugarNacimiento,
            DuracionSegundaEtapaMinutos = @DuracionSegundaEtapaMinutos,
            PerdidasML = @PerdidasML,
            Desgarro = @Desgarro,
            Episiotomia = @Episiotomia,
            Complicaciones = @Complicaciones,
            ComplicacionesMaternas = @ComplicacionesMaternas,
            Derivacion = @Derivacion,
            SeguroTipo = @SeguroTipo,
            NumeroHijosPrevios = @NumeroHijosPrevios,
            NumeroCesareasPrevias = @NumeroCesareasPrevias,
            EmbarazoMultiple = @EmbarazoMultiple,
            NumeroGemelos = @NumeroGemelos,
            Observaciones = @Observaciones,
            Estado = @Estado
        WHERE
            IdParto = @IdParto;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_AnularParto (
    @IdParto INT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        UPDATE Parto
        SET Estado = 0
        WHERE IdParto = @IdParto AND Estado = 1;
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END
GO
CREATE OR ALTER PROCEDURE sp_ListarEncuentrosPorEmbarazo (
    @IdEmbarazo INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.IdEncuentro,
        -- Creamos un texto descriptivo para el dropdown
        te.Descripcion + ' (' + CONVERT(varchar, e.FechaHoraInicio, 103) + ')' AS EncuentroDesc
    FROM 
        Encuentro e
    LEFT JOIN 
        TipoEncuentro te ON e.IdTipoEncuentro = te.IdTipoEncuentro
    WHERE
        e.IdEmbarazo = @IdEmbarazo
        AND e.Estado != 'Anulado'
    ORDER BY
        e.FechaHoraInicio DESC;
END
GO
CREATE OR ALTER PROCEDURE sp_Usuario_ObtenerPorNombre
  @NombreUsuario NVARCHAR(50)
AS
BEGIN
    SELECT 
        u.IdUsuario, 
        u.NombreUsuario, 
        u.ClaveHash, 
        u.Email, 
        u.Estado,
        ur.IdRol,
        r.NombreRol
    FROM Usuario u
    LEFT JOIN UsuarioRol ur ON u.IdUsuario = ur.IdUsuario
    LEFT JOIN Rol r ON ur.IdRol = r.IdRol
    WHERE u.NombreUsuario = @NombreUsuario AND u.Estado = 1;
END

GO
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'DIU T de Cobre')
    INSERT INTO MetodoPF (Nombre) VALUES ('DIU T de Cobre');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Implante Subdérmico')
    INSERT INTO MetodoPF (Nombre) VALUES ('Implante Subdérmico');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Píldoras Anticonceptivas')
    INSERT INTO MetodoPF (Nombre) VALUES ('Píldoras Anticonceptivas');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Inyección Trimestral')
    INSERT INTO MetodoPF (Nombre) VALUES ('Inyección Trimestral');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Ligadura de Trompas (AQV)')
    INSERT INTO MetodoPF (Nombre) VALUES ('Ligadura de Trompas (AQV)');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Vasectomía (AQV)')
    INSERT INTO MetodoPF (Nombre) VALUES ('Vasectomía (AQV)');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Preservativo (Condón)')
    INSERT INTO MetodoPF (Nombre) VALUES ('Preservativo (Condón)');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Método del Ritmo (MELA)')
    INSERT INTO MetodoPF (Nombre) VALUES ('Método del Ritmo (MELA)');
IF NOT EXISTS (SELECT 1 FROM MetodoPF WHERE Nombre = 'Ninguno')
    INSERT INTO MetodoPF (Nombre) VALUES ('Ninguno');
GO
CREATE OR ALTER PROCEDURE sp_Usuario_ObtenerRoles
  @IdUsuario INT
AS
BEGIN
    SELECT 
        ur.IdUsuarioRol,     -- 👈 importante
        ur.IdUsuario,
        r.IdRol,
        r.NombreRol,
        r.Descripcion,
        r.Estado
    FROM UsuarioRol ur
    INNER JOIN Rol r ON ur.IdRol = r.IdRol
    WHERE ur.IdUsuario = @IdUsuario AND r.Estado = 1;
END

GO
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Ecografía Obstétrica')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Ecografía Obstétrica');
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Perfil Biofísico')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Perfil Biofísico');
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Monitoreo Fetal (NST)')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Monitoreo Fetal (NST)');
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Análisis de Sangre (Hemograma)')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Análisis de Sangre (Hemograma)');
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Análisis de Orina (Completo)')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Análisis de Orina (Completo)');
IF NOT EXISTS (SELECT 1 FROM TipoAyudaDiagnostica WHERE Nombre = 'Test de Tolerancia a la Glucosa')
    INSERT INTO TipoAyudaDiagnostica (Nombre) VALUES ('Test de Tolerancia a la Glucosa');
GO
CREATE OR ALTER PROCEDURE sp_ListarEmbarazosActivos
(
    @Estado BIT 
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        E.IdEmbarazo,
        E.IdPaciente,
        P.Nombres + ' ' + P.Apellidos AS NombrePaciente,
        E.FPP,
        E.Estado 
    FROM
        Embarazo E
    JOIN
        Paciente P ON E.IdPaciente = P.IdPaciente
    WHERE
        E.Estado = @Estado 
    ORDER BY
        P.Apellidos, P.Nombres;
END


GO

-- SP PARA BEBE --

CREATE OR ALTER PROCEDURE sp_ListarBebe
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        b.IdBebe,
        b.IdParto,
        b.NumeroBebe,
        b.Sexo,
        b.FechaHoraNacimiento,
        b.Apgar1,
        b.Apgar5,
        b.PesoGr,
        b.TallaCm,
        b.PerimetroCefalico,
        b.EG_Semanas,
        b.Reanimacion,
        b.Observaciones,
        b.Estado
    FROM Bebe b
    ORDER BY b.IdBebe;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarBebe
    @IdParto INT,
    @NumeroBebe INT = 1,
    @Sexo CHAR(1) = NULL,
    @FechaHoraNacimiento DATETIME2 = NULL,
    @Apgar1 TINYINT = NULL,
    @Apgar5 TINYINT = NULL,
    @PesoGr INT = NULL,
    @TallaCm DECIMAL(4,1) = NULL,
    @PerimetroCefalico DECIMAL(4,1) = NULL,
    @EG_Semanas DECIMAL(4,1) = NULL,
    @Reanimacion BIT = NULL,
    @Observaciones NVARCHAR(200) = NULL,
    @Estado BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Bebe (
        IdParto,
        NumeroBebe,
        Sexo,
        FechaHoraNacimiento,
        Apgar1,
        Apgar5,
        PesoGr,
        TallaCm,
        PerimetroCefalico,
        EG_Semanas,
        Reanimacion,
        Observaciones,
        Estado
    )
    VALUES (
        @IdParto,
        @NumeroBebe,
        @Sexo,
        @FechaHoraNacimiento,
        @Apgar1,
        @Apgar5,
        @PesoGr,
        @TallaCm,
        @PerimetroCefalico,
        @EG_Semanas,
        @Reanimacion,
        @Observaciones,
        @Estado
    );
    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_EditarBebe
    @IdBebe INT,
    @IdParto INT,
    @NumeroBebe INT = 1,
    @Sexo CHAR(1) = NULL,
    @FechaHoraNacimiento DATETIME2 = NULL,
    @Apgar1 TINYINT = NULL,
    @Apgar5 TINYINT = NULL,
    @PesoGr INT = NULL,
    @TallaCm DECIMAL(4,1) = NULL,
    @PerimetroCefalico DECIMAL(4,1) = NULL,
    @EG_Semanas DECIMAL(4,1) = NULL,
    @Reanimacion BIT = NULL,
    @Observaciones NVARCHAR(200) = NULL,
    @Estado BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Bebe
    SET
        IdParto = @IdParto,
        NumeroBebe = @NumeroBebe,
        Sexo = @Sexo,
        FechaHoraNacimiento = @FechaHoraNacimiento,
        Apgar1 = @Apgar1,
        Apgar5 = @Apgar5,
        PesoGr = @PesoGr,
        TallaCm = @TallaCm,
        PerimetroCefalico = @PerimetroCefalico,
        EG_Semanas = @EG_Semanas,
        Reanimacion = @Reanimacion,
        Observaciones = @Observaciones,
        Estado = @Estado
    WHERE IdBebe = @IdBebe;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarBebe
    @IdBebe INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        b.IdBebe,
        b.IdParto,
        b.NumeroBebe,
        b.Sexo,
        b.FechaHoraNacimiento,
        b.Apgar1,
        b.Apgar5,
        b.PesoGr,
        b.TallaCm,
        b.PerimetroCefalico,
        b.EG_Semanas,
        b.Reanimacion,
        b.Observaciones,
        b.Estado
    FROM Bebe b
    WHERE b.IdBebe = @IdBebe;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarBebe
    @IdBebe INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Bebe WHERE IdBebe = @IdBebe;
END
GO
CREATE OR ALTER PROCEDURE sp_ListarRol
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdRol,
        NombreRol,
        Descripcion,
        Estado
    FROM Rol
    ORDER BY IdRol;
END;
GO
CREATE OR ALTER PROCEDURE sp_EditarUsuario
(
    @IdUsuario     INT,
    @Username      NVARCHAR(50)   = NULL,  -- NombreUsuario (nullable: si viene NULL, no cambia)
    @PasswordHash  NVARCHAR(500)  = NULL,  -- ClaveHash     (nullable: si viene NULL, no cambia)
    @Correo        NVARCHAR(100)  = NULL,  -- email         (nullable: si viene NULL, no cambia)
    @IdRol         INT,                    -- rol requerido
    @Estado        BIT                     -- estado requerido
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRAN;

        -- 1) Validaciones
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuario WHERE IdUsuario = @IdUsuario)
            THROW 50000, 'El usuario indicado no existe.', 1;

        IF NOT EXISTS (SELECT 1 FROM dbo.Rol WHERE IdRol = @IdRol AND Estado = 1)
            THROW 50001, 'El rol indicado no existe o está inactivo.', 1;

        IF @Username IS NOT NULL
           AND EXISTS (SELECT 1 FROM dbo.Usuario WHERE NombreUsuario = @Username AND IdUsuario <> @IdUsuario)
            THROW 50002, 'El NombreUsuario ya está en uso por otro usuario.', 1;

        -- (Opcional) Validar correo único si lo quieres único:
        -- IF @Correo IS NOT NULL
        --    AND EXISTS (SELECT 1 FROM dbo.Usuario WHERE email = @Correo AND IdUsuario <> @IdUsuario)
        --     THROW 50003, 'El correo ya está en uso por otro usuario.', 1;

        -- 2) Actualización parcial: solo cambia lo que no llega NULL
        UPDATE dbo.Usuario
        SET
            NombreUsuario = COALESCE(@Username, NombreUsuario),
            ClaveHash     = COALESCE(@PasswordHash, ClaveHash),
            email         = COALESCE(@Correo, email),
            Estado        = @Estado
        WHERE IdUsuario = @IdUsuario;

        -- 3) Asegurar el rol (un único rol por usuario):
        --    Si ya tiene el mismo rol, se conservan; si no, se reemplazan.
        IF EXISTS (SELECT 1 FROM dbo.UsuarioRol WHERE IdUsuario = @IdUsuario AND IdRol = @IdRol)
        BEGIN
            -- El rol ya es el correcto; elimina otros si existen
            DELETE FROM dbo.UsuarioRol
            WHERE IdUsuario = @IdUsuario AND IdRol <> @IdRol;
        END
        ELSE
        BEGIN
            -- Reemplazar roles actuales por el nuevo
            DELETE FROM dbo.UsuarioRol WHERE IdUsuario = @IdUsuario;
            INSERT INTO dbo.UsuarioRol (IdUsuario, IdRol)
            VALUES (@IdUsuario, @IdRol);
        END

        COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK;
        THROW; -- Propaga el error a C#
    END CATCH
END;
GO
CREATE OR ALTER PROCEDURE sp_BuscarRol
    @IdRol INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdRol,
           NombreRol,
           Descripcion,
           Estado
    FROM Rol
    WHERE IdRol = @IdRol;
END;
GO
CREATE OR ALTER PROCEDURE sp_Rol_ObtenerPorNombre
    @NombreRol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT IdRol,
           NombreRol,
           Descripcion,
           Estado
    FROM Rol
    WHERE UPPER(NombreRol) = UPPER(@NombreRol);
END;
GO
CREATE OR ALTER  PROCEDURE sp_InsertarUsuario
    @Username      NVARCHAR(50),
    @PasswordHash  NVARCHAR(500),
    @Correo        NVARCHAR(100),
    @IdRol         INT,
    @Estado        BIT,
    @NewIdUsuario  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Usuario (NombreUsuario, ClaveHash, Email, Estado)
    VALUES (@Username, @PasswordHash, @Correo, @Estado);

    SET @NewIdUsuario = CAST(SCOPE_IDENTITY() AS INT);

    INSERT INTO UsuarioRol (IdUsuario, IdRol)
    VALUES (@NewIdUsuario, @IdRol);
END;
GO

CREATE OR ALTER PROCEDURE sp_InsertarEmbarazo
(
    @IdPaciente INT,
    @FUR DATE = NULL, -- Fecha Última Regla (opcional)
    @FPP DATE = NULL, -- Fecha Probable de Parto (opcional)
    @Riesgo NVARCHAR(50) = NULL -- (opcional)
)
AS
BEGIN
    INSERT INTO Embarazo (
        IdPaciente,
        FUR,
        FPP,
        Riesgo,
        Estado
        -- FechaApertura usa DEFAULT SYSUTCDATETIME()
        -- Estado usa DEFAULT 1
    )
    VALUES (
        @IdPaciente,
        @FUR,
        @FPP,
        @Riesgo,
        1 -- 1 = Activo
    );
    
    -- Devolvemos el ID del Embarazo recién creado
    SELECT SCOPE_IDENTITY(); 
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarEmbarazoPorId
(
    @IdEmbarazo INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        E.*, -- Todos los campos de Embarazo
        P.Nombres + ' ' + P.Apellidos AS NombrePaciente
    FROM 
        Embarazo E
    JOIN 
        Paciente P ON E.IdPaciente = P.IdPaciente
    WHERE 
        E.IdEmbarazo = @IdEmbarazo;
END


GO

/* Para detalle Orden-Ayuadas-Diagnosticas*/



CREATE OR ALTER PROCEDURE sp_ListarAyudasPorControl
(
    @IdControl INT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        cp_ad.IdCP_AD,
        cp_ad.IdControl,
        cp_ad.IdAyuda,
        ado.IdTipoAyuda,
        cp_ad.FechaOrden,
        cp_ad.Comentario,
        cp_ad.Estado,
        ta.Nombre AS NombreTipoAyuda,
        ado.Descripcion,
        ado.Urgente
    FROM
        ControlPrenatal_AyudaDiagnostica cp_ad
    JOIN
        AyudaDiagnosticaOrden ado ON cp_ad.IdAyuda = ado.IdAyuda
    LEFT JOIN
        TipoAyudaDiagnostica ta ON ado.IdTipoAyuda = ta.IdTipoAyuda
    WHERE
        cp_ad.IdControl = @IdControl
        AND cp_ad.Estado = 1
    ORDER BY
        cp_ad.FechaOrden DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarControlPrenatal_AyudaDiagnostica
(
    @IdControl INT,
    @IdAyuda INT,
    @FechaOrden DATETIME2 = NULL,
    @Comentario NVARCHAR(200) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ControlPrenatal_AyudaDiagnostica (
        IdControl,
        IdAyuda,
        FechaOrden,
        Comentario,
        Estado
    )
    VALUES (
        @IdControl,
        @IdAyuda,
        @FechaOrden,
        @Comentario,
        1
    );

    SELECT SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE sp_EditarControlPrenatal_AyudaDiagnostica
(
    @IdCP_AD INT,
    @IdControl INT,
    @IdAyuda INT,
    @FechaOrden DATETIME2 = NULL,
    @Comentario NVARCHAR(200) = NULL,
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ControlPrenatal_AyudaDiagnostica
    SET
        IdControl = @IdControl,
        IdAyuda = @IdAyuda,
        FechaOrden = @FechaOrden,
        Comentario = @Comentario,
        Estado = @Estado
    WHERE
        IdCP_AD = @IdCP_AD;
END
GO


CREATE OR ALTER PROCEDURE sp_InhabilitarControlPrenatal_AyudaDiagnostica
(
    @IdCP_AD INT
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ControlPrenatal_AyudaDiagnostica
    SET Estado = 0
    WHERE IdCP_AD = @IdCP_AD;
END
GO


-- Resultado Diagmnostico --
 CREATE OR ALTER PROCEDURE sp_ListarResultadoDiagnostico
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdResultado,
        IdAyuda,
        FechaResultado,
        Resumen,
        Critico,
        Estado
    FROM ResultadoDiagnostico
    ORDER BY FechaResultado DESC;
END
GO
 



 CREATE OR ALTER PROCEDURE sp_InsertarResultadoDiagnostico
(
    @IdAyuda INT,
    @FechaResultado DATE,
    @Resumen NVARCHAR(500) = NULL,
    @Critico BIT,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    INSERT INTO ResultadoDiagnostico
    (
        IdAyuda,
        FechaResultado,
        Resumen,
        Critico,
        Estado
    )
    VALUES 
    (
        @IdAyuda,
        @FechaResultado,
        @Resumen,
        @Critico,
        @Estado
    );

    SELECT SCOPE_IDENTITY() AS IdResultado;
END
GO
CREATE OR ALTER PROCEDURE sp_ActualizarResultadoDiagnostico
(
    @IdResultado INT,
    @IdAyuda INT,
    @FechaResultado DATE,
    @Resumen NVARCHAR(500) = NULL,
    @Critico BIT,
    @Estado NVARCHAR(20)
)
AS
BEGIN
    UPDATE ResultadoDiagnostico
    SET
        IdAyuda = @IdAyuda,
        FechaResultado = @FechaResultado,
        Resumen = @Resumen,
        Critico = @Critico,
        Estado = @Estado
    WHERE IdResultado = @IdResultado;
END
GO
CREATE OR ALTER PROCEDURE sp_BuscarResultadoDiagnostico
(
    @IdResultado INT
)
AS
BEGIN
    SELECT
        IdResultado,
        IdAyuda,
        FechaResultado,
        Resumen,
        Critico,
        Estado
    FROM ResultadoDiagnostico
    WHERE IdResultado = @IdResultado;
END
GO
CREATE OR ALTER PROCEDURE sp_AnularResultadoDiagnostico
(
    @IdResultado INT
)
AS
BEGIN
    UPDATE ResultadoDiagnostico
    SET Estado = 'INACTIVO'
    WHERE IdResultado = @IdResultado;
END
GO

-- Antecedentes Obstétricos --

CREATE OR ALTER PROCEDURE sp_ListarAntecedenteObstetrico
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ao.IdAntecedente,
        ao.IdPaciente,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ao.Menarquia,
        ao.CicloDias,
        ao.Gestas,
        ao.Partos,
        ao.Abortos,
        ao.Observacion,
        ao.Estado
    FROM AntecedenteObstetrico ao
    INNER JOIN Paciente p ON ao.IdPaciente = p.IdPaciente
    ORDER BY ao.IdAntecedente DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarAntecedenteObstetrico
(
    @IdAntecedente INT
)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ao.IdAntecedente,
        ao.IdPaciente,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        ao.Menarquia,
        ao.CicloDias,
        ao.Gestas,
        ao.Partos,
        ao.Abortos,
        ao.Observacion,
        ao.Estado
    FROM AntecedenteObstetrico ao
    INNER JOIN Paciente p ON ao.IdPaciente = p.IdPaciente
    WHERE ao.IdAntecedente = @IdAntecedente;
END
GO

CREATE OR ALTER PROCEDURE sp_InsertarAntecedenteObstetrico
(
    @IdPaciente INT,
    @Menarquia SMALLINT = NULL,
    @CicloDias SMALLINT = NULL,
    @Gestas SMALLINT = NULL,
    @Partos SMALLINT = NULL,
    @Abortos SMALLINT = NULL,
    @Observacion NVARCHAR(300) = NULL,
    @Estado BIT = 1
)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO AntecedenteObstetrico (
        IdPaciente,
        Menarquia,
        CicloDias,
        Gestas,
        Partos,
        Abortos,
        Observacion,
        Estado
    )
    VALUES (
        @IdPaciente,
        @Menarquia,
        @CicloDias,
        @Gestas,
        @Partos,
        @Abortos,
        @Observacion,
        @Estado
    );
    SELECT SCOPE_IDENTITY() AS IdAntecedente;
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarAntecedenteObstetrico
(
    @IdAntecedente INT,
    @IdPaciente INT,
    @Menarquia SMALLINT = NULL,
    @CicloDias SMALLINT = NULL,
    @Gestas SMALLINT = NULL,
    @Partos SMALLINT = NULL,
    @Abortos SMALLINT = NULL,
    @Observacion NVARCHAR(300) = NULL,
    @Estado BIT
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AntecedenteObstetrico
    SET
        IdPaciente = @IdPaciente,
        Menarquia = @Menarquia,
        CicloDias = @CicloDias,
        Gestas = @Gestas,
        Partos = @Partos,
        Abortos = @Abortos,
        Observacion = @Observacion,
        Estado = @Estado
    WHERE IdAntecedente = @IdAntecedente;
END
GO

CREATE OR ALTER PROCEDURE sp_AnularAntecedenteObstetrico
(
    @IdAntecedente INT
)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE AntecedenteObstetrico
    SET Estado = 0
    WHERE IdAntecedente = @IdAntecedente;
END
GO

CREATE OR ALTER PROCEDURE sp_EliminarAntecedenteObstetrico
(
    @IdAntecedente INT
)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM AntecedenteObstetrico
    WHERE IdAntecedente = @IdAntecedente;
END
GO
