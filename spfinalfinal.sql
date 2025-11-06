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
    
    @TipoTelefono NVARCHAR(20) = 'Celular' 
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
CREATE OR ALTER PROCEDURE sp_InsertarEncuentro (
    @IdEmbarazo INT,
    @IdProfesional INT = NULL,
    @IdTipoEncuentro SMALLINT,
    @Notas NVARCHAR(500) = NULL
)
AS
BEGIN
    -- No usamos SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;

    BEGIN TRY
        -- La tabla Encuentro ya tiene defaults para FechaHoraInicio y Estado
        INSERT INTO Encuentro (
            IdEmbarazo, 
            IdProfesional, 
            IdTipoEncuentro, 
            Notas
        )
        VALUES (
            @IdEmbarazo, 
            @IdProfesional, 
            @IdTipoEncuentro, 
            @Notas
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
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
        c.IdControl, -- Nota: SQL usa IdControl
        c.IdEmbarazo,
        c.IdEncuentro,
        c.IdProfesional,
        c.Fecha,
        c.PesoKg, c.TallaM, c.PA_Sistolica, c.PA_Diastolica,
        c.AlturaUterina_cm, c.FCF_bpm, c.Presentacion, c.Proteinuria,
        c.MovFetales, c.Consejerias, c.Observaciones,
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
    @PesoKg DECIMAL(5,2) = NULL,
    @TallaM DECIMAL(3,2) = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @AlturaUterina_cm DECIMAL(4,1) = NULL,
    @FCF_bpm TINYINT = NULL,
    @Presentacion NVARCHAR(50) = NULL,
    @Proteinuria NVARCHAR(10) = NULL,
    @MovFetales BIT = NULL,
    @Consejerias NVARCHAR(200) = NULL,
    @Observaciones NVARCHAR(300) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        INSERT INTO ControlPrenatal (
            IdEmbarazo, IdEncuentro, IdProfesional, Fecha, PesoKg, TallaM, 
            PA_Sistolica, PA_Diastolica, AlturaUterina_cm, FCF_bpm, Presentacion, 
            Proteinuria, MovFetales, Consejerias, Observaciones, Estado
        )
        VALUES (
            @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @PesoKg, @TallaM,
            @PA_Sistolica, @PA_Diastolica, @AlturaUterina_cm, @FCF_bpm, @Presentacion, 
            @Proteinuria, @MovFetales, @Consejerias, @Observaciones, @Estado
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
        c.PesoKg, c.TallaM, c.PA_Sistolica, c.PA_Diastolica,
        c.AlturaUterina_cm, c.FCF_bpm, c.Presentacion, c.Proteinuria,
        c.MovFetales, c.Consejerias, c.Observaciones, c.Estado,
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
    @PesoKg DECIMAL(5,2) = NULL,
    @TallaM DECIMAL(3,2) = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @AlturaUterina_cm DECIMAL(4,1) = NULL,
    @FCF_bpm TINYINT = NULL,
    @Presentacion NVARCHAR(50) = NULL,
    @Proteinuria NVARCHAR(10) = NULL,
    @MovFetales BIT = NULL,
    @Consejerias NVARCHAR(200) = NULL,
    @Observaciones NVARCHAR(300) = NULL,
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
            PesoKg = @PesoKg,
            TallaM = @TallaM,
            PA_Sistolica = @PA_Sistolica,
            PA_Diastolica = @PA_Diastolica,
            AlturaUterina_cm = @AlturaUterina_cm,
            FCF_bpm = @FCF_bpm,
            Presentacion = @Presentacion,
            Proteinuria = @Proteinuria,
            MovFetales = @MovFetales,
            Consejerias = @Consejerias,
            Observaciones = @Observaciones,
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
CREATE OR ALTER PROCEDURE sp_ListarSeguimientoPuerperio (
    @Estado BIT = 1 -- Para filtrar por estado
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
        sp.PA_Sistolica, sp.PA_Diastolica, sp.Temp_C,
        sp.AlturaUterinaPP_cm, sp.Loquios, sp.Lactancia,
        sp.SignosInfeccion, sp.TamizajeDepresion, sp.Observaciones,
        sp.Estado,
        -- JOINs para nombres
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
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Temp_C DECIMAL(4,1) = NULL,
    @AlturaUterinaPP_cm DECIMAL(4,1) = NULL,
    @Loquios NVARCHAR(20) = NULL,
    @Lactancia NVARCHAR(20) = NULL,
    @SignosInfeccion BIT = NULL,
    @TamizajeDepresion NVARCHAR(20) = NULL,
    @IdMetodoPF SMALLINT = NULL,
    @Observaciones NVARCHAR(300) = NULL,
    @Estado BIT
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        INSERT INTO SeguimientoPuerperio (
            IdEmbarazo, IdEncuentro, IdProfesional, Fecha, PA_Sistolica, PA_Diastolica,
            Temp_C, AlturaUterinaPP_cm, Loquios, Lactancia, SignosInfeccion,
            TamizajeDepresion, IdMetodoPF, Observaciones, Estado
        )
        VALUES (
            @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @PA_Sistolica, @PA_Diastolica,
            @Temp_C, @AlturaUterinaPP_cm, @Loquios, @Lactancia, @SignosInfeccion,
            @TamizajeDepresion, @IdMetodoPF, @Observaciones, @Estado
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
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
        sp.PA_Sistolica, sp.PA_Diastolica, sp.Temp_C, sp.AlturaUterinaPP_cm, 
        sp.Loquios, sp.Lactancia, sp.SignosInfeccion, sp.TamizajeDepresion, 
        sp.IdMetodoPF, sp.Observaciones, sp.Estado,
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
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @Temp_C DECIMAL(4,1) = NULL,
    @AlturaUterinaPP_cm DECIMAL(4,1) = NULL,
    @Loquios NVARCHAR(20) = NULL,
    @Lactancia NVARCHAR(20) = NULL,
    @SignosInfeccion BIT = NULL,
    @TamizajeDepresion NVARCHAR(20) = NULL,
    @IdMetodoPF SMALLINT = NULL,
    @Observaciones NVARCHAR(300) = NULL,
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
            PA_Sistolica = @PA_Sistolica,
            PA_Diastolica = @PA_Diastolica,
            Temp_C = @Temp_C,
            AlturaUterinaPP_cm = @AlturaUterinaPP_cm,
            Loquios = @Loquios,
            Lactancia = @Lactancia,
            SignosInfeccion = @SignosInfeccion,
            TamizajeDepresion = @TamizajeDepresion,
            IdMetodoPF = @IdMetodoPF,
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
CREATE OR ALTER PROCEDURE sp_InsertarParto (
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @HoraIngreso DATETIME2 = NULL,
    @HoraInicioTrabajo DATETIME2 = NULL,
    @Membranas NVARCHAR(10) = NULL,
    @IdLiquido SMALLINT = NULL,
    @Analgesia NVARCHAR(50) = NULL,
    @IdViaParto SMALLINT = NULL,
    @IndicacionCesarea NVARCHAR(150) = NULL,
    @PerdidasML INT = NULL,
    @Desgarro NVARCHAR(10) = NULL,
    @Complicaciones NVARCHAR(200) = NULL,
    @Estado BIT = 1
)
AS
BEGIN
    -- No SET NOCOUNT ON
    DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
    BEGIN TRY
        INSERT INTO Parto (
            IdEmbarazo, IdEncuentro, IdProfesional, Fecha, HoraIngreso, HoraInicioTrabajo,
            Membranas, IdLiquido, Analgesia, IdViaParto, IndicacionCesarea,
            PerdidasML, Desgarro, Complicaciones, Estado
        )
        VALUES (
            @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @HoraIngreso, @HoraInicioTrabajo,
            @Membranas, @IdLiquido, @Analgesia, @IdViaParto, @IndicacionCesarea,
            @PerdidasML, @Desgarro, @Complicaciones, @Estado
        );
    END TRY
    BEGIN CATCH
        SELECT @ErrorMessage = ERROR_MESSAGE(), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
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
    @Membranas NVARCHAR(10) = NULL,
    @IdLiquido SMALLINT = NULL,
    @Analgesia NVARCHAR(50) = NULL,
    @IdViaParto SMALLINT = NULL,
    @IndicacionCesarea NVARCHAR(150) = NULL,
    @PerdidasML INT = NULL,
    @Desgarro NVARCHAR(10) = NULL,
    @Complicaciones NVARCHAR(200) = NULL,
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
            Membranas = @Membranas,
            IdLiquido = @IdLiquido,
            Analgesia = @Analgesia,
            IdViaParto = @IdViaParto,
            IndicacionCesarea = @IndicacionCesarea,
            PerdidasML = @PerdidasML,
            Desgarro = @Desgarro,
            Complicaciones = @Complicaciones,
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