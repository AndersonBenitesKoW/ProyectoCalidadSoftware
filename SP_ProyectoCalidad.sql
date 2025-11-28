use ProyectoCalidad; 
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

CREATE OR ALTER PROCEDURE sp_ListarProfesionales
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

CREATE OR ALTER PROCEDURE sp_InsertarProfesional
  @IdUsuario INT,
  @CMP NVARCHAR(20),
  @Especialidad NVARCHAR(80),
  @Nombres NVARCHAR(100),
  @Apellidos NVARCHAR(100)
AS
BEGIN
  INSERT INTO ProfesionalSalud (IdUsuario, CMP, Especialidad, Nombres, Apellidos)
  VALUES (@IdUsuario, @CMP, @Especialidad, @Nombres, @Apellidos);

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
    @IdEmbarazo                    INT,
    @IdEncuentro                   INT             = NULL,
    @IdProfesional                 INT             = NULL,
    @Fecha                         DATE,
    @HoraIngreso                   DATETIME2       = NULL,
    @HoraInicioTrabajo             DATETIME2       = NULL,
    @HoraExpulsion                 DATETIME2       = NULL,
    @TipoParto                     NVARCHAR(50)    = NULL,
    @IdViaParto                    SMALLINT        = NULL,
    @IndicacionCesarea             NVARCHAR(150)   = NULL,

    @Membranas                     NVARCHAR(10)    = NULL,
    @TiempoRoturaMembranasHoras    INT             = NULL,
    @IdLiquido                     SMALLINT        = NULL,
    @AspectoLiquido                NVARCHAR(50)    = NULL,

    @Analgesia                     NVARCHAR(50)    = NULL,
    @PosicionMadre                 NVARCHAR(50)    = NULL,
    @Acompanante                   BIT             = NULL,
    @LugarNacimiento               NVARCHAR(100)   = NULL,

    @DuracionSegundaEtapaMinutos   INT             = NULL,
    @PerdidasML                    INT             = NULL,
    @Desgarro                      NVARCHAR(10)    = NULL,
    @Episiotomia                   BIT             = NULL,

    @ComplicacionesMaternas        NVARCHAR(300)   = NULL,
    @Derivacion                    BIT             = NULL,
    @SeguroTipo                    NVARCHAR(50)    = NULL,

    @NumeroHijosPrevios            INT             = NULL,
    @NumeroCesareasPrevias         INT             = NULL,
    @EmbarazoMultiple              BIT             = NULL,
    @NumeroGemelos                 INT             = NULL,

    @Observaciones                 NVARCHAR(500)   = NULL,
    @Estado                        BIT
AS
BEGIN
    SET NOCOUNT ON;

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

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS IdParto;
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
    
    -- 1. Obtener los datos principales del Parto y tablas relacionadas
    SELECT 
        pa.*, -- Todos los campos de Parto
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

    -- 2. Obtener las intervenciones asociadas a ese Parto
    SELECT 
        IdPartoIntervencion,
        Intervencion
    FROM 
        PartoIntervencion
    WHERE 
        IdParto = @IdParto;

END
GO

