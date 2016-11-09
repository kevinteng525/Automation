CREATE TABLE [dbo].[Product] (
    [ProductId]   INT            NOT NULL,
    [Name]        NVARCHAR (300) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [BuildProviderId] INT NULL, 
    [TestCaseProviderId] INT NULL, 
    [EnvironmentProviderId] INT NULL, 
    [RunTime] NVARCHAR(150) NULL, 
    [RQMProjectAlias] NVARCHAR(200) NULL, 
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductId] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC), 
    CONSTRAINT [FK_Product_BuildProvider] FOREIGN KEY ([BuildProviderId]) REFERENCES [Provider]([ProviderId]), 
    CONSTRAINT [FK_Product_TestCaseProvider] FOREIGN KEY ([TestCaseProviderId]) REFERENCES [Provider]([ProviderId]), 
    CONSTRAINT [FK_Product_EnvironmentProvider] FOREIGN KEY ([EnvironmentProviderId]) REFERENCES [Provider]([ProviderId])
);



