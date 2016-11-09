CREATE TABLE [dbo].[AutomationTask] (
    [TaskId]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [Status]        INT            CONSTRAINT [DF_Task_TaskStatusId] DEFAULT ((0)) NOT NULL,
    [Type]          INT            NOT NULL,
    [Priority]      INT            NOT NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [CreateBy]      INT            NOT NULL,
    [ModifyDate]    DATETIME       NOT NULL,
    [ModifyBy]      INT            NOT NULL,
    [BuildId]       INT            NOT NULL,
    [EnvironmentId] INT            NOT NULL,
    [TestContent]   NVARCHAR (MAX) NULL,
    [Information]   NVARCHAR (MAX) NULL,
    [Description]   NVARCHAR (MAX) NULL,
	[RecurrencePattern]	INT		   NULL,
	[StartDate]		DATETIME       NULL,
	[StartTime]		DATETIME       NULL,
	[WeekDays]		INT			   NULL,
	[WeekInterval]	INT			   NULL,
    [ParentTaskId] INT NULL, 
    [BranchId] INT NULL, 
    [ReleaseId] INT NULL, 
    [ProductId] INT NULL, 
    [ProjectId] INT NULL, 
    [NotifyStakeholders] BIT NULL, 
    [WriteTestResultBack] BIT NULL, 
    [SetupScript] TEXT NULL, 
    [TeardownScript] TEXT NULL, 
    [EnableCodeCoverage] BIT NULL, 
    CONSTRAINT [PK_Task] PRIMARY KEY CLUSTERED ([TaskId] ASC),
    CONSTRAINT [FK_AutomationTask_CreateUser] FOREIGN KEY ([CreateBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_AutomationTask_ModifyUser] FOREIGN KEY ([ModifyBy]) REFERENCES [dbo].[User] ([UserId]),
    CONSTRAINT [FK_AutomationTask_SupportedEnvironment] FOREIGN KEY ([EnvironmentId]) REFERENCES [dbo].[SupportedEnvironment] ([EnvironmentId]),
    CONSTRAINT [FK_AutomationTask_TestBuild] FOREIGN KEY ([BuildId]) REFERENCES [dbo].[Build] ([BuildId]), 
    CONSTRAINT [FK_AutomationTask_Branch] FOREIGN KEY ([BranchId]) REFERENCES [Branch]([BranchId]), 
    CONSTRAINT [FK_AutomationTask_Release] FOREIGN KEY ([ReleaseId]) REFERENCES [Release]([ReleaseId]), 
    CONSTRAINT [FK_AutomationTask_Product] FOREIGN KEY ([ProductId]) REFERENCES [Product]([ProductId]),
	CONSTRAINT [FK_AutomationTask_Project] FOREIGN KEY ([ProjectId]) REFERENCES [Project]([ProjectId])
);















