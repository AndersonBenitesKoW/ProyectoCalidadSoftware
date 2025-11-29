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
  ClaveHash NVARCHAR(500) NOT NULL,
  email NVARCHAR(100) NOT NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

/* DECLARE @IdUsuario INT;

INSERT INTO Usuario (NombreUsuario, ClaveHash, email, Estado) VALUES ('ADMIN_anderson', '3a6eb0790f39ac87c94f3856b2dd2c5d110e6811602261a9a923d3bb23adc8b7', 'admin123@gmail.com', 1);

SET @IdUsuario = SCOPE_IDENTITY();

INSERT INTO UsuarioRol (IdUsuario, IdRol) VALUES (@IdUsuario, 1);

GO

select * from Usuario;

 */


CREATE TABLE UsuarioRol(
  IdUsuarioRol INT IDENTITY(1,1) PRIMARY KEY,
  IdUsuario INT NOT NULL FOREIGN KEY REFERENCES Usuario(IdUsuario),
  IdRol INT NOT NULL FOREIGN KEY REFERENCES Rol(IdRol),
  CONSTRAINT UQ_UsuarioRol UNIQUE(IdUsuario, IdRol)
);
GO


INSERT INTO Rol (NombreRol, Descripcion) VALUES
('ADMIN', 'Administrador del sistema'),
('PERSONAL_SALUD', 'Médicos y obstetras'),
('SECRETARIA', 'Recepción y citas'),
('PACIENTE', 'Portal de pacientes'),
('LABORATORIO', 'Encargado de laboratorio');  
GO

--////////////////////////////////////////////////////////////////////////////////////
--//////////////////////////////////////////////////////////////////////////////////////
--USUARIO PARA USTED INGENIERO CHIRINOS-- SU INGRESO SERA COMO ADMINISTRADOR-- 
--USURIO: ADMIN_CHIRINOS
--PASSWORD:CHIRINOS123
--EMAIL:chirinos@gmail.com

--INSERT INTO Usuario (NombreUsuario, ClaveHash, email)
--VALUES (
--    'ADMIN_CHIRINOS',
--    '9d5630276cbb713c87c86e13b949df0e904519b0c9135ed6e8b95e98a6a61e6b',
--    'chirinos@gmail.com'
--);
-- select * from usuario

----------------
--//////////////////////////////////////



--DBCC CHECKIDENT ('Rol', RESEED, 4);

--ALTER TABLE ROL 
--DROP COLUMN LABORATORIO
--SELECT NOMBREROL FROM Rol WHERE idrol=5
--DELETE FROM Rol
--WHERE IdRol = 6;


-- Usuario 1: admin →ADMIN_anderson

--contraseña: anderson


--INSERT INTO UsuarioRol (IdUsuario, IdRol)
--VALUES (1, (SELECT IdRol FROM Rol WHERE NombreRol='ADMIN'));
--GO
--UPDATE Usuario
--SET ClaveHash = 'c1d35cc3471fe509203d65b3e7a53fc82337ac9ae2c797b836621f706fe37df8'
--WHERE NombreUsuario = 'ADMIN_anderson';





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
select*	from paciente
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

select * from Paciente



INSERT INTO EstadoCita (Codigo,Descripcion)
VALUES (N'Programada',N'Cita programada'),
       (N'Atendida',N'Cita atendida'),
       (N'NoAsistio',N'Paciente no asisti�'),
       (N'Anulada',N'Cita anulada');
GO
select * from Usuario
select * from Paciente
select * from cita 


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

CREATE TABLE ProfesionalEmail(
  IdProfesionalEmail INT IDENTITY(1,1) PRIMARY KEY,
  IdProfesional INT NOT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Email NVARCHAR(100) NOT NULL,
  EsPrincipal BIT NOT NULL DEFAULT 0,
  CONSTRAINT UQ_ProfesionalEmail UNIQUE(IdProfesional, Email)
);

