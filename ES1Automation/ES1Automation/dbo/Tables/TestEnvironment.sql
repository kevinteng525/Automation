CREATE TABLE [dbo].[TestEnvironment] (
    [EnvironmentId] INT            IDENTITY (1, 1) NOT NULL,
    [ProviderId]    INT            NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [Type]          NVARCHAR (200) NOT NULL,
    [Status]        INT            NOT NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [ModifyDate]    DATETIME       NOT NULL,
    [Config]        XML            NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Environment] PRIMARY KEY CLUSTERED ([EnvironmentId] ASC),
    CONSTRAINT [FK_TestEnvironment_Provider] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId]),
    CONSTRAINT [UQ_TestEnvironment_Name] UNIQUE NONCLUSTERED ([ProviderId], [Name])
);













