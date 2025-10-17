use ProyectoCalidad;
--Paciente
CREATE PROCEDURE sp_InsertarPaciente
  @IdUsuario INT,
  @Nombres NVARCHAR(100),
  @Apellidos NVARCHAR(100),
  @DNI NVARCHAR(15),
  @FechaNacimiento DATE
AS
BEGIN
  INSERT INTO Paciente (IdUsuario, Nombres, Apellidos, DNI, FechaNacimiento)
  VALUES (@IdUsuario, @Nombres, @Apellidos, @DNI, @FechaNacimiento);

  SELECT SCOPE_IDENTITY() AS IdPaciente;
END
GO

CREATE PROCEDURE sp_ListarPacientes
AS
BEGIN
  SELECT IdPaciente, Nombres, Apellidos, DNI, FechaNacimiento, Estado
  FROM Paciente
  WHERE Estado = 1;
END
GO

--ProfesionalSalud
CREATE PROCEDURE sp_InsertarProfesional
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

CREATE PROCEDURE sp_ListarProfesionales
AS
BEGIN
  SELECT IdProfesional, CMP, Especialidad, Nombres, Apellidos, Estado
  FROM ProfesionalSalud
  WHERE Estado = 1;
END
GO

--Embarazo
CREATE PROCEDURE sp_InsertarEmbarazo
  @IdPaciente INT,
  @FUR DATE,
  @FPP DATE,
  @Riesgo NVARCHAR(50)
AS
BEGIN
  INSERT INTO Embarazo (IdPaciente, FUR, FPP, Riesgo)
  VALUES (@IdPaciente, @FUR, @FPP, @Riesgo);

  SELECT SCOPE_IDENTITY() AS IdEmbarazo;
END
GO

CREATE PROCEDURE sp_ListarEmbarazos
AS
BEGIN
  SELECT e.IdEmbarazo, e.IdPaciente, e.FUR, e.FPP, e.Riesgo, e.Estado
  FROM Embarazo e
  WHERE e.Estado = 1;
END
GO

--Cita
CREATE PROCEDURE sp_InsertarCita
  @IdPaciente INT,
  @IdRecepcionista INT,
  @IdProfesional INT,
  @IdEmbarazo INT,
  @FechaCita DATETIME2,
  @Motivo NVARCHAR(200),
  @IdEstadoCita SMALLINT,
  @Observacion NVARCHAR(300)
AS
BEGIN
  INSERT INTO Cita (IdPaciente, IdRecepcionista, IdProfesional, IdEmbarazo, FechaCita, Motivo, IdEstadoCita, Observacion)
  VALUES (@IdPaciente, @IdRecepcionista, @IdProfesional, @IdEmbarazo, @FechaCita, @Motivo, @IdEstadoCita, @Observacion);

  SELECT SCOPE_IDENTITY() AS IdCita;
END
GO

CREATE PROCEDURE sp_ListarCitas
AS
BEGIN
  SELECT * FROM vw_CitasProgramadas;
END
GO

--ControlPrenatal
CREATE PROCEDURE sp_InsertarControlPrenatal
  @IdEmbarazo INT,
  @IdEncuentro INT,
  @IdProfesional INT,
  @Fecha DATE,
  @PesoKg DECIMAL(5,2),
  @TallaM DECIMAL(3,2),
  @PA_Sistolica TINYINT,
  @PA_Diastolica TINYINT,
  @AlturaUterina_cm DECIMAL(4,1),
  @FCF_bpm TINYINT,
  @Presentacion NVARCHAR(50),
  @Proteinuria NVARCHAR(10),
  @MovFetales BIT,
  @Consejerias NVARCHAR(200),
  @Observaciones NVARCHAR(300)
AS
BEGIN
  INSERT INTO ControlPrenatal (
    IdEmbarazo, IdEncuentro, IdProfesional, Fecha, PesoKg, TallaM,
    PA_Sistolica, PA_Diastolica, AlturaUterina_cm, FCF_bpm,
    Presentacion, Proteinuria, MovFetales, Consejerias, Observaciones
  )
  VALUES (
    @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @PesoKg, @TallaM,
    @PA_Sistolica, @PA_Diastolica, @AlturaUterina_cm, @FCF_bpm,
    @Presentacion, @Proteinuria, @MovFetales, @Consejerias, @Observaciones
  );

  SELECT SCOPE_IDENTITY() AS IdControl;
END
GO

