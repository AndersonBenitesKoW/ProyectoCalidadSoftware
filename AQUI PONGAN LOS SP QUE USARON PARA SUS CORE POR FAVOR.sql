CREATE  PROCEDURE sp_InsertarEmbarazo
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

CREATE  PROCEDURE sp_BuscarEmbarazoPorId
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

CREATE PROCEDURE sp_ListarEmbarazosActivos
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

CREATE PROCEDURE sp_ListarProfesionalSalud
(
    @Estado BIT -- Parámetro requerido: 1 para activos, 0 para inactivos
)
AS
BEGIN
  SELECT IdProfesional, IdUsuario, CMP, Especialidad, Nombres, Apellidos, Estado
  FROM ProfesionalSalud
  WHERE Estado = @Estado; -- Filtro directo
END
GO

CREATE PROCEDURE sp_InsertarProfesionalSalud
  @CMP NVARCHAR(20),
  @Especialidad NVARCHAR(80),
  @Nombres NVARCHAR(100),
  @Apellidos NVARCHAR(100)
AS
BEGIN
  INSERT INTO ProfesionalSalud (CMP, Especialidad, Nombres, Apellidos)
  VALUES (@CMP, @Especialidad, @Nombres, @Apellidos);

  SELECT SCOPE_IDENTITY() AS IdProfesional;
END
GO

CREATE PROCEDURE sp_BuscarProfesionalSalud
(
    @IdProfesional INT
)
AS
BEGIN
    SELECT IdProfesional, IdUsuario, CMP, Especialidad, Nombres, Apellidos, Estado
    FROM ProfesionalSalud
    WHERE IdProfesional = @IdProfesional;
END
GO

CREATE PROCEDURE sp_EditarProfesionalSalud
(
    @IdProfesional  INT,
    @Nombres        VARCHAR(100),
    @Apellidos      VARCHAR(100),
    @Especialidad   VARCHAR(100),
    @Colegiatura    VARCHAR(50), -- (Mapeado desde CMP)
    @Estado         BIT
)
AS
BEGIN
    UPDATE ProfesionalSalud
    SET
        Nombres = @Nombres,
        Apellidos = @Apellidos,
        Especialidad = @Especialidad,
        CMP = @Colegiatura,
        Estado = @Estado
    WHERE
        IdProfesional = @IdProfesional;
END
GO

CREATE PROCEDURE sp_InsertarParto
    @IdEmbarazo INT,
    @IdEncuentro INT, -- 👈 RE-AGREGADO: Ahora es un parámetro obligatorio
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
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. ELIMINAMOS la lógica que creaba el Encuentro automáticamente.
    
    -- 2. Insertar el Parto (usando el @IdEncuentro manual)
    INSERT INTO Parto (
        IdEmbarazo, 
        IdEncuentro, -- 👈 Usamos el ID que viene del formulario
        IdProfesional, 
        Fecha, 
        HoraIngreso, 
        HoraInicioTrabajo,
        Membranas, 
        IdLiquido, 
        Analgesia, 
        IdViaParto, 
        IndicacionCesarea,
        PerdidasML, 
        Desgarro, 
        Complicaciones, 
        Estado
    )
    VALUES (
        @IdEmbarazo, 
        @IdEncuentro, -- 👈 VALOR ASIGNADO
        @IdProfesional, 
        @Fecha, 
        @HoraIngreso, 
        @HoraInicioTrabajo,
        @Membranas, 
        @IdLiquido, 
        @Analgesia, 
        @IdViaParto, 
        @IndicacionCesarea,
        @PerdidasML, 
        @Desgarro, 
        @Complicaciones, 
        @Estado
    );
    
    -- 3. Devolvemos el ID del Parto
    SELECT SCOPE_IDENTITY(); 
END
GO

CREATE PROCEDURE sp_ListarPartos
(
    @Estado BIT -- 1 para activos (habilitados), 0 para anulados (inhabilitados)
)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        pa.IdParto,
        pa.Fecha,
        pa.Estado,
        e.IdEmbarazo,
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        vp.Descripcion AS DescripcionViaParto
    FROM 
        Parto pa
    JOIN 
        Embarazo e ON pa.IdEmbarazo = e.IdEmbarazo
    JOIN 
        Paciente p ON e.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ViaParto vp ON pa.IdViaParto = vp.IdViaParto
    WHERE 
        pa.Estado = @Estado
    ORDER BY
        pa.Fecha DESC, p.Apellidos;
END
GO

CREATE  PROCEDURE sp_InsertarPartoIntervencion
    @IdParto INT,
    @Intervencion NVARCHAR(80)
AS
BEGIN
    INSERT INTO PartoIntervencion (IdParto, Intervencion)
    VALUES (@IdParto, @Intervencion);
END
GO

CREATE  PROCEDURE sp_AnularParto
    @IdParto INT
