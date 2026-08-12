-- DaoSuppliers_S1_Insert
-- 2026/8/12 日立 太郎
INSERT INTO 
  [Suppliers]
    (
      [SupplierID],
      [CompanyName],
      [ContactName],
      [ContactTitle],
      [Address],
      [City],
      [Region],
      [PostalCode],
      [Country],
      [Phone],
      [Fax],
      [HomePage]
    )
VALUES
    (
      @SupplierID,
      @CompanyName,
      @ContactName,
      @ContactTitle,
      @Address,
      @City,
      @Region,
      @PostalCode,
      @Country,
      @Phone,
      @Fax,
      @HomePage
    )
