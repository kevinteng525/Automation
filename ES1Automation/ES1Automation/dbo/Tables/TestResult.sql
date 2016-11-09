CREATE TABLE [dbo].[TestResult] (
    [ResultId]    INT            IDENTITY (1, 1) NOT NULL,
    [ExecutionId] INT            NOT NULL,
    [Result]      INT            NOT NULL,
    [IsTriaged]   BIT            NOT NULL,
    [TriagedBy]   INT            NULL,
    [Files]       NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Result] PRIMARY KEY CLUSTERED ([ResultId] ASC),
    CONSTRAINT [FK_TestResult_TestCaseExecution] FOREIGN KEY ([ExecutionId]) REFERENCES [dbo].[TestCaseExecution] ([ExecutionId]),
    CONSTRAINT [FK_TestResult_User] FOREIGN KEY ([TriagedBy]) REFERENCES [dbo].[User] ([UserId])
);