CREATE OR ALTER PROCEDURE sp_EditarParto
    @IdParto INT,
    @IdEmbarazo INT,
    @IdEncuentro INT = NULL,
    @IdProfesional INT = NULL,
    @Fecha DATE,
    @HoraIngreso DATETIME2 = NULL,
    @HoraInicioTrabajo DATETIME2 = NULL,
    @HoraExpulsion DATETIME2 = NULL,
    @TipoParto NVARCHAR(50) = NULL,
    @IdViaParto SMALLINT = NULL,
    @IndicacionCesarea NVARCHAR(150) = NULL,

    @Membranas NVARCHAR(10) = NULL,
    @TiempoRoturaMembranasHoras INT = NULL,
    @IdLiquido SMALLINT = NULL,
    @AspectoLiquido NVARCHAR(50) = NULL,

    @Analgesia NVARCHAR(50) = NULL,
    @PosicionMadre NVARCHAR(50) = NULL,
    @Acompanante BIT = NULL,
    @LugarNacimiento NVARCHAR(100) = NULL,

    @DuracionSegundaEtapaMinutos INT = NULL,
    @PerdidasML INT = NULL,
    @Desgarro NVARCHAR(10) = NULL,
    @Episiotomia BIT = NULL,

    @ComplicacionesMaternas NVARCHAR(300) = NULL,
    @Derivacion BIT = NULL,
    @SeguroTipo NVARCHAR(50) = NULL,

    @NumeroHijosPrevios INT = NULL,
    @NumeroCesareasPrevias INT = NULL,
    @EmbarazoMultiple BIT = NULL,
    @NumeroGemelos INT = NULL,

    @Observaciones NVARCHAR(500) = NULL,
    @Estado BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Parto
    SET
        IdEmbarazo = @IdEmbarazo,
        IdEncuentro = @IdEncuentro,
        IdProfesional = @IdProfesional,
        Fecha = @Fecha,
        HoraIngreso = @HoraIngreso,
        HoraInicioTrabajo = @HoraInicioTrabajo,
        HoraExpulsion = @HoraExpulsion,
        TipoParto = @TipoParto,
        IdViaParto = @IdViaParto,
        IndicacionCesarea = @IndicacionCesarea,

        Membranas = @Membranas,
        TiempoRoturaMembranasHoras = @TiempoRoturaMembranasHoras,
        IdLiquido = @IdLiquido,
        AspectoLiquido = @AspectoLiquido,

        Analgesia = @Analgesia,
        PosicionMadre = @PosicionMadre,
        Acompanante = @Acompanante,
        LugarNacimiento = @LugarNacimiento,

        DuracionSegundaEtapaMinutos = @DuracionSegundaEtapaMinutos,
        PerdidasML = @PerdidasML,
        Desgarro = @Desgarro,
        Episiotomia = @Episiotomia,

        ComplicacionesMaternas = @ComplicacionesMaternas,
        Derivacion = @Derivacion,
        SeguroTipo = @SeguroTipo,

        NumeroHijosPrevios = @NumeroHijosPrevios,
        NumeroCesareasPrevias = @NumeroCesareasPrevias,
        EmbarazoMultiple = @EmbarazoMultiple,
        NumeroGemelos = @NumeroGemelos,

        Observaciones = @Observaciones,
        Estado = @Estado
    WHERE IdParto = @IdParto;
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
        -- Asumimos que también podrían tener un estado, aunque no lo veo en tu script
        -- Si hubiera un campo "Estado", agregaríamos: WHERE Estado = 1
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
        DNI -- Incluimos DNI por si ayuda a diferenciar
    FROM 
        Paciente
    WHERE 
        Estado = 1 -- Solo pacientes activos
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
    
    -- Devolvemos el ID del Encuentro recién creado
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
    @CodigoTipo NVARCHAR(20) -- Ej: 'INTRAPARTO'
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
        AND E.Estado <> 'Cerrado'; -- O la lógica que prefieras
END
GO


-- CORE CONTROL PRENATAL--

CREATE OR ALTER PROCEDURE sp_ListarControlPrenatal
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        cp.IdControl,
        cp.IdEmbarazo,
        cp.IdEncuentro,
        cp.IdProfesional,
        cp.Fecha,
        cp.PesoKg,
        cp.TallaM,
        cp.PA_Sistolica,
        cp.PA_Diastolica,
        cp.AlturaUterina_cm,
        cp.FCF_bpm,
        cp.Presentacion,
        cp.Proteinuria,
        cp.MovFetales,
        cp.Consejerias,
        cp.Observaciones,
        cp.Estado,
        -- Datos adicionales útiles
        p.Nombres + ' ' + p.Apellidos AS Paciente,
        pr.Nombres + ' ' + pr.Apellidos AS Profesional
    FROM ControlPrenatal cp
    JOIN Embarazo e ON e.IdEmbarazo = cp.IdEmbarazo
    JOIN Paciente p ON p.IdPaciente = e.IdPaciente
    LEFT JOIN ProfesionalSalud pr ON pr.IdProfesional = cp.IdProfesional
    ORDER BY cp.Fecha DESC;
END
GO



CREATE OR ALTER PROCEDURE sp_InsertarControlPrenatal
(
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
    @Estado BIT = 1
)
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

    SELECT SCOPE_IDENTITY() AS IdControlPrenatal; -- devuelve el nuevo ID
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

