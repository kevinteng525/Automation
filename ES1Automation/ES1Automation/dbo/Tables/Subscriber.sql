CREATE TABLE [dbo].[Subscriber]
(
	[SubscriberId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ProjectId] INT NOT NULL, 
    [UserId] INT NOT NULL, 
    [CreateTime] DATETIME NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [SubscriberType] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_Subscriber_Project] FOREIGN KEY ([ProjectId]) REFERENCES [Project]([ProjectId]), 
    CONSTRAINT [FK_Subscriber_User] FOREIGN KEY ([UserId]) REFERENCES [User]([UserId])
)
