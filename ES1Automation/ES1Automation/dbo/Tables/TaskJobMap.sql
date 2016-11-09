CREATE TABLE [dbo].[TaskJobMap] (
    [MapId]  INT IDENTITY (1, 1) NOT NULL,
    [TaskId] INT NOT NULL,
    [JobId]  INT NOT NULL,
    CONSTRAINT [PK_TaskJobMap] PRIMARY KEY CLUSTERED ([MapId] ASC),
    CONSTRAINT [FK_TaskJobMap_AutomationTask] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[AutomationTask] ([TaskId]),
    CONSTRAINT [FK_TaskJobMap_TestJob] FOREIGN KEY ([JobId]) REFERENCES [dbo].[AutomationJob] ([JobId])
);