CREATE TABLE ProfesionalTelefono(
  IdProfesionalTelefono INT IDENTITY(1,1) PRIMARY KEY,
  IdProfesional INT NOT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Telefono NVARCHAR(20) NOT NULL,
  Tipo NVARCHAR(20) NULL,
  EsPrincipal BIT NOT NULL DEFAULT 0,
  CONSTRAINT UQ_ProfesionalTelefono UNIQUE(IdProfesional, Telefono)
);



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

select *from Usuario


CREATE TABLE ControlPrenatal (
  IdControl INT IDENTITY(1,1) PRIMARY KEY,
  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),
  Fecha DATE NOT NULL,
  NumeroControl INT NULL,
  EdadGestSemanas INT NULL,
  EdadGestDias INT NULL,
  MetodoEdadGest VARCHAR(50) NULL,
  PesoKg DECIMAL(5,2) NULL CHECK (PesoKg >= 25 AND PesoKg <= 250),
  PesoPreGestacionalKg DECIMAL(5,2) NULL,
  TallaM DECIMAL(3,2) NULL CHECK (TallaM BETWEEN 1.20 AND 2.20),
  IMCPreGestacional DECIMAL(5,2) NULL,
  PA_Sistolica TINYINT NULL CHECK (PA_Sistolica BETWEEN 50 AND 250),
  PA_Diastolica TINYINT NULL CHECK (PA_Diastolica BETWEEN 30 AND 180),
  Pulso SMALLINT NULL,
  FrecuenciaRespiratoria SMALLINT NULL,
  Temperatura DECIMAL(4,1) NULL,
  AlturaUterina_cm DECIMAL(5,1) NULL,
  DinamicaUterina VARCHAR(10) NULL,
  Presentacion NVARCHAR(50) NULL,
  TipoEmbarazo VARCHAR(20) NULL,
  FCF_bpm TINYINT NULL CHECK (FCF_bpm BETWEEN 60 AND 220),
  LiquidoAmniotico VARCHAR(20) NULL,
  IndiceLiquidoAmniotico DECIMAL(4,1) NULL,
  PerfilBiofisico VARCHAR(10) NULL,
  Proteinuria VARCHAR(10) NULL,
  Edemas VARCHAR(10) NULL,
  Reflejos VARCHAR(10) NULL,
  Hemoglobina DECIMAL(4,1) NULL,
  ResultadoVIH VARCHAR(20) NULL,
  ResultadoSifilis VARCHAR(20) NULL,
  GrupoSanguineoRh VARCHAR(5) NULL,
  EcografiaRealizada BIT NULL,
  FechaEcografia DATE NULL,
  LugarEcografia NVARCHAR(100) NULL,
  PlanPartoEntregado BIT NULL,
  MicronutrientesEntregados NVARCHAR(100) NULL,
  ViajoUltSemanas BIT NULL,
  ReferenciaObstetrica BIT NULL,
  Consejerias NVARCHAR(200) NULL,
  Observaciones NVARCHAR(300) NULL,
  ProximaCitaFecha DATE NULL,
  EstablecimientoAtencion NVARCHAR(100) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

 



CREATE INDEX IX_ControlPrenatal_EmbarazoFecha ON ControlPrenatal(IdEmbarazo, Fecha);
GO

