CREATE TABLE [dbo].[TestSuite] (
    [SuiteId]     INT            IDENTITY (1, 1) NOT NULL,
    [ProviderId]  INT            NULL,
    [Type]        INT            NOT NULL,
    [Name]        NVARCHAR (200) NOT NULL,
    [SubSuites]   NVARCHAR (MAX) NULL,
    [TestCases]   NVARCHAR (MAX) NULL,
    [CreateBy]    INT            NULL,
    [CreateTime]  DATETIME       NULL,
    [ModityBy]    INT            NULL,
    [ModifyTime]  DATETIME       NULL,
    [Description] NVARCHAR (MAX) NULL,
    [IsActive]    BIT            NOT NULL,
	[SourceId]    NVARCHAR (200) NULL,
    [ExecutionCommand] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_TestSuite] PRIMARY KEY CLUSTERED ([SuiteId] ASC),
    CONSTRAINT [FK_TestSuite_CreateUser] FOREIGN KEY ([CreateBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_TestSuite_ModifyUser] FOREIGN KEY ([ModityBy]) REFERENCES [dbo].[User] ([UserId]),
	CONSTRAINT [FK_TestSuite_Provider] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId])
);










