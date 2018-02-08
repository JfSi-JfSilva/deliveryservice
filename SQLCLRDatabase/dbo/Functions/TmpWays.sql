CREATE FUNCTION [dbo].[TmpWays] ()
RETURNS 
@TmpWays TABLE
(	
    origin 		VARCHAR(32),
	destination	VARCHAR(32),
	fromidpoint INT,
	toidpoint 	INT,
	rp_cost 	INT,
	rp_time 	INT
)
AS 
BEGIN

	INSERT INTO @TmpWays (origin, destination, fromidpoint, toidpoint, rp_cost, rp_time)
	select	a.point_code, d.point_code,
			b.fromidpoint, b.toidpoint, b.rp_cost, b.rp_time
	from [dbo].route_points b, [dbo].[points] a, [dbo].[routes] c, [dbo].[points] d
	where a.id = b.fromidpoint
	and c.id = b.idroute
	and d.id = b.toidpoint;
	RETURN
	
END