/* ==================  INTRAPARTO / PARTO  ================== */
CREATE TABLE Parto (
  IdParto INT IDENTITY(1,1) PRIMARY KEY,

  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),

  Fecha DATE NOT NULL,                             -- fecha del parto
  HoraIngreso DATETIME2 NULL,                       -- hora de ingreso al establecimiento
  HoraInicioTrabajo DATETIME2 NULL,                 -- hora de inicio del trabajo de parto
  HoraExpulsion DATETIME2 NULL,                     -- hora de expulsión del feto
  TipoParto NVARCHAR(50) NULL,                      -- “vaginal”, “cesárea”, “instrumental”
  IdViaParto SMALLINT NULL FOREIGN KEY REFERENCES ViaParto(IdViaParto),
  IndicacionCesarea NVARCHAR(150) NULL,             -- motivo de la cesárea

  Membranas NVARCHAR(10) NULL,                      -- estado de membranas al ingreso
  TiempoRoturaMembranasHoras INT NULL,              -- horas desde rotura de membranas
  IdLiquido SMALLINT NULL FOREIGN KEY REFERENCES LiquidoAmniotico(IdLiquido),
  AspectoLiquido NVARCHAR(50) NULL,                 -- descriptor adicional del líquido amniótico

  Analgesia NVARCHAR(50) NULL,                      -- tipo de analgesia
  PosicionMadre NVARCHAR(50) NULL,                  -- posición de la madre durante trabajo de parto
  Acompanante BIT NULL,                             -- si hubo acompañante (1=Sí,0=No)
  LugarNacimiento NVARCHAR(100) NULL,               -- establecimiento o domicilio

  DuracionSegundaEtapaMinutos INT NULL,             -- duración en minutos de la segunda etapa (empuje)
  PerdidasML INT NULL CHECK (PerdidasML IS NULL OR PerdidasML >= 0),  -- pérdidas de sangre estimadas
  Desgarro NVARCHAR(10) NULL,                        -- grado de desgarro (I, II, III, IV)
  Episiotomia BIT NULL,                             -- si se realizó episiotomía

  ComplicacionesMaternas NVARCHAR(300) NULL,        -- complicaciones registradas
  Derivacion BIT NULL,                              -- si hubo derivación a otro nivel
  SeguroTipo NVARCHAR(50) NULL,                     -- tipo de seguro (SIS, ESSALUD, privado, etc)

  NumeroHijosPrevios INT NULL,                      -- número de hijos vivos previos
  NumeroCesareasPrevias INT NULL,                    -- número de cesáreas previas
  EmbarazoMultiple BIT NULL,                         -- embarazo múltiple (1=Sí,0=No)
  NumeroGemelos INT NULL,                            -- número de gemelos/trillizos si aplica

  Observaciones NVARCHAR(500) NULL,                 -- observaciones adicionales

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

CREATE TABLE Bebe (
  IdBebe INT IDENTITY(1,1) PRIMARY KEY,
  IdParto INT NOT NULL FOREIGN KEY REFERENCES Parto(IdParto),
  NumeroBebe INT NOT NULL DEFAULT 1,                 -- 1,2 para gemelos, etc.
  Sexo CHAR(1) NULL CHECK (Sexo IN ('F','M')),
  FechaHoraNacimiento DATETIME2 NULL,
  Apgar1 TINYINT NULL CHECK (Apgar1 BETWEEN 0 AND 10),
  Apgar5 TINYINT NULL CHECK (Apgar5 BETWEEN 0 AND 10),
  PesoGr INT NULL CHECK (PesoGr IS NULL OR (PesoGr BETWEEN 300 AND 7000)),
  TallaCm DECIMAL(4,1) NULL,
  PerimetroCefalico DECIMAL(4,1) NULL,
  EG_Semanas DECIMAL(4,1) NULL,                      -- edad gestacional en semanas
  Reanimacion BIT NULL,                              -- si requirió reanimación
  Observaciones NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
);
GO

