IF ( EXISTS ( SELECT    *
              FROM      INFORMATION_SCHEMA.TABLES
              WHERE     TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'Products' ) )
    BEGIN
        DROP TABLE dbo.Products
    END

IF ( EXISTS ( SELECT    *
              FROM      INFORMATION_SCHEMA.TABLES
              WHERE     TABLE_SCHEMA = 'dbo'
                        AND TABLE_NAME = 'ProductDetails' ) )
    BEGIN
        DROP TABLE dbo.ProductDetails
    END
