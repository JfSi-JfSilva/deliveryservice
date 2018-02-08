CREATE TABLE [dbo].[points] (
    [id]                INT           NOT NULL,
    [point_code]        NVARCHAR (32) NOT NULL,
    [point_description] NVARCHAR (64) NOT NULL,
    [iduser]            INT           DEFAULT ((-999)) NOT NULL,
    CONSTRAINT [PK_points] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_points_iduser] FOREIGN KEY ([iduser]) REFERENCES [dbo].[users] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [U01_points]
    ON [dbo].[points]([point_code] ASC);

