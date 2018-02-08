/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [dbo].[users] ([id], [login], [password], [name], [email], [is_admin]) 
VALUES
(-999, 'sa', CONVERT(VARCHAR(32), HashBytes('MD5', 'ReD'), 2), 'System Administrator', 'jorge.fonseca.silva@gmail.com', 1)

INSERT INTO [dbo].[login] ([token], [iduser]) 
VALUES
(CONVERT(VARCHAR(256), HashBytes('SHA2_256', '0123456789 9876543210 ReD'), 2), -999)

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(1, 'A', 'A')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(2, 'B', 'B')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(3, 'C', 'C')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(4, 'D', 'D')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(5, 'E', 'E')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(6, 'F', 'F')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(7, 'G', 'G')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(8, 'H', 'H')

INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) 
VALUES
(9, 'I', 'I');

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(1, 'A-C-B', 'A-C-B')

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(2, 'A-E-D', 'A-E-D')

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(3, 'D-F', 'D-F')

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(4, 'F-I-B', 'F-I-B')

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(5, 'F-G-B', 'F-G-B')

INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) 
VALUES
(6, 'A-H-E-D', 'A-H-E-D')

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(1, 1, 3, 10000, 1)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(1, 3, 2, 12, 1)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(4, 6, 9, 50, 45)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(4, 9, 2, 5, 55)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(5, 6, 7, 50, 40)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(5, 7, 2, 73, 64)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(2, 1, 5, 5, 30)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(2, 5, 4, 5, 3)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(6, 1, 8, 1, 10)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(6, 8, 5, 1, 30)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(6, 5, 4, 5, 3)

INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) 
VALUES
(3, 4, 6, 50, 5)
