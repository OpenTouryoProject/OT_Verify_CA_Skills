-- SuppliersSelectAll
-- Suppliers テーブルの一覧を取得する（共通Dao＝CmnDao 用の静的パラメタライズドクエリ）
SELECT
  SupplierID,
  CompanyName,
  ContactName,
  ContactTitle,
  City,
  Country,
  Phone
FROM
  Suppliers
ORDER BY
  SupplierID
