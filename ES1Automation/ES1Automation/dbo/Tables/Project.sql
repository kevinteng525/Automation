CREATE TABLE [dbo].[Project]
(
	[ProjectId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(300) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [VCSType] INT NULL, 
    [VCSServer] NVARCHAR(100) NULL, 
    [VCSUser] NVARCHAR(50) NULL, 
    [VCSPassword] NVARCHAR(50) NULL, 
    [VCSRootPath] NVARCHAR(MAX) NULL, 
    [RunTime] NVARCHAR(150) NULL
)
