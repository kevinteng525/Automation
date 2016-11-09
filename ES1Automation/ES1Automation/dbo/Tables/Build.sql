CREATE TABLE [dbo].[Build] (
    [BuildId]     INT            IDENTITY (1, 1) NOT NULL,
    [ProviderId]  INT            NOT NULL,
    [ProductId]   INT            NOT NULL,
    [Name]        NVARCHAR (300) NOT NULL,
    [Type]        INT            NOT NULL,
    [Status]      INT            NOT NULL,
    [BranchId]      INT NULL,
    [ReleaseId]      INT NULL,
    [Location]    NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Number] NCHAR(10) NULL, 
    CONSTRAINT [PK_TestBuild] PRIMARY KEY CLUSTERED ([BuildId] ASC),
    CONSTRAINT [FK_Build_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]),
    CONSTRAINT [FK_Build_Provider] FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[Provider] ([ProviderId]),
    CONSTRAINT [FK_Build_Branch] FOREIGN KEY ([BranchId]) REFERENCES [Branch]([BranchId]), 
    CONSTRAINT [FK_Build_Release] FOREIGN KEY ([ReleaseId]) REFERENCES [Release]([ReleaseId])
);