/* ==================  PUERPERIO (PNC)  ================== */
CREATE TABLE SeguimientoPuerperio (
  IdPuerperio INT IDENTITY(1,1) PRIMARY KEY,

  IdEmbarazo INT NOT NULL FOREIGN KEY REFERENCES Embarazo(IdEmbarazo),
  IdEncuentro INT NULL FOREIGN KEY REFERENCES Encuentro(IdEncuentro),
  IdProfesional INT NULL FOREIGN KEY REFERENCES ProfesionalSalud(IdProfesional),

  Fecha DATE NOT NULL,                              -- fecha del seguimiento posparto
  DiasPosparto INT NULL,                            -- número de días desde el parto
  PA_Sistolica TINYINT NULL CHECK (PA_Sistolica BETWEEN 50 AND 250),
  PA_Diastolica TINYINT NULL CHECK (PA_Diastolica BETWEEN 30 AND 180),
  Temp_C DECIMAL(4,1) NULL,                          -- temperatura corporal en grados C

  AlturaUterinaPP_cm DECIMAL(5,1) NULL,            -- altura uterina en cm en pos-parto
  InvolucionUterina VARCHAR(50) NULL,               -- descriptor de la involución uterina (“normal”, “retardada”, etc)
  Loquios NVARCHAR(20) NULL,                        -- descripción de los loquios
  HemorragiaResidual BIT NULL,                -- si hay sangrado o loquios excesivos, se puede usar BIT o descriptor
  Lactancia NVARCHAR(20) NULL,                       -- tipo de lactancia (“exclusiva”, “mixta”, “ninguna”)
  ApoyoLactancia BIT NULL,                           -- si se brindó consejería de lactancia (1=Sí,0=No)
  SignosInfeccion BIT NULL,                         -- si se detectaron signos de infección materna (1=Sí,0=No)
  TamizajeDepresion NVARCHAR(20) NULL,               -- resultado del tamizaje de depresión posparto

  IdMetodoPF SMALLINT NULL FOREIGN KEY REFERENCES MetodoPF(IdMetodoPF),  -- método de planificación familiar elegido
  ConsejoPlanificacion BIT NULL,                 -- si se brindó consejería de planificación familiar (1=Sí,0=No)
  VisitaDomiciliariaFecha DATE NULL,                 -- fecha de la visita domiciliaria (si aplica)
  SeguroTipo NVARCHAR(50) NULL,                      -- tipo de seguro o financiamiento (“SIS”, “EsSalud”, “Privado”, etc)

  ComplicacionesMaternas NVARCHAR(300) NULL,        -- complicaciones observadas durante el puerperio (mastitis, trombosis, etc)
  Derivacion BIT NULL,                               -- si la puérpera fue derivada a otro nivel de atención (1=Sí,0=No)
  EstablecimientoAtencion NVARCHAR(100) NULL,        -- establecimiento de salud donde se realiza el seguimiento

  Observaciones NVARCHAR(500) NULL,                 -- observaciones adicionales

  Estado BIT NOT NULL DEFAULT 1                      -- activo / inactivo
);
GO



CREATE INDEX IX_Puerperio_EmbarazoFecha
  ON SeguimientoPuerperio(IdEmbarazo, Fecha);