CREATE PROCEDURE sp_ListarControlesPrenatales
AS
BEGIN
  SELECT * FROM vw_ControlesPrenatales;
END
GO


CREATE PROCEDURE sp_InhabilitarControlPrenatal
    @IdControl INT
AS
BEGIN
    UPDATE ControlPrenatal
    SET Estado = 0
    WHERE IdControl = @IdControl;
END
GO




--AntecedenteObstetrico
CREATE PROCEDURE sp_InsertarAntecedenteObstetrico
  @IdPaciente INT,
  @Menarquia SMALLINT,
  @CicloDias SMALLINT,
  @Gestas SMALLINT,
  @Partos SMALLINT,
  @Abortos SMALLINT,
  @Observacion NVARCHAR(200)
AS
BEGIN
  INSERT INTO AntecedenteObstetrico (
    IdPaciente, Menarquia, CicloDias, Gestas, Partos, Abortos, Observacion
  )
  VALUES (
    @IdPaciente, @Menarquia, @CicloDias, @Gestas, @Partos, @Abortos, @Observacion
  );

  SELECT SCOPE_IDENTITY() AS IdAntecedente;
END
GO

CREATE PROCEDURE sp_ListarAntecedentesObstetricos
AS
BEGIN
  SELECT * FROM AntecedenteObstetrico WHERE Estado = 1;
END
GO

--FactorRiesgoCat
CREATE PROCEDURE sp_InsertarFactorRiesgoCat
  @Nombre NVARCHAR(120)
AS
BEGIN
  INSERT INTO FactorRiesgoCat (Nombre)
  VALUES (@Nombre);

  SELECT SCOPE_IDENTITY() AS IdFactorCat;
END
GO

CREATE PROCEDURE sp_ListarFactoresRiesgoCat
AS
BEGIN
  SELECT * FROM FactorRiesgoCat;
END
GO

--PacienteFactorRiesgo
CREATE PROCEDURE sp_InsertarPacienteFactorRiesgo
  @IdPaciente INT,
  @IdFactorCat INT,
  @Detalle NVARCHAR(200)
AS
BEGIN
  INSERT INTO PacienteFactorRiesgo (IdPaciente, IdFactorCat, Detalle)
  VALUES (@IdPaciente, @IdFactorCat, @Detalle);

  SELECT SCOPE_IDENTITY() AS IdPacienteFactor;
END
GO

CREATE PROCEDURE sp_ListarPacienteFactoresRiesgo
AS
BEGIN
  SELECT pf.IdPacienteFactor, pf.IdPaciente, pf.IdFactorCat, f.Nombre AS Factor, pf.Detalle, pf.FechaRegistro
  FROM PacienteFactorRiesgo pf
  JOIN FactorRiesgoCat f ON f.IdFactorCat = pf.IdFactorCat
  WHERE pf.Estado = 1;
END
GO

--Parto
CREATE PROCEDURE sp_InsertarParto
  @IdEmbarazo INT,
  @IdEncuentro INT,
  @IdProfesional INT,
  @Fecha DATE,
  @HoraIngreso DATETIME2,
  @HoraInicioTrabajo DATETIME2,
  @Membranas NVARCHAR(10),
  @IdLiquido SMALLINT,
  @Analgesia NVARCHAR(50),
  @IdViaParto SMALLINT,
  @IndicacionCesarea NVARCHAR(150),
  @PerdidasML INT,
  @Desgarro NVARCHAR(10),
  @Complicaciones NVARCHAR(200)
AS
BEGIN
  INSERT INTO Parto (
    IdEmbarazo, IdEncuentro, IdProfesional, Fecha, HoraIngreso, HoraInicioTrabajo,
    Membranas, IdLiquido, Analgesia, IdViaParto, IndicacionCesarea,
    PerdidasML, Desgarro, Complicaciones
  )
  VALUES (
    @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @HoraIngreso, @HoraInicioTrabajo,
    @Membranas, @IdLiquido, @Analgesia, @IdViaParto, @IndicacionCesarea,
    @PerdidasML, @Desgarro, @Complicaciones
  );

  SELECT SCOPE_IDENTITY() AS IdParto;
END
GO

CREATE PROCEDURE sp_ListarPartos
AS
BEGIN
  SELECT * FROM vw_PartosRegistrados;
END
GO

