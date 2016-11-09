CREATE TABLE [dbo].[DiagnosticLog]
(
	[LogId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CreateTime] DATETIME NULL, 
    [Component] NVARCHAR(300) NULL, 
    [LogType] INT NULL, 
    [Message] NVARCHAR(MAX) NULL
)
