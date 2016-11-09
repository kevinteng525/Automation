CREATE TABLE [dbo].[Release]
(
	[ReleaseId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Path] NVARCHAR(MAX) NULL, 
    [ProductId] INT NOT NULL, 
    [Type] INT NOT NULL, 
    CONSTRAINT [FK_Release_Product] FOREIGN KEY ([ProductId]) REFERENCES [Product]([ProductId])
)
