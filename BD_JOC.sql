DROP DATABASE IF EXISTS BD;
CREATE DATABASE BD;
USE BD;

CREATE TABLE Jugador (
  Us VARCHAR (60),
  Psw VARCHAR (60),
  PRIMARY KEY (Us)

)ENGINE = InnoDB;


CREATE TABLE Partida (
 Id  VARCHAR (60),
 Dia INT ,
 Mes INT ,
 Año INT ,
 H   INT ,
 M   INT ,
 Ganador VARCHAR (60),
PRIMARY KEY (Id)

)ENGINE = InnoDB;

CREATE TABLE Historial (
jugadores varchar (120),
  Id_part VARCHAR(60),
  PRIMARY KEY (jugadores),
FOREIGN KEY (Id_part) REFERENCES Partida(Id)

)ENGINE = InnoDB;

INSERT INTO Jugador VALUES ('kikito', 'contra');
INSERT INTO Jugador VALUES ('enric', 'prueba');
INSERT INTO Partida VALUES ('12345A', 1, 2, 1993, 1, 2, 'Juan');
INSERT INTO Historial VALUES ('kikito, juan, manuel, pepe', '12345A');
