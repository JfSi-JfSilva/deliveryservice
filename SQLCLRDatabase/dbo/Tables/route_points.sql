CREATE TABLE [dbo].[route_points] (
    [id]          INT IDENTITY (1, 1) NOT NULL,
    [idroute]     INT NOT NULL,
    [fromidpoint] INT NOT NULL,
    [toidpoint]   INT NOT NULL,
    [rp_cost]     INT NOT NULL,
    [rp_time]     INT NOT NULL,
    [iduser]      INT DEFAULT ((-999)) NOT NULL,
    CONSTRAINT [PK_route_points] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_route_points_fromidpoint] FOREIGN KEY ([fromidpoint]) REFERENCES [dbo].[points] ([id]),
    CONSTRAINT [FK_route_points_idroute] FOREIGN KEY ([idroute]) REFERENCES [dbo].[routes] ([id]) ON DELETE CASCADE,
    CONSTRAINT [FK_route_points_iduser] FOREIGN KEY ([iduser]) REFERENCES [dbo].[users] ([id]),
    CONSTRAINT [FK_route_points_toidpoint] FOREIGN KEY ([toidpoint]) REFERENCES [dbo].[points] ([id])
);


GO
CREATE NONCLUSTERED INDEX [K01_route_points]
    ON [dbo].[route_points]([fromidpoint] ASC, [toidpoint] ASC, [idroute] ASC);

