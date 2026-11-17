USE master;
GO

-- Recrear la base de datos desde cero
IF DB_ID('MonitorSerial') IS NOT NULL
BEGIN
    ALTER DATABASE MonitorSerial
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE MonitorSerial;
END
GO

CREATE DATABASE MonitorSerial;
GO

USE MonitorSerial;
GO

-- Tabla de dispositivos
CREATE TABLE Dispositivo
(
    IdDispositivo INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    PuertoCOM NVARCHAR(20) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

-- Tabla de lecturas
CREATE TABLE Lectura
(
    IdLectura INT IDENTITY(1,1) PRIMARY KEY,

    IdDispositivo INT NOT NULL,

    Temperatura DECIMAL(5,2) NOT NULL,

    FechaHora DATETIME2(0) NOT NULL
        DEFAULT SYSDATETIME(),

    CONSTRAINT FK_Lectura_Dispositivo
        FOREIGN KEY (IdDispositivo)
        REFERENCES Dispositivo(IdDispositivo),

    CONSTRAINT CK_Lectura_Temperatura
        CHECK (Temperatura BETWEEN -40 AND 125)
);
GO

-- Dispositivo inicial para pruebas
INSERT INTO Dispositivo (Nombre, PuertoCOM, Activo)
VALUES ('Sensor de prueba', 'COM3', 1);
GO

SELECT
    L.IdLectura,
    D.Nombre AS Dispositivo,
    L.Temperatura,
    L.FechaHora
FROM Lectura L
INNER JOIN Dispositivo D
    ON L.IdDispositivo = D.IdDispositivo
ORDER BY L.FechaHora DESC;