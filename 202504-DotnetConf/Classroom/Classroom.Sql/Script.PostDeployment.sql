-- Seed Classes only if none exist
IF NOT EXISTS (SELECT 1 FROM Classes)
BEGIN
    INSERT INTO Classes (Id, Name) VALUES
    (1, 'Math'),
    (2, 'English'),
    (3, 'Science');
END

-- Seed Students only if none exist
IF NOT EXISTS (SELECT 1 FROM Students)
BEGIN
    INSERT INTO Students (Id, Name, ClassId) VALUES
    (1, 'Alice', 1),
    (2, 'Bob', 1),
    (3, 'Charlie', 1),
    (4, 'Daisy', 2),
    (5, 'Ethan', 2),
    (6, 'Fiona', 2),
    (7, 'George', 3),
    (8, 'Hannah', 3),
    (9, 'Isaac', 3),
    (10, 'Jill', 3);
END
