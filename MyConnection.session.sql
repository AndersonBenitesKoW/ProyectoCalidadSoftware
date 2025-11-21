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
Go

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

