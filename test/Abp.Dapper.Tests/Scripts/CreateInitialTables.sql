 CREATE TABLE IF NOT EXISTS Products (
											Id INTEGER PRIMARY KEY
										,	Name varchar(100) 
										,	IsDeleted BOOLEAN
										,	DeleterUserId BIGINT
										,	DeletionTime DATETIME
										,	LastModificationTime DATETIME
										,	LastModifierUserId BIGINT
										,	CreationTime DATETIME
										,	CreatorUserId BIGINT
										,	TenantId INTEGER NULLABLE
									);

 CREATE TABLE IF NOT EXISTS ProductDetails (
											Id INTEGER PRIMARY KEY
										,	Gender varchar(100) 
										,	IsDeleted BOOLEAN
										,	DeleterUserId BIGINT
										,	DeletionTime DATETIME
										,	LastModificationTime DATETIME
										,	LastModifierUserId BIGINT
										,	CreationTime DATETIME
										,	CreatorUserId BIGINT
										,	TenantId INTEGER
									);

 

 