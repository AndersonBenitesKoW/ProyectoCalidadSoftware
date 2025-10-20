/* =========================================================
   PROYECTO: Obstetricia (ProyectoCalidad)
   Versi�n con GO tras cada bloque
   ========================================================= */

CREATE DATABASE ProyectoCalidad;
GO
USE ProyectoCalidad;
GO



/* ==================  SEGURIDAD / USUARIOS  ================== */
CREATE TABLE Rol(
  IdRol INT IDENTITY(1,1) PRIMARY KEY,
  NombreRol NVARCHAR(50) NOT NULL UNIQUE,
  Descripcion NVARCHAR(50) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO


CREATE TABLE Usuario(
  IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
  NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
  ClaveHash NVARCHAR(50) NOT NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE UsuarioRol(
  IdUsuarioRol INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(IdUsuario),
  IdRol INT NOT NULL FOREIGN KEY REFERENCES Rol(IdRol),
  CONSTRAINT UQ_UsuarioRol UNIQUE(IdUsuario, IdRol)
);
GO

CREATE TABLE Auditoria(
  IdAuditoria INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NULL FOREIGN KEY REFERENCES Usuario(IdUsuario),
  Accion NVARCHAR(100) NOT NULL,
  Entidad NVARCHAR(100) NULL,
  IdRegistro INT NULL,
  Antes NVARCHAR(MAX) NULL,
  Despues NVARCHAR(MAX) NULL,
  IpCliente VARCHAR(64) NULL,
  FechaHora DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/* ================  CAT�LOGOS  ================== */
CREATE TABLE TipoEncuentro(
  IdTipoEncuentro SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Codigo NVARCHAR(20) NOT NULL UNIQUE,  
  Descripcion NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE EstadoCita(
  IdEstadoCita SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Codigo NVARCHAR(20) NOT NULL UNIQUE,  
  Descripcion NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE TipoAyudaDiagnostica(
  IdTipoAyuda SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Nombre NVARCHAR(80) NOT NULL UNIQUE
);
GO

CREATE TABLE ViaParto(
  IdViaParto SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Codigo NVARCHAR(20) NOT NULL UNIQUE,  
  Descripcion NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE LiquidoAmniotico(
  IdLiquido SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Codigo NVARCHAR(20) NOT NULL UNIQUE,  
  Descripcion NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE MetodoPF(
  IdMetodoPF SMALLINT IDENTITY(1,1) PRIMARY KEY,
  Nombre NVARCHAR(60) NOT NULL UNIQUE
);
GO

/* Semillas (cat�logos) */
INSERT INTO TipoEncuentro (Codigo,Descripcion)
VALUES (N'ANC',N'Atenci�n prenatal'),
       (N'INTRAPARTO',N'Trabajo de parto/Parto'),
       (N'PNC',N'Atenci�n posnatal/Puerperio');
GO

INSERT INTO EstadoCita (Codigo,Descripcion)
VALUES (N'Programada',N'Cita programada'),
       (N'Atendida',N'Cita atendida'),
       (N'NoAsistio',N'Paciente no asisti�'),
       (N'Anulada',N'Cita anulada');
GO

INSERT INTO ViaParto (Codigo,Descripcion)
VALUES (N'EUTOCICO',N'Vaginal eut�cico'),
       (N'INSTRUMENTAL',N'Vaginal instrumental'),
       (N'CESAREA',N'Ces�rea');
GO

INSERT INTO LiquidoAmniotico (Codigo,Descripcion)
VALUES (N'CLARO',N'Claro'),
       (N'MECONIAL',N'Meconial'),
       (N'HEMATICO',N'Hem�tico'),
       (N'ESCASO',N'Escaso');
GO

/* ================  PERSONAS  ================== */
CREATE TABLE Paciente(
  IdPaciente INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NULL UNIQUE FOREIGN KEY REFERENCES Usuario(IdUsuario),
  Nombres NVARCHAR(100) NOT NULL,
  Apellidos NVARCHAR(50) NOT NULL,
  DNI NVARCHAR(15) NULL UNIQUE,
  FechaNacimiento DATE NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

INSERT INTO Paciente (Nombres, Apellidos, DNI, Estado)
VALUES ('Paciente', 'De Prueba', '12345678', 1);

CREATE TABLE PacienteEmail(
  IdPacienteEmail INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  Email NVARCHAR(100) NOT NULL,
  EsPrincipal BIT NOT NULL DEFAULT 0,
  CONSTRAINT UQ_PacienteEmail UNIQUE(IdPaciente, Email)
);
GO

CREATE TABLE PacienteTelefono(
  IdPacienteTelefono INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  Telefono NVARCHAR(20) NOT NULL,
  Tipo NVARCHAR(20) NULL,
  EsPrincipal BIT NOT NULL DEFAULT 0,
  CONSTRAINT UQ_PacienteTelefono UNIQUE(IdPaciente, Telefono)
);
GO

CREATE TABLE ProfesionalSalud(
  IdProfesional INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NULL FOREIGN KEY REFERENCES Usuario(IdUsuario),
  CMP NVARCHAR(20) NULL,
  Especialidad NVARCHAR(80) NULL,
  Nombres NVARCHAR(100) NULL,
  Apellidos NVARCHAR(50) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

/* ================  EMBARAZO (EPISODIO) Y ENCUENTRO  ================= */
CREATE TABLE Embarazo(
  IdEmbarazo INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  FUR DATE NULL,
  FPP DATE NULL,
  Riesgo NVARCHAR(50) NULL,
  FechaApertura DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FechaCierre DATETIME2 NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Encuentro(
  IdEncuentro INT IDENTITY(1,1) PRIMARY KEY,
  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  IdTipoEncuentro SMALLINT NOT NULL FOREIGN KEY REFERENCES TipoEncuentro(IdTipoEncuentro),
  FechaHoraInicio DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  FechaHoraFin DATETIME2 NULL,
  Estado NVARCHAR(20) NOT NULL DEFAULT N'Cerrado',
  Notas NVARCHAR(500) NULL
);
GO
CREATE INDEX IX_Encuentro_Embarazo ON Encuentro(IdEmbarazo, IdTipoEncuentro);
GO

/* ================  CITAS  ================= */
CREATE TABLE Cita(
  IdCita INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  IdRecepcionista INT NULL FOREIGN KEY REFERENCES Usuario(IdUsuario),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  IdEmbarazo INT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  FechaCita DATETIME2 NOT NULL,
  Motivo NVARCHAR(150) NULL,
  IdEstadoCita SMALLINT NOT NULL FOREIGN KEY REFERENCES EstadoCita(IdEstadoCita) DEFAULT 1,
  Observacion NVARCHAR(300) NULL,
  FechaAnulacion DATETIME2 NULL,
  MotivoAnulacion NVARCHAR(200) NULL
);
GO
CREATE INDEX IX_Cita_PacienteFecha ON Cita(IdPaciente, FechaCita);
GO

/* ===========  ANTECEDENTES y FACTORES DE RIESGO  ============= */
CREATE TABLE AntecedenteObstetrico(
  IdAntecedente INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  Menarquia SMALLINT NULL,
  CicloDias SMALLINT NULL,
  Gestas SMALLINT NULL,
  Partos SMALLINT NULL,
  Abortos SMALLINT NULL,
  Observacion NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE FactorRiesgoCat(
  IdFactorCat INT IDENTITY(1,1) PRIMARY KEY,
  Nombre NVARCHAR(120) NOT NULL UNIQUE
);
GO

CREATE TABLE PacienteFactorRiesgo(
  IdPacienteFactor INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  IdFactorCat INT NOT NULL FOREIGN KEY REFERENCES FactorRiesgoCat(IdFactorCat),
  Detalle NVARCHAR(200) NULL,
  FechaRegistro DATE NOT NULL DEFAULT CONVERT(date, SYSUTCDATETIME()),
  Estado BIT NOT NULL DEFAULT 1,
  CONSTRAINT UQ_PacienteFactor UNIQUE(IdPaciente, IdFactorCat)
);
GO

/* ==================  CONTROL PRENATAL (ANC)  ================== */
CREATE TABLE ControlPrenatal(
  IdControl INT IDENTITY(1,1) PRIMARY KEY,
  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Fecha DATE NOT NULL,
  PesoKg DECIMAL(5,2) NULL CHECK (PesoKg >= 25 AND PesoKg <= 250),
  TallaM DECIMAL(3,2) NULL CHECK (TallaM BETWEEN 1.20 AND 2.20),
  PA_Sistolica TINYINT NULL CHECK (PA_Sistolica BETWEEN 50 AND 250),
  PA_Diastolica TINYINT NULL CHECK (PA_Diastolica BETWEEN 30 AND 180),
  AlturaUterina_cm DECIMAL(4,1) NULL,
  FCF_bpm TINYINT NULL CHECK (FCF_bpm BETWEEN 60 AND 220),
  Presentacion NVARCHAR(50) NULL,
  Proteinuria NVARCHAR(10) NULL,
  MovFetales BIT NULL,
  Consejerias NVARCHAR(200) NULL,
  Observaciones NVARCHAR(300) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO
CREATE INDEX IX_ControlPrenatal_EmbarazoFecha ON ControlPrenatal(IdEmbarazo, Fecha);
GO

SELECT * FROM Embarazo;

/* ==================  INTRAPARTO / PARTO  ================== */
CREATE TABLE Parto(
  IdParto INT IDENTITY(1,1) PRIMARY KEY,
  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Fecha DATE NOT NULL,
  HoraIngreso DATETIME2 NULL,
  HoraInicioTrabajo DATETIME2 NULL,
  Membranas NVARCHAR(10) NULL,  
  IdLiquido SMALLINT NULL FOREIGN KEY REFERENCES LiquidoAmniotico(IdLiquido),
  Analgesia NVARCHAR(50) NULL,
  IdViaParto SMALLINT NULL FOREIGN KEY REFERENCES ViaParto(IdViaParto),
  IndicacionCesarea NVARCHAR(150) NULL,
  PerdidasML INT NULL CHECK (PerdidasML IS NULL OR PerdidasML >= 0),
  Desgarro NVARCHAR(10) NULL,   
  Complicaciones NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO
CREATE INDEX IX_Parto_EmbarazoFecha ON Parto(IdEmbarazo, Fecha);
GO

CREATE TABLE PartoIntervencion(
  IdPartoIntervencion INT IDENTITY(1,1) PRIMARY KEY,
  IdParto INT NOT NULL FOREIGN KEY REFERENCES Parto(IdParto),
  Intervencion NVARCHAR(80) NOT NULL,
  CONSTRAINT UQ_PartoIntervencion UNIQUE(IdParto, Intervencion)
);
GO



CREATE TABLE Bebe(
  IdBebe INT IDENTITY(1,1) PRIMARY KEY,
  IdParto INT NOT NULL FOREIGN KEY REFERENCES Parto(IdParto),
  EstadoBebe NVARCHAR(50) NOT NULL,  
  Sexo CHAR(1) NULL CHECK (Sexo IN ('F','M')),
	  Apgar1 TINYINT NULL CHECK (Apgar1 BETWEEN 0 AND 10),
  Apgar5 TINYINT NULL CHECK (Apgar5 BETWEEN 0 AND 10),
  PesoGr INT NULL CHECK (PesoGr IS NULL OR PesoGr BETWEEN 300 AND 7000),
  TallaCm DECIMAL(4,1) NULL,
  PerimetroCefalico DECIMAL(4,1) NULL,
  EG_Semanas DECIMAL(4,1) NULL,
  Reanimacion BIT NULL,
  Observaciones NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

/* ==================  PUERPERIO (PNC)  ================== */
CREATE TABLE SeguimientoPuerperio(
  IdPuerperio INT IDENTITY(1,1) PRIMARY KEY,
  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Fecha DATE NOT NULL,
  PA_Sistolica TINYINT NULL CHECK (PA_Sistolica BETWEEN 50 AND 250),
  PA_Diastolica TINYINT NULL CHECK (PA_Diastolica BETWEEN 30 AND 180),
  Temp_C DECIMAL(4,1) NULL,
  AlturaUterinaPP_cm DECIMAL(4,1) NULL,
  Loquios NVARCHAR(20) NULL,
  Lactancia NVARCHAR(20) NULL,
  SignosInfeccion BIT NULL,
  TamizajeDepresion NVARCHAR(20) NULL,
  IdMetodoPF SMALLINT NULL FOREIGN KEY REFERENCES MetodoPF(IdMetodoPF),
  Observaciones NVARCHAR(300) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO
CREATE INDEX IX_Puerperio_EmbarazoFecha ON SeguimientoPuerperio(IdEmbarazo, Fecha);
GO

/* ============  AYUDAS DIAGN�STICAS (ORDEN + RESULTADO)  ============ */
CREATE TABLE AyudaDiagnosticaOrden(
  IdAyuda INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  IdEmbarazo INT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  IdTipoAyuda SMALLINT NULL FOREIGN KEY REFERENCES TipoAyudaDiagnostica(IdTipoAyuda),
  Descripcion NVARCHAR(200) NULL,
  Urgente BIT NOT NULL DEFAULT 0,
  FechaOrden DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Estado NVARCHAR(20) NOT NULL DEFAULT N'Solicitada' 
);
GO

CREATE TABLE ResultadoDiagnostico(
  IdResultado INT IDENTITY(1,1) PRIMARY KEY,
  IdAyuda INT NOT NULL FOREIGN KEY REFERENCES AyudaDiagnosticaOrden(IdAyuda),
  FechaResultado DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Resumen NVARCHAR(400) NULL,
  Critico BIT NOT NULL DEFAULT 0,
  Estado NVARCHAR(20) NOT NULL DEFAULT N'Validado'
);
GO

CREATE TABLE ResultadoItem(
  IdResultadoItem INT IDENTITY(1,1) PRIMARY KEY,
  IdResultado INT NOT NULL FOREIGN KEY REFERENCES ResultadoDiagnostico(IdResultado),
  Parametro NVARCHAR(100) NOT NULL,    
  ValorNumerico DECIMAL(12,4) NULL,
  ValorTexto NVARCHAR(200) NULL,
  Unidad NVARCHAR(40) NULL,
  RangoRef NVARCHAR(60) NULL
);
GO

/* ==================  VISTAS ================== */
CREATE VIEW vw_CitasProgramadas AS
SELECT c.IdCita,
       p.Nombres + ' ' + p.Apellidos AS Paciente,
       c.FechaCita,
       ec.Codigo AS Estado,
       pr.Nombres + ' ' + pr.Apellidos AS Profesional,
       c.Motivo
FROM Cita c
JOIN Paciente p ON p.IdPaciente = c.IdPaciente
JOIN EstadoCita ec ON ec.IdEstadoCita = c.IdEstadoCita
LEFT JOIN ProfesionalSalud pr ON pr.IdProfesional = c.IdProfesional;
GO

CREATE VIEW vw_ControlesPrenatales AS
SELECT cp.IdControl,
       em.IdEmbarazo,
       pa.Nombres + ' ' + pa.Apellidos AS Paciente,
       cp.Fecha,
       cp.PesoKg, cp.PA_Sistolica, cp.PA_Diastolica,
       cp.AlturaUterina_cm, cp.FCF_bpm
FROM ControlPrenatal cp
JOIN Embarazo em ON em.IdEmbarazo = cp.IdEmbarazo
JOIN Paciente pa ON pa.IdPaciente = em.IdPaciente;
GO

CREATE VIEW vw_AyudasPendientes AS
SELECT a.IdAyuda,
       p.Nombres + ' ' + p.Apellidos AS Paciente,
       ta.Nombre AS Tipo,
       a.Descripcion, a.Urgente, a.FechaOrden, a.Estado
FROM AyudaDiagnosticaOrden a
JOIN Paciente p ON p.IdPaciente = a.IdPaciente
LEFT JOIN TipoAyudaDiagnostica ta ON ta.IdTipoAyuda = a.IdTipoAyuda
WHERE a.Estado = 'Solicitada';
GO

CREATE VIEW vw_PartosRegistrados AS
SELECT prt.IdParto,
       em.IdEmbarazo,
       pa.Nombres + ' ' + pa.Apellidos AS Paciente,
       prt.Fecha,
       vp.Codigo AS Via,
       prt.PerdidasML,
       prt.Desgarro
FROM Parto prt
JOIN Embarazo em ON em.IdEmbarazo = prt.IdEmbarazo
JOIN Paciente pa ON pa.IdPaciente = em.IdPaciente
LEFT JOIN ViaParto vp ON vp.IdViaParto = prt.IdViaParto;
GO

CREATE VIEW vw_Puerperio AS
SELECT sp.IdPuerperio,
       em.IdEmbarazo,
       pa.Nombres + ' ' + pa.Apellidos AS Paciente,
       sp.Fecha, sp.Lactancia, sp.Loquios, sp.SignosInfeccion, sp.TamizajeDepresion
FROM SeguimientoPuerperio sp
JOIN Embarazo em ON em.IdEmbarazo = sp.IdEmbarazo
JOIN Paciente pa ON pa.IdPaciente = em.IdPaciente;
GO

/* ==================  �NDICES EXTRA ================== */
CREATE INDEX IX_Paciente_DNI ON Paciente(DNI) WHERE DNI IS NOT NULL;
GO
CREATE INDEX IX_Ayuda_PacienteFecha ON AyudaDiagnosticaOrden(IdPaciente, FechaOrden);
GO
CREATE INDEX IX_Resultado_Ayuda ON ResultadoDiagnostico(IdAyuda);
GO

PRINT 'Esquema creado correctamente (Paciente relacionado con Usuario).';
GO

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

CREATE OR ALTER PROCEDURE sp_ListarPacientes
AS
BEGIN
  SELECT IdPaciente, Nombres, Apellidos, DNI, FechaNacimiento, Estado
  FROM Paciente
  WHERE Estado = 1;
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

--ProfesionalSalud

select * from ProfesionalSalud


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
    -- NOTA: Si también quieres editar el IdUsuario, deberás agregarlo aquí
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

--Embarazo
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
SELECT * FROM Parto
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

