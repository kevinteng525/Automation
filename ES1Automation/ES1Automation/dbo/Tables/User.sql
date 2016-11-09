CREATE TABLE [dbo].[User] (
    [UserId]      INT            IDENTITY (1, 1) NOT NULL,
    [Type]        INT            NOT NULL,
    [Username]    VARCHAR (50)   NOT NULL,
    [Password]    TEXT   NULL,
    [Role]        INT            NOT NULL,
    [IsActive]    BIT            NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Email] TEXT NULL, 
    CONSTRAINT [PK_ATFUser] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ__Username] UNIQUE NONCLUSTERED ([Username] ASC) 
);







