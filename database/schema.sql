use SalesAnalyticsDB;

-- Drop existing tables
DROP TABLE IF EXISTS Sales;
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Regions;
GO

-- Drop existing view
DROP VIEW IF EXISTS vw_SalesAnalytics;
GO

-- ============================================
-- REGIONS TABLE (from Country)
-- ============================================
CREATE TABLE Regions (
    RegionId INT PRIMARY KEY IDENTITY(1,1),
    Country NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================================
-- PRODUCTS TABLE (from StockCode + Description)
-- ============================================
CREATE TABLE Products (
    ProductId INT PRIMARY KEY IDENTITY(1,1),
    StockCode NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================================
-- CUSTOMERS TABLE (from CustomerID)
-- ============================================
CREATE TABLE Customers (
    CustomerId INT PRIMARY KEY IDENTITY(1,1),
    CustomerCode NVARCHAR(50) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- ============================================
-- SALES TABLE (from Transaction data)
-- ============================================
CREATE TABLE Sales (
    SaleId INT PRIMARY KEY IDENTITY(1,1),
    InvoiceNo NVARCHAR(50) NOT NULL,
    ProductId INT NOT NULL,
    CustomerId INT NOT NULL,
    RegionId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    InvoiceDate DATETIME NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId),
    FOREIGN KEY (RegionId) REFERENCES Regions(RegionId)
);
GO

-- ============================================
-- ANALYTICS VIEW
-- ============================================
CREATE VIEW vw_SalesAnalytics AS
SELECT 
    s.SaleId,
    s.InvoiceNo,
    s.InvoiceDate,
    s.Quantity,
    s.UnitPrice,
    s.TotalAmount,
    p.ProductId,
    p.StockCode,
    p.Description AS ProductName,
    c.CustomerId,
    c.CustomerCode,
    r.RegionId,
    r.Country
FROM Sales s
INNER JOIN Products p ON s.ProductId = p.ProductId
INNER JOIN Customers c ON s.CustomerId = c.CustomerId
INNER JOIN Regions r ON s.RegionId = r.RegionId;
GO

-- ============================================
-- INDEXES
-- ============================================
CREATE INDEX IX_Sales_InvoiceDate ON Sales(InvoiceDate);
CREATE INDEX IX_Sales_ProductId ON Sales(ProductId);
CREATE INDEX IX_Sales_RegionId ON Sales(RegionId);
CREATE INDEX IX_Sales_InvoiceNo ON Sales(InvoiceNo);
