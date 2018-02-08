CREATE TABLE [dbo].[users] (
    [id]       INT            NOT NULL,
    [login]    NVARCHAR (32)  NOT NULL,
    [password] NVARCHAR (32)  NOT NULL,
    [name]     NVARCHAR (64)  NOT NULL,
    [email]    NVARCHAR (128) NOT NULL,
    [is_admin] BIT            DEFAULT ((0)) NOT NULL,
    [iduser]   INT            DEFAULT ((-999)) NOT NULL,
    CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [U01_users]
    ON [dbo].[users]([login] ASC);

