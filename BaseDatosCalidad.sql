/* =========================================================
   PROYECTO: Obstetricia (ProyectoCalidad)
   Versión con GO tras cada bloque
   ========================================================= */

CREATE DATABASE ProyectoCalidad;
GO
USE ProyectoCalidad;
GO



/* ==================  SEGURIDAD / USUARIOS  ================== */
CREATE TABLE Rol(
  IdRol INT IDENTITY(1,1) PRIMARY KEY,
  NombreRol NVARCHAR(50) NOT NULL UNIQUE,
  Descripcion NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Usuario(
  IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
  NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
  ClaveHash NVARCHAR(200) NOT NULL,
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

/* ================  CATÁLOGOS  ================== */
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

/* Semillas (catálogos) */
INSERT INTO TipoEncuentro (Codigo,Descripcion)
VALUES (N'ANC',N'Atención prenatal'),
       (N'INTRAPARTO',N'Trabajo de parto/Parto'),
       (N'PNC',N'Atención posnatal/Puerperio');
GO

INSERT INTO EstadoCita (Codigo,Descripcion)
VALUES (N'Programada',N'Cita programada'),
       (N'Atendida',N'Cita atendida'),
       (N'NoAsistio',N'Paciente no asistió'),
       (N'Anulada',N'Cita anulada');
GO

INSERT INTO ViaParto (Codigo,Descripcion)
VALUES (N'EUTOCICO',N'Vaginal eutócico'),
       (N'INSTRUMENTAL',N'Vaginal instrumental'),
       (N'CESAREA',N'Cesárea');
GO

INSERT INTO LiquidoAmniotico (Codigo,Descripcion)
VALUES (N'CLARO',N'Claro'),
       (N'MECONIAL',N'Meconial'),
       (N'HEMATICO',N'Hemático'),
       (N'ESCASO',N'Escaso');
GO

/* ================  PERSONAS  ================== */
CREATE TABLE Paciente(
  IdPaciente INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NULL UNIQUE FOREIGN KEY REFERENCES Usuario(IdUsuario),
  Nombres NVARCHAR(100) NOT NULL,
  Apellidos NVARCHAR(100) NOT NULL,
  DNI NVARCHAR(15) NULL UNIQUE,
  FechaNacimiento DATE NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE PacienteEmail(
  IdPacienteEmail INT IDENTITY(1,1) PRIMARY KEY,
  IdPaciente INT NOT NULL FOREIGN KEY REFERENCES Paciente(IdPaciente),
  Email NVARCHAR(150) NOT NULL,
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
  Apellidos NVARCHAR(100) NULL,
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
  Motivo NVARCHAR(200) NULL,
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

/* ============  AYUDAS DIAGNÓSTICAS (ORDEN + RESULTADO)  ============ */
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

/* ==================  ÍNDICES EXTRA ================== */
CREATE INDEX IX_Paciente_DNI ON Paciente(DNI) WHERE DNI IS NOT NULL;
GO
CREATE INDEX IX_Ayuda_PacienteFecha ON AyudaDiagnosticaOrden(IdPaciente, FechaOrden);
GO
CREATE INDEX IX_Resultado_Ayuda ON ResultadoDiagnostico(IdAyuda);
GO

PRINT 'Esquema creado correctamente (Paciente relacionado con Usuario).';
GO
