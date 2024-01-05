CREATE DATABASE Skola;
GO

USE Skola;
GO

CREATE TABLE Skola.dbo.Personal (
	PersonalID int IDENTITY(1,1) NOT NULL,
	Personnummer nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Förnamn nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Efternamn nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Personal PRIMARY KEY (PersonalID)
);

CREATE TABLE Skola.dbo.Roller (
	RollID int IDENTITY(1,1) NOT NULL,
	RollNamn nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Roller PRIMARY KEY (RollID)
);

CREATE TABLE Skola.dbo.Studenter (
	StudentID int IDENTITY(1,1) NOT NULL,
	Personnummer nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Förnamn nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Efternamn nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Studenter PRIMARY KEY (StudentID)
);

CREATE TABLE Skola.dbo.Klasser (
	KlassID int NOT NULL,
	StudentID int NOT NULL,
	CONSTRAINT PK_Klasser PRIMARY KEY (KlassID,StudentID),
	CONSTRAINT FK_Klasser_Studenter FOREIGN KEY (StudentID) REFERENCES Skola.dbo.Studenter(StudentID)
);

CREATE TABLE Skola.dbo.Kurser (
	KursID int NOT NULL,
	StudentID int NOT NULL,
	CONSTRAINT PK_Kurser PRIMARY KEY (KursID,StudentID),
	CONSTRAINT FK_Kurser_Studenter FOREIGN KEY (StudentID) REFERENCES Skola.dbo.Studenter(StudentID)
);

CREATE TABLE Skola.dbo.RollerPersonal (
	RollID int NOT NULL,
	PersonalID int NOT NULL,
	CONSTRAINT PK_RollerPersonal PRIMARY KEY (RollID,PersonalID),
	CONSTRAINT FK_RollerPersonal_Roller FOREIGN KEY (RollID) REFERENCES Skola.dbo.Roller(RollID),
	CONSTRAINT FK_Roller_Personal FOREIGN KEY (PersonalID) REFERENCES Skola.dbo.Personal(PersonalID)
);

CREATE TABLE Skola.dbo.Betyg (
	BetygID int IDENTITY(1,1) NOT NULL,
	Resultat nvarchar(5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Datum datetime NOT NULL,
	PersonalID int NOT NULL,
	StudentID int NOT NULL,
	KursID int NOT NULL,
	CONSTRAINT PK_Betyg PRIMARY KEY (BetygID),
	CONSTRAINT FK_Betyg_Kurser FOREIGN KEY (KursID,StudentID) REFERENCES Skola.dbo.Kurser(KursID,StudentID),
	CONSTRAINT FK_Betyg_Personal FOREIGN KEY (PersonalID) REFERENCES Skola.dbo.Personal(PersonalID)
);
GO

CREATE PROCEDURE [dbo].[SelectAllPersonal] @Roll INT = NULL
AS
BEGIN
    SELECT Personal.Personnummer, Personal.Förnamn, Personal.Efternamn, Roller.RollNamn 
    FROM Personal 
    LEFT JOIN RollerPersonal ON Personal.PersonalID = RollerPersonal.PersonalID 
    LEFT JOIN Roller ON RollerPersonal.RollID = Roller.RollID
    WHERE (@Roll IS NULL OR Roller.RollID = @Roll)
END
GO

INSERT INTO Skola.dbo.Personal (Personnummer,Förnamn,Efternamn) VALUES
	 (N'19460314-2672',N'Isabelle',N'Webb'),
	 (N'20151121-2498',N'Imogene',N'Simon'),
	 (N'19720128-1008',N'Rod',N'Sears'),
	 (N'20000229-1098',N'Henrik',N'Håkansson'),
	 (N'20011017-5432',N'Johanna',N'Jansson'),
	 (N'20030405-2109',N'Karl',N'Karlsson'),
	 (N'19890411-5489',N'Isak',N'Adamas');


INSERT INTO Skola.dbo.Roller (RollNamn) VALUES
	 (N'Lärare'),
	 (N'Administratör'),
	 (N'Rektor');

INSERT INTO Skola.dbo.RollerPersonal (RollID,PersonalID) VALUES
	 (1,1),
	 (1,2),
	 (1,4),
	 (1,5),
	 (1,6),
	 (2,1),
	 (3,1),
	 (3,3);

INSERT INTO Skola.dbo.Studenter (Personnummer,Förnamn,Efternamn) VALUES
	 (N'20150703-4740',N'Melanie',N'Doyle'),
	 (N'19910516-5931',N'Faustino',N'Craig'),
	 (N'20100115-8888',N'Magdalena',N'Erickson'),
	 (N'19900101-1234',N'Anna',N'Andersson'),
	 (N'19910215-5678',N'Erik',N'Berggren'),
	 (N'19930420-9876',N'Sofia',N'Carlsson'),
	 (N'19941010-2345',N'Gustav',N'Dahlström'),
	 (N'19951203-8765',N'Emma',N'Eklund'),
	 (N'19970308-3210',N'Oscar',N'Fransson'),
	 (N'19981112-6543',N'Maria',N'Gustafsson');

INSERT INTO Skola.dbo.Klasser (KlassID,StudentID) VALUES
	 (1,1),
	 (1,2),
	 (2,3),
	 (2,4),
	 (2,5),
	 (3,6),
	 (3,7),
	 (3,8),
	 (3,9),
	 (3,10);

INSERT INTO Skola.dbo.Kurser (KursID,StudentID) VALUES
	 (1,1),
	 (1,2),
	 (1,3),
	 (2,3),
	 (2,4),
	 (2,5),
	 (2,6),
	 (2,7),
	 (2,8),
	 (2,9),
	 (2,10),
	 (3,5),
	 (3,7),
	 (4,8),
	 (4,9);

INSERT INTO Skola.dbo.Betyg (Resultat,Datum,PersonalID,StudentID,KursID) VALUES
	 (N'G','2023-09-12 00:00:00.0',3,1,1),
	 (N'VG','2022-04-16 00:00:00.0',1,3,2),
	 (N'U','2023-12-03 00:00:00.0',2,2,1),
	 (N'G','2023-12-05 00:00:00.0',2,2,1),
	 (N'G','2022-06-04 00:00:00.0',4,4,2),
	 (N'VG','2022-06-04 00:00:00.0',4,5,3),
	 (N'U','2022-03-09 00:00:00.0',5,7,3),
	 (N'G','2023-07-18 00:00:00.0',6,8,4);