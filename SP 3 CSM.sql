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

CREATE OR ALTER PROCEDURE sp_ListarProfesionalSalud
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

CREATE OR ALTER PROCEDURE sp_InsertarProfesionalSalud
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

CREATE OR ALTER PROCEDURE sp_BuscarProfesionalSalud
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

CREATE OR ALTER PROCEDURE sp_EditarProfesionalSalud
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

CREATE OR ALTER PROCEDURE sp_InsertarParto
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

CREATE OR ALTER PROCEDURE sp_ListarPartos
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

CREATE OR ALTER PROCEDURE sp_InsertarPartoIntervencion
    @IdParto INT,
    @Intervencion NVARCHAR(80)
AS
BEGIN
    INSERT INTO PartoIntervencion (IdParto, Intervencion)
    VALUES (@IdParto, @Intervencion);
END
GO

CREATE OR ALTER PROCEDURE sp_AnularParto
    @IdParto INT
AS
BEGIN
    -- Anulación lógica (Estado = 0)
    UPDATE Parto
    SET Estado = 0
    WHERE IdParto = @IdParto;
END
GO

CREATE OR ALTER PROCEDURE sp_BuscarPartoPorId
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

CREATE OR ALTER PROCEDURE sp_ListarViaParto
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

CREATE OR ALTER PROCEDURE sp_ListarLiquidoAmniotico
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

CREATE OR ALTER PROCEDURE sp_ListarPacientesActivos
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

CREATE OR ALTER PROCEDURE sp_ListarTipoEncuentro
AS
BEGIN
    SELECT IdTipoEncuentro, Codigo, Descripcion 
    FROM TipoEncuentro;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarTipoEncuentro
AS
BEGIN
    SELECT IdTipoEncuentro, Codigo, Descripcion 
    FROM TipoEncuentro;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarEncuentrosPorEmbarazoYTipo
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

CREATE OR ALTER PROCEDURE sp_ListarPacientes
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