AS
BEGIN
    -- Anulación lógica (Estado = 0)
    UPDATE Parto
    SET Estado = 0
    WHERE IdParto = @IdParto;
END
GO

CREATE  PROCEDURE sp_BuscarPartoPorId
(
    @IdParto INT
)
AS
BEGIN
    SET NOCOUNT ON;
    
    
    SELECT 
        pa.*, 
        p.Nombres + ' ' + p.Apellidos AS NombrePaciente,
        pr.Nombres + ' ' + pr.Apellidos AS NombreProfesional,
        vp.Descripcion AS DescripcionViaParto,
        la.Descripcion AS DescripcionLiquido
    FROM 
        Parto pa
    JOIN 
        Embarazo e ON pa.IdEmbarazo = e.IdEmbarazo
    JOIN 
        Paciente p ON e.IdPaciente = p.IdPaciente
    LEFT JOIN 
        ProfesionalSalud pr ON pa.IdProfesional = pr.IdProfesional
    LEFT JOIN 
        ViaParto vp ON pa.IdViaParto = vp.IdViaParto
    LEFT JOIN 
        LiquidoAmniotico la ON pa.IdLiquido = la.IdLiquido
    WHERE 
        pa.IdParto = @IdParto;

    SELECT 
        IdPartoIntervencion,
        Intervencion
    FROM 
        PartoIntervencion
    WHERE 
        IdParto = @IdParto;

END
GO

CREATE  PROCEDURE sp_ListarViaParto
AS
BEGIN
    SELECT 
        IdViaParto,
        Codigo,
        Descripcion
    FROM 
        ViaParto
    WHERE 
        1 = 1;
END
GO

CREATE  PROCEDURE sp_ListarLiquidoAmniotico
AS
BEGIN
    SELECT 
        IdLiquido,
        Codigo,
        Descripcion
    FROM 
        LiquidoAmniotico
    WHERE 
        1 = 1;
END
GO

CREATE  PROCEDURE sp_ListarPacientesActivos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPaciente,
        Nombres,
        Apellidos,
        DNI 
    FROM 
        Paciente
    WHERE 
        Estado = 1 
    ORDER BY
        Apellidos, Nombres;
END
GO

CREATE  PROCEDURE sp_InsertarEncuentro
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

CREATE  PROCEDURE sp_ListarTipoEncuentro
AS
BEGIN
    SELECT IdTipoEncuentro, Codigo, Descripcion 
    FROM TipoEncuentro;
END
GO

CREATE  PROCEDURE sp_ListarTipoEncuentro
AS
BEGIN
    SELECT IdTipoEncuentro, Codigo, Descripcion 
    FROM TipoEncuentro;
END
GO