--SeguimientoPuerperio
CREATE PROCEDURE sp_InsertarSeguimientoPuerperio
  @IdEmbarazo INT,
  @IdEncuentro INT,
  @IdProfesional INT,
  @Fecha DATE,
  @PA_Sistolica TINYINT,
  @PA_Diastolica TINYINT,
  @Temp_C DECIMAL(4,1),
  @AlturaUterinaPP_cm DECIMAL(4,1),
  @Loquios NVARCHAR(20),
  @Lactancia NVARCHAR(20),
  @SignosInfeccion BIT,
  @TamizajeDepresion NVARCHAR(20),
  @IdMetodoPF SMALLINT,
  @Observaciones NVARCHAR(300)
AS
BEGIN
  INSERT INTO SeguimientoPuerperio (
    IdEmbarazo, IdEncuentro, IdProfesional, Fecha, PA_Sistolica, PA_Diastolica,
    Temp_C, AlturaUterinaPP_cm, Loquios, Lactancia, SignosInfeccion,
    TamizajeDepresion, IdMetodoPF, Observaciones
  )
  VALUES (
    @IdEmbarazo, @IdEncuentro, @IdProfesional, @Fecha, @PA_Sistolica, @PA_Diastolica,
    @Temp_C, @AlturaUterinaPP_cm, @Loquios, @Lactancia, @SignosInfeccion,
    @TamizajeDepresion, @IdMetodoPF, @Observaciones
  );

  SELECT SCOPE_IDENTITY() AS IdPuerperio;
END
GO

CREATE PROCEDURE sp_ListarSeguimientoPuerperio
AS
BEGIN
  SELECT * FROM vw_Puerperio;
END
GO

CREATE PROCEDURE sp_InhabilitarSeguimientoPuerperio
    @IdPuerperio INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE SeguimientoPuerperio
    SET Estado = 0
    WHERE IdPuerperio = @IdPuerperio;
END
GO



--AyudaDiagnosticaOrden
CREATE PROCEDURE sp_InsertarAyudaDiagnosticaOrden
  @IdPaciente INT,
  @IdEmbarazo INT,
  @IdProfesional INT,
  @IdTipoAyuda SMALLINT,
  @Descripcion NVARCHAR(200),
  @Urgente BIT
AS
BEGIN
  INSERT INTO AyudaDiagnosticaOrden (
    IdPaciente, IdEmbarazo, IdProfesional, IdTipoAyuda, Descripcion, Urgente
  )
  VALUES (
    @IdPaciente, @IdEmbarazo, @IdProfesional, @IdTipoAyuda, @Descripcion, @Urgente
  );

  SELECT SCOPE_IDENTITY() AS IdAyuda;
END
GO

CREATE PROCEDURE sp_ListarAyudasDiagnosticasPendientes
AS
BEGIN
  SELECT * FROM vw_AyudasPendientes;
END
GO


CREATE PROCEDURE sp_InhabilitarAyudaDiagnosticaOrden
    @IdAyuda INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE AyudaDiagnosticaOrden
    SET Estado = 'INACTIVO'
    WHERE IdAyuda = @IdAyuda;
END
GO

--ResultadoDiagnostico
CREATE PROCEDURE sp_InsertarResultadoDiagnostico
  @IdAyuda INT,
  @Resumen NVARCHAR(400),
  @Critico BIT
AS
BEGIN
  INSERT INTO ResultadoDiagnostico (
    IdAyuda, Resumen, Critico
  )
  VALUES (
    @IdAyuda, @Resumen, @Critico
  );

  SELECT SCOPE_IDENTITY() AS IdResultado;
END
GO

CREATE PROCEDURE sp_ListarResultadosDiagnosticos
AS
BEGIN
  SELECT * FROM ResultadoDiagnostico;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarBebe
    @IdParto     INT = NULL,   -- opcional: filtra por parto
    @SoloActivos BIT = 1       -- 1 = solo Estado=1; 0 = todos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        b.IdBebe,
        b.IdParto,
        b.EstadoBebe,
        b.Sexo,
        b.Apgar1,
        b.Apgar5,
        b.PesoGr,
        b.TallaCm,
        b.PerimetroCefalico,
        b.EG_Semanas,
        b.Reanimacion,
        b.Observaciones,
        b.Estado
    FROM dbo.Bebe AS b
    WHERE
        (@IdParto IS NULL OR b.IdParto = @IdParto)
        AND (@SoloActivos = 0 OR b.Estado = 1)
    ORDER BY b.IdBebe DESC;
END
GO
