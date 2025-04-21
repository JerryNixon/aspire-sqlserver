CREATE TABLE Crew (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Equipment (
    Id INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Operation (
    Id INT PRIMARY KEY, -- seed: 1 = CheckOut, 2 = CheckIn
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE [Transaction] (
    Id INT IDENTITY PRIMARY KEY,
    Date DATETIME2 NOT NULL,
    CrewId INT NOT NULL,
    EquipmentId INT NOT NULL,
    OperationId INT NOT NULL,
    CONSTRAINT FK_Transaction_Crew FOREIGN KEY (CrewId) REFERENCES Crew(Id),
    CONSTRAINT FK_Transaction_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT FK_Transaction_Operation FOREIGN KEY (OperationId) REFERENCES Operation(Id)
);

INSERT INTO Operation (Id, Name) VALUES (1, 'CheckOut'), (2, 'CheckIn');
