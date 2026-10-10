CREATE TABLE Gastos (
Id INT IDENTITY(1,1) PRIMARY KEY,
Descripcion NVARCHAR(100) NOT NULL,
Monto DECIMAL(10,2) NOT NULL,
Categoria NVARCHAR(50) NOT NULL
);

INSERT INTO Gastos (Descripcion, Monto, Categoria)
VALUES
('Almuerzo', 45.50, 'Comida'),
('Gasolina', 250.00, 'Transporte'),
('Cine', 75.00, 'Entretenimiento');

SELECT * FROM Gastos;