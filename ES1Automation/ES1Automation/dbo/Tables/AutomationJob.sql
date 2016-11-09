CREATE TABLE [dbo].[AutomationJob] (
    [JobId]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [SUTEnvironmentId] INT            NULL,
    [Type]          INT            NOT NULL,
    [Priority]      INT            NOT NULL,
    [Status]        INT            NOT NULL,
    [RetryTimes]    INT            CONSTRAINT [DF_Job_RetryTimes] DEFAULT ((1)) NOT NULL,
    [Timeout]       INT            NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [CreateBy]      INT            NOT NULL,
    [ModifyDate]    DATETIME       NOT NULL,
    [ModifyBy]      INT            NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [TestAgentEnvironmentId] INT NULL, 
    CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED ([JobId] ASC),
    CONSTRAINT [FK_AutomationJob_CreateUser] FOREIGN KEY ([CreateBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_AutomationJob_ModifyUser] FOREIGN KEY ([ModifyBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_AutomationJob_TestSUTEnvironment] FOREIGN KEY ([SUTEnvironmentId]) REFERENCES [dbo].[TestEnvironment] ([EnvironmentId]),
	CONSTRAINT [FK_AutomationJob_TestAgentEnvironment] FOREIGN KEY ([TestAgentEnvironmentId]) REFERENCES [dbo].[TestEnvironment] ([EnvironmentId])
);



















