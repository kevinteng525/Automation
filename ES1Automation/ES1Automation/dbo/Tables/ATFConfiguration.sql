CREATE TABLE [dbo].[ATFConfiguration] (
    [ConfigName]  NVARCHAR (200) NOT NULL,
    [ConfigValue] NVARCHAR (MAX) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_SystemConfiguration] PRIMARY KEY CLUSTERED ([ConfigName] ASC)
);

