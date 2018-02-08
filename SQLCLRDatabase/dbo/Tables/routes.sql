CREATE TABLE [dbo].[routes] (
    [id]                INT           NOT NULL,
    [route_code]        NVARCHAR (32) NOT NULL,
    [route_description] NVARCHAR (64) NOT NULL,
    [iduser]            INT           DEFAULT ((-999)) NOT NULL,
    CONSTRAINT [PK_routes] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_routes_iduser] FOREIGN KEY ([iduser]) REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [U01_routes]
    ON [dbo].[routes]([route_code] ASC);

