CREATE TABLE [dbo].[Branch]
(
	[BranchId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Path] NVARCHAR(MAX) NULL, 
    [ProductId] INT NOT NULL, 
    [Type] INT NOT NULL, 
    CONSTRAINT [FK_Branch_Product] FOREIGN KEY ([ProductId]) REFERENCES [Product]([ProductId])
)
