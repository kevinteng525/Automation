CREATE TABLE [dbo].[SupportedEnvironment] (
    [EnvironmentId] INT            NOT NULL,
    [Name]          NVARCHAR (500) NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [Type] INT NOT NULL, 
    [Config] NVARCHAR(MAX) NULL, 
	[ProviderId] INT NOT NULL,
    CONSTRAINT [PK_SupportedEnvironment] PRIMARY KEY CLUSTERED ([EnvironmentId] ASC),
	CONSTRAINT [FK_SupportedEnvironment_Provider] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId])
);