CREATE  PROCEDURE sp_ListarEncuentrosPorEmbarazoYTipo
(
    @IdEmbarazo INT,
    @CodigoTipo NVARCHAR(20) 
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        E.IdEncuentro,
        E.FechaHoraInicio,
        E.Estado,
        TE.Descripcion AS TipoDescripcion,
        P.Nombres + ' ' + P.Apellidos AS NombreProfesional
    FROM 
        Encuentro E
    JOIN 
        TipoEncuentro TE ON E.IdTipoEncuentro = TE.IdTipoEncuentro
    LEFT JOIN
        ProfesionalSalud P ON E.IdProfesional = P.IdProfesional
    WHERE 
        E.IdEmbarazo = @IdEmbarazo
        AND TE.Codigo = @CodigoTipo
        AND E.Estado <> 'Cerrado';
END
GO

CREATE  PROCEDURE sp_ListarPacientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdPaciente,
        IdUsuario,      
        Nombres,
        Apellidos,
        DNI,
        FechaNacimiento,
        Estado
    FROM 
        Paciente; 
END
GO

--------------------------------STORES PARA LA VALIDACION DE LOS USUARIOS-------------
-------------------------------                                           --------------------
                                      
CREATE  PROCEDURE sp_Usuario_ObtenerPorNombre
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





CREATE  PROCEDURE sp_Usuario_ObtenerRoles
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


------ SP PARA LOS ROLES -------------

CREATE  PROCEDURE sp_ListarRol
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


CREATE  PROCEDURE sp_InsertarRol
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(100) = NULL,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Rol (NombreRol, Descripcion, Estado)
    VALUES (@Nombre, @Descripcion, @Estado);
END;
GO

CREATE  PROCEDURE sp_EditarRol
    @IdRol INT,
    @Nombre NVARCHAR(50),
    @Descripcion NVARCHAR(100) = NULL,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Rol
    SET 
        NombreRol = @Nombre,
        Descripcion = @Descripcion,
        Estado = @Estado
    WHERE IdRol = @IdRol;
END;
GO


CREATE  PROCEDURE sp_BuscarRol
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


CREATE  PROCEDURE sp_Rol_ObtenerPorNombre
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






----------------------SP DE USUARIOS-----------------------

CREATE   PROCEDURE sp_ListarUsuario
AS
BEGIN 
    SET NOCOUNT ON;

    SELECT 
        u.IdUsuario,
        u.NombreUsuario,
        u.ClaveHash,
        u.Email,
        Ur.IdRol,
        r.NombreRol,      -- si quieres traer el nombre del rol
        u.Estado
    FROM Usuario u
    INNER JOIN UsuarioRol Ur ON u.IdUsuario = Ur.IdUsuario
	INNER JOIN Rol r on r.IdRol = Ur.IdRol
    ORDER BY u.IdUsuario;
END;
GO

CREATE  PROCEDURE sp_InsertarUsuario
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


CREATE  PROCEDURE sp_EliminarUsuario
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;

    -- desactivar usuario
    UPDATE Usuario
    SET Estado = 0
    WHERE IdUsuario = @IdUsuario;

    -- OPCIONAL: si no quieres que aparezca en listados por rol
    UPDATE UsuarioRol
	 SET IdRol = NULL -- o borrar
     WHERE IdUsuario = @IdUsuario;
END;
GO
---SP CITAS ------------

CREATE  PROCEDURE sp_ListarCitas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.IdCita,
        c.IdPaciente,
        c.IdRecepcionista,
        c.IdProfesional,
        c.IdEmbarazo,
        c.FechaCita,
        c.Motivo,
        c.IdEstadoCita,
        c.Observacion,
        c.FechaAnulacion,
        c.MotivoAnulacion
    FROM Cita c
    ORDER BY c.FechaCita DESC;
END;
GO



CREATE  PROCEDURE sp_InsertarCita
    @IdPaciente       INT,
    @IdRecepcionista  INT = NULL,
    @IdProfesional    INT = NULL,
    @IdEmbarazo       INT = NULL,
    @FechaCita        DATETIME,
    @Motivo           NVARCHAR(200) = NULL,
    @IdEstadoCita     SMALLINT,
    @Observacion      NVARCHAR(250) = NULL,
    @FechaAnulacion   DATETIME = NULL,
    @MotivoAnulacion  NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Cita (
        IdPaciente,
        IdRecepcionista,
        IdProfesional,
        IdEmbarazo,
        FechaCita,
        Motivo,
        IdEstadoCita,
        Observacion,
        FechaAnulacion,
        MotivoAnulacion
    )
    VALUES (
        @IdPaciente,
        @IdRecepcionista,
        @IdProfesional,
        @IdEmbarazo,
        @FechaCita,
        @Motivo,
        @IdEstadoCita,
        @Observacion,
        @FechaAnulacion,
        @MotivoAnulacion
    );
END;
GO


CREATE  PROCEDURE sp_BuscarCitaPorId
    @IdCita INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.IdCita,
        c.IdPaciente,
        c.IdRecepcionista,
        c.IdProfesional,
        c.IdEmbarazo,
        c.FechaCita,
        c.Motivo,
        c.IdEstadoCita,
        c.Observacion,
        c.FechaAnulacion,
        c.MotivoAnulacion
    FROM Cita c
    WHERE c.IdCita = @IdCita;
END;
GO


CREATE  PROCEDURE sp_AnularCita
    @IdCita INT,
    @MotivoAnulacion NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Cita
    SET 
        IdEstadoCita = 0, -- o el id correspondiente al estado "Anulado"
        FechaAnulacion = GETDATE(),
        MotivoAnulacion = @MotivoAnulacion
    WHERE IdCita = @IdCita;
END;
GO


--SEGUIMIENTO PUERPERIO-----
CREATE  PROCEDURE sp_ListarSeguimientoPuerperio
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdPuerperio,
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PA_Sistolica,
        PA_Diastolica,
        Temp_C,
        AlturaUterinaPP_cm,
        Loquios,
        Lactancia,
        SignosInfeccion,
        TamizajeDepresion,
        IdMetodoPF,
        Observaciones,
        Estado
    FROM SeguimientoPuerperio
    ORDER BY Fecha DESC, IdPuerperio DESC;
END;
GO



CREATE  PROCEDURE sp_InsertarSeguimientoPuerperio
    @IdEmbarazo          INT,
    @IdEncuentro         INT         = NULL,
    @IdProfesional       INT         = NULL,
    @Fecha               DATETIME,
    @PA_Sistolica        TINYINT     = NULL,
    @PA_Diastolica       TINYINT     = NULL,
    @Temp_C              DECIMAL(5,2) = NULL,
    @AlturaUterinaPP_cm  DECIMAL(5,2) = NULL,
    @Loquios             NVARCHAR(100) = NULL,
    @Lactancia           NVARCHAR(100) = NULL,
    @SignosInfeccion     BIT         = NULL,
    @TamizajeDepresion   NVARCHAR(100) = NULL,
    @IdMetodoPF          SMALLINT    = NULL,
    @Observaciones       NVARCHAR(500) = NULL,
    @Estado              BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SeguimientoPuerperio (
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PA_Sistolica,
        PA_Diastolica,
        Temp_C,
        AlturaUterinaPP_cm,
        Loquios,
        Lactancia,
        SignosInfeccion,
        TamizajeDepresion,
        IdMetodoPF,
        Observaciones,
        Estado
    )
    VALUES (
        @IdEmbarazo,
        @IdEncuentro,
        @IdProfesional,
        @Fecha,
        @PA_Sistolica,
        @PA_Diastolica,
        @Temp_C,
        @AlturaUterinaPP_cm,
        @Loquios,
        @Lactancia,
        @SignosInfeccion,
        @TamizajeDepresion,
        @IdMetodoPF,
        @Observaciones,
        @Estado
    );
END;
GO

CREATE  PROCEDURE sp_InhabilitarSeguimientoPuerperio
    @IdPuerperio INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SeguimientoPuerperio
    SET Estado = 0
    WHERE IdPuerperio = @IdPuerperio;
END;
GO


CREATE  PROCEDURE sp_BuscarSeguimientoPuerperio
    @IdSeguimiento INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        IdPuerperio,
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PA_Sistolica,
        PA_Diastolica,
        Temp_C,
        AlturaUterinaPP_cm,
        Loquios,
        Lactancia,
        SignosInfeccion,
        TamizajeDepresion,
        IdMetodoPF,
        Observaciones,
        Estado
    FROM SeguimientoPuerperio
    WHERE IdPuerperio = @IdSeguimiento;
END;
GO


--SP CONTROL PRENATAL --

CREATE  PROCEDURE sp_ListarControlPrenatal
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdControl,
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PesoKg,
        TallaM,
        PA_Sistolica,
        PA_Diastolica,
        AlturaUterina_cm,
        FCF_bpm,
        Presentacion,
        Proteinuria,
        MovFetales,
        Consejerias,
        Observaciones,
        Estado
    FROM ControlPrenatal
    ORDER BY Fecha DESC, IdControl DESC;
END;
GO

CREATE  PROCEDURE sp_InsertarControlPrenatal
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATETIME,
    @PesoKg DECIMAL(5,2) = NULL,
    @TallaM DECIMAL(5,2) = NULL,
    @PA_Sistolica TINYINT = NULL,
    @PA_Diastolica TINYINT = NULL,
    @AlturaUterina_cm DECIMAL(5,2) = NULL,
    @FCF_bpm TINYINT = NULL,
    @Presentacion NVARCHAR(100) = NULL,
    @Proteinuria NVARCHAR(100) = NULL,
    @MovFetales BIT = NULL,
    @Consejerias NVARCHAR(500) = NULL,
    @Observaciones NVARCHAR(500) = NULL,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO ControlPrenatal (
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PesoKg,
        TallaM,
        PA_Sistolica,
        PA_Diastolica,
        AlturaUterina_cm,
        FCF_bpm,
        Presentacion,
        Proteinuria,
        MovFetales,
        Consejerias,
        Observaciones,
        Estado
    )
    VALUES (
        @IdEmbarazo,
        @IdEncuentro,
        @IdProfesional,
        @Fecha,
        @PesoKg,
        @TallaM,
        @PA_Sistolica,
        @PA_Diastolica,
        @AlturaUterina_cm,
        @FCF_bpm,
        @Presentacion,
        @Proteinuria,
        @MovFetales,
        @Consejerias,
        @Observaciones,
        @Estado
    );
END;
GO

CREATE  PROCEDURE sp_BuscarControlPrenatal
    @IdControl INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdControl,
        IdEmbarazo,
        IdEncuentro,
        IdProfesional,
        Fecha,
        PesoKg,
        TallaM,
        PA_Sistolica,
        PA_Diastolica,
        AlturaUterina_cm,
        FCF_bpm,
        Presentacion,
        Proteinuria,
        MovFetales,
        Consejerias,
        Observaciones,
        Estado
    FROM ControlPrenatal
    WHERE IdControl = @IdControl;
END;
GO

CREATE  PROCEDURE sp_InhabilitarControlPrenatal
    @IdControl INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE ControlPrenatal
    SET Estado = 0
    WHERE IdControl = @IdControl;
END;
GO

CREATE  PROCEDURE sp_EliminarControlPrenatal
    @IdControl INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM ControlPrenatal
    WHERE IdControl = @IdControl;
END;
GO

/* ==================  STORED PROCEDURES PARA AYUDAS DIAGNÓSTICAS  ================== */

-- SP para listar ayudas diagnósticas
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