GO
/* ============  AYUDAS DIAGN�STICAS (ORDEN + RESULTADO)  ============ */
CREATE TABLE AyudaDiagnosticaOrden (
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
CREATE TABLE ResultadoDiagnostico (
  IdResultado INT IDENTITY(1,1) PRIMARY KEY,
  IdAyuda INT NOT NULL FOREIGN KEY REFERENCES AyudaDiagnosticaOrden(IdAyuda),
  FechaResultado DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
  Resumen NVARCHAR(400) NULL,
  Critico BIT NOT NULL DEFAULT 0,
  Estado NVARCHAR(20) NOT NULL DEFAULT N'Validado'
);
GO

CREATE TABLE ResultadoItem (
  IdResultadoItem INT IDENTITY(1,1) PRIMARY KEY,
  IdResultado INT NOT NULL FOREIGN KEY REFERENCES ResultadoDiagnostico(IdResultado),
  Parametro NVARCHAR(100) NOT NULL,
  ValorNumerico DECIMAL(12,4) NULL,
  ValorTexto NVARCHAR(200) NULL,
  Unidad NVARCHAR(40) NULL,
  RangoRef NVARCHAR(60) NULL
);
GO

CREATE TABLE ControlPrenatal_AyudaDiagnostica (
  IdCP_AD INT IDENTITY(1,1) PRIMARY KEY,
  IdControl INT NOT NULL FOREIGN KEY REFERENCES ControlPrenatal(IdControl),
  IdAyuda INT NOT NULL FOREIGN KEY REFERENCES AyudaDiagnosticaOrden(IdAyuda),
  FechaOrden DATETIME2 NULL,
  Comentario NVARCHAR(200) NULL,
  Estado BIT NOT NULL DEFAULT 1
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

--USE ProyectoCalidad;
--GO

--DELETE FROM Paciente;
--DELETE FROM PacienteEmail;
--DELETE FROM PacienteTelefono;

--DELETE FROM ProfesionalSalud;
--DELETE FROM ProfesionalEmail;
--DELETE FROM ProfesionalTelefono;


--DBCC CHECKIDENT ('Paciente', RESEED, 0);
--DBCC CHECKIDENT ('ProfesionalSalud', RESEED, 0);



/* ================== INSERCIONES DE DATOS DE PRUEBA ================== */

/* Insertar Pacientes */
INSERT INTO Paciente (Nombres, Apellidos, DNI, FechaNacimiento, Estado) VALUES
('María', 'González', '12345678', '1990-05-15', 1),
('Ana', 'López', '87654321', '1985-03-22', 1),
('Carla', 'Martínez', '11223344', '1992-07-10', 1),
('Sofía', 'Hernández', '44332211', '1988-12-05', 1),
('Lucía', 'Pérez', '55667788', '1995-01-30', 1),
('Isabella', 'Rodríguez', '66778899', '1993-09-12', 1),
('Valentina', 'Gómez', '77889900', '1987-11-25', 1),
('Camila', 'Díaz', '88990011', '1991-04-08', 1),
('Gabriela', 'Sánchez', '99001122', '1989-06-18', 1),
('Natalia', 'Romero', '00112233', '1994-02-14', 1),
('Daniela', 'Torres', '11223345', '1986-08-30', 1),
('Victoria', 'Ruiz', '22334455', '1996-10-05', 1),
('Emma', 'Flores', '33445566', '1984-12-20', 1),
('Luna', 'Acosta', '44556677', '1997-03-15', 1),
('Mia', 'Benítez', '55667789', '1983-07-22', 1);
GO

/* Insertar Emails de Pacientes */
INSERT INTO PacienteEmail (IdPaciente, Email, EsPrincipal) VALUES
(1, 'maria.gonzalez@email.com', 1),
(2, 'ana.lopez@email.com', 1),
(3, 'carla.martinez@email.com', 1),
(4, 'sofia.hernandez@email.com', 1),
(5, 'lucia.perez@email.com', 1),
(6, 'isabella.rodriguez@email.com', 1),
(7, 'valentina.gomez@email.com', 1),
(8, 'camila.diaz@email.com', 1),
(9, 'gabriela.sanchez@email.com', 1),
(10, 'natalia.romero@email.com', 1),
(11, 'daniela.torres@email.com', 1),
(12, 'victoria.ruiz@email.com', 1),
(13, 'emma.flores@email.com', 1),
(14, 'luna.acosta@email.com', 1),
(15, 'mia.benitez@email.com', 1);
GO

/* Insertar Teléfonos de Pacientes */
INSERT INTO PacienteTelefono (IdPaciente, Telefono, Tipo, EsPrincipal) VALUES
(1, '987654321', 'Celular', 1),
(2, '912345678', 'Celular', 1),
(3, '923456789', 'Celular', 1),
(4, '934567890', 'Celular', 1),
(5, '945678901', 'Celular', 1),
(6, '956789012', 'Celular', 1),
(7, '967890123', 'Celular', 1),
(8, '978901234', 'Celular', 1),
(9, '989012345', 'Celular', 1),
(10, '990123456', 'Celular', 1),
(11, '901234567', 'Celular', 1),
(12, '912345679', 'Celular', 1),
(13, '923456780', 'Celular', 1),
(14, '934567891', 'Celular', 1),
(15, '945678902', 'Celular', 1);
GO

/* Insertar Profesionales de Salud */
INSERT INTO ProfesionalSalud (CMP, Especialidad, Nombres, Apellidos, Estado) VALUES
('12345', 'Obstetricia y Ginecología', 'Dr. Juan', 'Ramírez', 1),
('67890', 'Medicina Familiar', 'Dra. Elena', 'Torres', 1),
('54321', 'Pediatría', 'Dr. Carlos', 'Vargas', 1),
('09876', 'Obstetricia y Ginecología', 'Dra. Patricia', 'Morales', 1),
('13579', 'Enfermería Obstétrica', 'Lic. Rosa', 'Flores', 1),
('24680', 'Ginecología', 'Dr. Miguel', 'Herrera', 1),
('13580', 'Medicina Interna', 'Dra. Laura', 'Castro', 1),
('86420', 'Obstetricia', 'Dr. Andrés', 'Jiménez', 1),
('97531', 'Enfermería', 'Lic. Ana', 'Silva', 1),
('75319', 'Pediatría', 'Dr. Fernando', 'Mendoza', 1),
('64208', 'Ginecología Oncológica', 'Dra. Sofia', 'Ortega', 1),
('53197', 'Medicina Familiar', 'Dr. Roberto', 'Guerrero', 1),
('42086', 'Obstetricia y Ginecología', 'Dra. Carmen', 'Reyes', 1),
('31975', 'Enfermería Obstétrica', 'Lic. Beatriz', 'Navarro', 1),
('20864', 'Pediatría Neonatal', 'Dr. Diego', 'Lozano', 1);
GO

/* Insertar Emails de Profesionales */
INSERT INTO ProfesionalEmail (IdProfesional, Email, EsPrincipal) VALUES
(1, 'juan.ramirez@hospital.com', 1),
(2, 'elena.torres@hospital.com', 1),
(3, 'carlos.vargas@hospital.com', 1),
(4, 'patricia.morales@hospital.com', 1),
(5, 'rosa.flores@hospital.com', 1),
(6, 'miguel.herrera@hospital.com', 1),
(7, 'laura.castro@hospital.com', 1),
(8, 'andres.jimenez@hospital.com', 1),
(9, 'ana.silva@hospital.com', 1),
(10, 'fernando.mendoza@hospital.com', 1),
(11, 'sofia.ortega@hospital.com', 1),
(12, 'roberto.guerrero@hospital.com', 1),
(13, 'carmen.reyes@hospital.com', 1),
(14, 'beatriz.navarro@hospital.com', 1),
(15, 'diego.lozano@hospital.com', 1);
GO

/* Insertar Teléfonos de Profesionales */
INSERT INTO ProfesionalTelefono (IdProfesional, Telefono, Tipo, EsPrincipal) VALUES
(1, '987123456', 'Oficina', 1),
(2, '912345678', 'Celular', 1),
(3, '923456789', 'Oficina', 1),
(4, '934567890', 'Celular', 1),
(5, '945678901', 'Oficina', 1),
(6, '956789012', 'Oficina', 1),
(7, '967890123', 'Celular', 1),
(8, '978901234', 'Oficina', 1),
(9, '989012345', 'Celular', 1),
(10, '990123456', 'Oficina', 1),
(11, '901234567', 'Celular', 1),
(12, '912345679', 'Oficina', 1),
(13, '923456780', 'Celular', 1),
(14, '934567891', 'Oficina', 1),
(15, '945678902', 'Celular', 1);
GO

/* =========================================================
   FIN DEL SCRIPT
   ========================================================= */
