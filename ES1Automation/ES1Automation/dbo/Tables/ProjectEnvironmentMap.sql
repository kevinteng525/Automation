CREATE TABLE [dbo].[ProjectEnvironmentMap] (
    [MapId]         INT IDENTITY (1, 1) NOT NULL,
    [ProjectId]     INT NOT NULL,
    [EnvironmentId] INT NOT NULL,
    CONSTRAINT [PK_ProjectEnvironmentMap] PRIMARY KEY CLUSTERED ([MapId] ASC),
    CONSTRAINT [FK_ProjectEnvironmentMap_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]),
    CONSTRAINT [FK_ProjectEnvironmentMap_SupportedEnvironment] FOREIGN KEY ([EnvironmentId]) REFERENCES [dbo].[SupportedEnvironment] ([EnvironmentId])
);

