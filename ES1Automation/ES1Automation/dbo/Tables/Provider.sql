CREATE TABLE [dbo].[Provider] (
    [ProviderId]  INT             NOT NULL,
    [Name]        NVARCHAR (200)  NOT NULL,
    [Category]    INT             NOT NULL,
    [Type]        NVARCHAR (200)  NOT NULL,
    [Path]        NVARCHAR (2000) NOT NULL,
    [Config]      XML             NULL,
    [Description] NVARCHAR (MAX)  NULL,
    [IsActive]    BIT             NOT NULL,
    CONSTRAINT [PK_Provider] PRIMARY KEY CLUSTERED ([ProviderId] ASC)
);







