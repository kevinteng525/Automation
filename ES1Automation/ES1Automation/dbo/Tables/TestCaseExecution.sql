CREATE TABLE [dbo].[TestCaseExecution] (
    [ExecutionId] INT      IDENTITY (1, 1) NOT NULL,
    [TestCaseId]  INT      NOT NULL,
    [JobId]       INT      NOT NULL,
    [Status]      INT      NOT NULL,
    [StartTime]   DATETIME NULL,
    [EndTime]     DATETIME NULL,
    [RetryTimes]  INT      NOT NULL,
    [Timeout]     INT      NOT NULL,
    [Info] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_TestCaseExecution] PRIMARY KEY CLUSTERED ([ExecutionId] ASC),
    CONSTRAINT [FK_TestCaseExecution_AutomationJob] FOREIGN KEY ([JobId]) REFERENCES [dbo].[AutomationJob] ([JobId]),
    CONSTRAINT [FK_TestCaseExecution_TestCase] FOREIGN KEY ([TestCaseId]) REFERENCES [dbo].[TestCase] ([TestCaseId])
);







