CREATE TABLE [dbo].[TestCase] (
    [TestCaseId]  INT            IDENTITY (1, 1) NOT NULL,
    [ProviderId]  INT            NOT NULL,
    [SourceId]    NVARCHAR (100) NULL,
    [Name]        NVARCHAR (300) NOT NULL,
    [ProductId]   INT            NOT NULL,
    [Feature]     NVARCHAR (300) NOT NULL,
    [ScriptPath]  NVARCHAR (MAX) NULL,
	[Ranking]	  NVARCHAR (300) NULL,
	[Release]     NVARCHAR (300) NULL,
    [Weight]      INT            NOT NULL,
    [IsAutomated] BIT            NOT NULL,
    [CreateBy]    INT            NULL,
    [CreateTime]  DATETIME       NULL,
    [ModifyBy]    INT            NULL,
    [ModifyTime]  DATETIME       NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsActive]    BIT            NOT NULL,
    CONSTRAINT [PK_TestCase] PRIMARY KEY CLUSTERED ([TestCaseId] ASC),
    CONSTRAINT [FK_TestCase_CreateUser] FOREIGN KEY ([CreateBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_TestCase_ModifyUser] FOREIGN KEY ([ModifyBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_TestCase_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]),
    CONSTRAINT [FK_TestCase_Provider] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId])
);

















