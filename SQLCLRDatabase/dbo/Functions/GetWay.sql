CREATE FUNCTION [dbo].[GetWay]
(     
      @start_point	VARCHAR(32),
      @end_point	VARCHAR(32),
	  @sort_by		VARCHAR(4)
)
RETURNS 
@GetWay TABLE
(	
    origin 		VARCHAR(32),
	destination VARCHAR(32),
	w_path		VARCHAR(8000),
	fromidpoint INT,
	toidpoint 	INT,
	rp_value 	INT
)
AS 
BEGIN
	;WITH 	CTE1 		(origin, destination, w_path, fromidpoint, toidpoint, rp_value)
			AS 	(
					select 
						origin, 
						destination, 
						cast(origin as varchar(8000)), 
						fromidpoint, 
						toidpoint, 
						CASE WHEN @sort_by = 'cost' then rp_cost else rp_time END
					from [dbo].TmpWays()
				),

			SRCURSCTE 	(origin, destination, w_path, fromidpoint, toidpoint, rp_value)
			AS 	(
					select 
						origin, 
						destination, 
						w_path, 
						fromidpoint, 
						toidpoint, 
						rp_value
                    from CTE1
					where origin = @start_point
					
					UNION ALL
					
					select 
						a.origin,
						b.destination,						
						cast(a.w_path + '->' + b.origin + CASE WHEN b.destination = @end_point THEN '->' + b.destination ELSE '' END as varchar(8000)),
						b.fromidpoint, b.toidpoint, 
						a.rp_value + CASE WHEN b.origin = @end_point THEN 0 ELSE b.rp_value END
						from SRCURSCTE a,CTE1 b
						where a.toidpoint = b.fromidpoint
						and a.origin != @end_point					
				)

	

	INSERT INTO @GetWay (origin, destination, w_path, fromidpoint, toidpoint, rp_value)
	select 
		DISTINCT
		origin, destination, w_path, fromidpoint, toidpoint, rp_value
	FROM SRCURSCTE
	WHERE destination = @end_point
	AND LEN(w_path) - LEN(REPLACE(w_path,'->','')) > 2
	OPTION (MAXRECURSION 3660);
	RETURN
	
END