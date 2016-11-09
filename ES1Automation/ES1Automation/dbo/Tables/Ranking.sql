CREATE TABLE [dbo].[Ranking]
(
	[RankingId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL
)
