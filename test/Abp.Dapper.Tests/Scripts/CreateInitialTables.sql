

IF ( NOT EXISTS ( SELECT    *
                  FROM      INFORMATION_SCHEMA.TABLES
                  WHERE     TABLE_SCHEMA = 'dbo'
                            AND TABLE_NAME = 'Products' )
   )
    BEGIN
        CREATE TABLE [dbo].[Products]
            (
              [Id] [INT] IDENTITY(1, 1)
                         NOT NULL
            , [Name] [NVARCHAR](50) NOT NULL
            , [IsDeleted] [BIT] NOT NULL
            , [DeleterUserId] BIGINT NULL
            , [DeletionTime] [DATETIME] NULL
            , [LastModificationTime] [DATETIME] NULL
            , [LastModifierUserId] BIGINT NULL
            , [CreationTime] [DATETIME] NOT NULL
            , [CreatorUserId] BIGINT NULL
            , TenantId INT NULL
            )
        ON  [PRIMARY]
    END


IF ( NOT EXISTS ( SELECT    *
                  FROM      INFORMATION_SCHEMA.TABLES
                  WHERE     TABLE_SCHEMA = 'dbo'
                            AND TABLE_NAME = 'ProductDetails' )
   )
    BEGIN
        CREATE TABLE [dbo].ProductDetails
            (
              [Id] [INT] IDENTITY(1, 1)
                         NOT NULL
            , Gender [NVARCHAR](50) NOT NULL
            , [IsDeleted] [BIT] NOT NULL
            , [DeleterUserId] BIGINT NULL
            , [DeletionTime] [DATETIME] NULL
            , [LastModificationTime] [DATETIME] NULL
            , [LastModifierUserId] BIGINT NULL
            , [CreationTime] [DATETIME] NOT NULL
            , [CreatorUserId] BIGINT NULL
            , TenantId INT NOT NULL
            )
        ON  [PRIMARY]
    END


