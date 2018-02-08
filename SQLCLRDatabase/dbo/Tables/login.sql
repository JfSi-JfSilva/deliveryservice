CREATE TABLE [dbo].[login] (
    [token]           NVARCHAR (256) NOT NULL,
    [iduser]          INT            NOT NULL,
    [crate_timestamp] DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_login] PRIMARY KEY CLUSTERED ([token] ASC),
    CONSTRAINT [FK_login_iduser] FOREIGN KEY ([iduser]) REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
);

