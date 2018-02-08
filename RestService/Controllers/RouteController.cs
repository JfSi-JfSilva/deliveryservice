using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace RestService.Controllers
{
    public class Login
    {
        public int iduser { get; set; }
    }

    public class Routes
    {
        public List<Route> routes { get; set; }
        public string err_message { get; set; }
        public Routes()
        {
            routes = new List<Route>();
        }
    }

    public class RoutesPoints
    {
        public List<Route> routes { get; set; }
    }

    public class Route
    {
        public int idroute { get; set; }
        public string route_code { get; set; }
        public string route_description { get; set; }
        public List<WayPoint> waypoint { get; set; }
        public Route()
        {
            waypoint = new List<WayPoint>();
        }
    }

    public class RouteID
    {
        public int idroute { get; set; }
        public string route_code { get; set; }
        public string route_description { get; set; }
        public List<WayPoint> waypoint { get; set; }
        public string err_message { get; set; }
        public RouteID()
        {
            waypoint = new List<WayPoint>();
        }
    }

    public class WayPoint
    {
        public string fromidpoint { get; set; }
        public string toidpoint { get; set; }
        public int rp_cost { get; set; }
        public int rp_time { get; set; }
    }

    public class RouteController : ApiController
    {
        #region Private Members

        private string Host = "localhost";
        private SqlConnection conn;
        private SqlCommand command;
        private ConnectionStringSettings connString;
        private ErrorHandler.ErrorHandler errHandler;
        private Login login;

        #endregion

        #region Main Functions

        // GET: api/Route
        [Route("api/Route/GetAllRoute")]
        [HttpGet]
        public string GetAllRoute()
        {
            Routes routes = new Routes();

            errHandler = new ErrorHandler.ErrorHandler
            {
                ErrorMessage = ""
            };

            try
            {
                GetConectionString();

                List<Route> RouteList = new List<Route>();
                RouteList = GetAllRoutes();

                foreach (Route route in RouteList)
                {
                    routes.routes.Add(route);
                }          
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            routes.err_message = errHandler.ErrorMessage;

            return JsonConvert.SerializeObject(routes);
        }

        // GET: api/Route/5
        public string Get(int id)
        {
            RouteID route = new RouteID();

            errHandler = new ErrorHandler.ErrorHandler
            {
                ErrorMessage = ""
            };

            try
            {
                GetConectionString();

                route = GetUniqueRoute(id);
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            route.err_message = errHandler.ErrorMessage;

            return JsonConvert.SerializeObject(route);
        }

        // POST: api/Route
        public string Post([FromBody]Models.Route route)
        {
            errHandler = new ErrorHandler.ErrorHandler
            {
                ErrorMessage = ""
            };

            bool bContinue = true;

            try
            {
                if (string.IsNullOrEmpty(route.Str_key))
                {
                    errHandler.ErrorMessage = "Str_key Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Str_key.ToString().Trim().Length >= 256)
                    {
                        errHandler.ErrorMessage = "str_key Field length > 256";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(route.Route_code))
                {
                    errHandler.ErrorMessage = "Route_code Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Route_code.ToString().Trim().Length > 32)
                    {
                        errHandler.ErrorMessage = "Route_code Field length > 32";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(route.Route_description))
                {
                    errHandler.ErrorMessage = "Route_description Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Route_description.ToString().Trim().Length > 64)
                    {
                        errHandler.ErrorMessage = "Route_description Field length > 64";
                        bContinue = false;
                    }
                }

                bool bFieldExitsNotEmpty = true;

                try
                {
                    if (route.Waypoint.Count < 2)
                    {
                        errHandler.ErrorMessage = "Waypoint Field must have at lest two waypoint, start point and end point";
                        bContinue = false;
                        bFieldExitsNotEmpty = false;
                    }
                }
                catch
                {
                    errHandler.ErrorMessage = "Waypoint Field is requiered";
                    bContinue = false;
                    bFieldExitsNotEmpty = false;
                }
                finally
                {
                    if (bFieldExitsNotEmpty)
                    {
                        int iSeq = 0;
                        foreach (Models.WayPoint wp in route.Waypoint)
                        {
                            bFieldExitsNotEmpty = true;

                            if (string.IsNullOrEmpty(wp.Rp_Code))
                            {
                                errHandler.ErrorMessage = "Rp_Code Waypoint: (" + iSeq.ToString().Trim() + ") Field is requiered";
                                bContinue = false;
                                bFieldExitsNotEmpty = false;
                            }
                            else
                            {
                                if (wp.Rp_Code.ToString().Trim().Length > 32)
                                {
                                    errHandler.ErrorMessage = "Rp_Code Waypoint: (" + iSeq.ToString().Trim() + ") length > 32";
                                    bContinue = false;
                                    bFieldExitsNotEmpty = false;
                                }
                            }

                            if (string.IsNullOrEmpty(wp.Rp_Description))
                            {
                                errHandler.ErrorMessage = "Rp_Description Waypoint: (" + iSeq.ToString().Trim() + ") Field is requiered";
                                bContinue = false;
                                bFieldExitsNotEmpty = false;
                            }
                            else
                            {
                                if (wp.Rp_Description.ToString().Trim().Length > 64)
                                {
                                    errHandler.ErrorMessage = "Rp_Description Waypoint: (" + iSeq.ToString().Trim() + ") length > 64";
                                    bContinue = false;
                                    bFieldExitsNotEmpty = false;
                                }
                            }

                            iSeq++;
                        }
                    }
                }

                if (bContinue)
                {
                    GetConectionString();

                    if (CheckToken(route.Str_key.ToString().Trim(), true))
                    {
                        InsertRoute(route);
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            return JsonConvert.SerializeObject(errHandler);
        }

        // PUT: api/Route/5
        public string Put(int id, [FromBody]Models.Route route)
        {
            errHandler = new ErrorHandler.ErrorHandler
            {
                ErrorMessage = ""
            };

            bool bContinue = true;

            try
            {
                if (string.IsNullOrEmpty(route.Str_key))
                {
                    errHandler.ErrorMessage = "Str_key Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Str_key.ToString().Trim().Length >= 256)
                    {
                        errHandler.ErrorMessage = "str_key Field length > 256";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(route.Route_code))
                {
                    errHandler.ErrorMessage = "Route_code Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Route_code.ToString().Trim().Length > 32)
                    {
                        errHandler.ErrorMessage = "Route_code Field length > 32";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(route.Route_description))
                {
                    errHandler.ErrorMessage = "Route_description Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Route_description.ToString().Trim().Length > 64)
                    {
                        errHandler.ErrorMessage = "Route_description Field length > 64";
                        bContinue = false;
                    }
                }

                bool bFieldExitsNotEmpty = true;

                try
                {
                    if (route.Waypoint.Count < 2)
                    {
                        errHandler.ErrorMessage = "Waypoint Field must have at lest two waypoint, start point and end point";
                        bContinue = false;
                        bFieldExitsNotEmpty = false;
                    }
                }
                catch
                {
                    errHandler.ErrorMessage = "Waypoint Field is requiered";
                    bContinue = false;
                    bFieldExitsNotEmpty = false;
                }
                finally
                {
                    if (bFieldExitsNotEmpty)
                    {
                        int iSeq = 0;
                        foreach (Models.WayPoint wp in route.Waypoint)
                        {
                            bFieldExitsNotEmpty = true;

                            if (string.IsNullOrEmpty(wp.Rp_Code))
                            {
                                errHandler.ErrorMessage = "Rp_Code Waypoint: (" + iSeq.ToString().Trim() + ") Field is requiered";
                                bContinue = false;
                                bFieldExitsNotEmpty = false;
                            }
                            else
                            {
                                if (wp.Rp_Code.ToString().Trim().Length > 32)
                                {
                                    errHandler.ErrorMessage = "Rp_Code Waypoint: (" + iSeq.ToString().Trim() + ") length > 32";
                                    bContinue = false;
                                    bFieldExitsNotEmpty = false;
                                }
                            }

                            if (string.IsNullOrEmpty(wp.Rp_Description))
                            {
                                errHandler.ErrorMessage = "Rp_Description Waypoint: (" + iSeq.ToString().Trim() + ") Field is requiered";
                                bContinue = false;
                                bFieldExitsNotEmpty = false;
                            }
                            else
                            {
                                if (wp.Rp_Description.ToString().Trim().Length > 64)
                                {
                                    errHandler.ErrorMessage = "Rp_Description Waypoint: (" + iSeq.ToString().Trim() + ") length > 64";
                                    bContinue = false;
                                    bFieldExitsNotEmpty = false;
                                }
                            }

                            iSeq++;
                        }
                    }
                }

                if (bContinue)
                {
                    GetConectionString();

                    if (CheckToken(route.Str_key.ToString().Trim(), true))
                    {
                        InsertRoute(route, id);
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            return JsonConvert.SerializeObject(errHandler);
        }

        // DELETE: api/Route/5
        [HttpDelete]
        public string Delete(int id, [FromBody]Models.Route route)
        {
            errHandler = new ErrorHandler.ErrorHandler
            {
                ErrorMessage = ""
            };

            bool bContinue = true;

            try
            {
                if (string.IsNullOrEmpty(route.Str_key))
                {
                    errHandler.ErrorMessage = "str_key Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (route.Str_key.ToString().Trim().Length >= 256)
                    {
                        errHandler.ErrorMessage = "Str_key Field length > 256";
                        bContinue = false;
                    }
                }
 
                if (bContinue)
                {
                    GetConectionString();

                    if (CheckToken(route.Str_key.ToString().Trim(), true))
                    {
                        DelRoute(id);
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            return JsonConvert.SerializeObject(errHandler);
        }

        #endregion

        #region Utility Functions

        /// <summary>
        /// Connection to database
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        private void GetConectionString()
        {
            Configuration rootWebConfig =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/" + Host.Trim());

            if (rootWebConfig.ConnectionStrings.ConnectionStrings.Count != 0)
            {
                connString =
                    rootWebConfig.ConnectionStrings.ConnectionStrings["DBConnectionString"];
                if (connString == null)
                {
                    throw new System.ArgumentException("No DB connection string");
                }
            }
            else
            {
                throw new System.ArgumentException("Could not find configuratuon file");
            }
        }
        
        /// <summary>
        /// Database SELECT - Check token exists
        /// Can Check if is Administrator (Default = false)
        /// </summary>
        /// <param name="token"></param>
        /// <param name="bIsAdmin"></param>
        /// <returns></returns>
        private bool CheckToken(string strtoken, bool bIsAdmin = false)
        {
            bool bResult = false;

            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select a.[iduser], CAST(b.[is_admin] AS INT) from[dbo].[login] a, [dbo].[users] b where a.[token]=@strtoken and a.[iduser] = b.[iduser]";

                    command = new SqlCommand(sqlSelectString, conn);
                    SqlParameter IDparam = new SqlParameter("@strtoken", strtoken);
                    command.Parameters.Add(IDparam);
                    command.Connection.Open();

                    login = new Login
                    {
                        iduser = 0
                    };

                    int iIsAdmin = 0;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        login.iduser = Convert.ToInt32(reader[0].ToString());
                        iIsAdmin = Convert.ToInt32(reader[1].ToString());
                    }

                    if (login.iduser == 0)
                    {
                        errHandler.ErrorMessage = "The token supplied is not valid";
                    }
                    else
                    if (bIsAdmin && iIsAdmin == 0)
                    {
                        errHandler.ErrorMessage = "Token as no administrator priveliges, can not continue";
                    }
                    else
                    {
                        bResult = true;
                    }

                    command.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }

            return bResult;
        }

        /// <summary>
        /// Method - Get list of all employees
        /// </summary>
        /// <param name="Waypoint"></param>
        /// <returns></returns>
        private void InsertRoute(Models.Route route, int id=0)
        {
            try
            {
                DelRoute(id, false);

                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select a.[id] from [dbo].[routes] a where [route_code] = '" + route.Route_code.ToString() + "'";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    int idroute = 0;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idroute = Convert.ToInt16(reader[0].ToString());
                    }

                    command.Connection.Close();

                    if (idroute != 0)
                    {
                        errHandler.ErrorMessage = "Route with code: " + route.Route_code.ToString() + "already exists";
                    }
                    else
                    {
                        if (DBPoint(route.Waypoint))
                        {
                            if (id == 0)
                            {
                                sqlSelectString = "select MAX(a.[id]) from [dbo].[routes] a";
                                command = new SqlCommand(sqlSelectString, conn);
                                command.Connection.Open();

                                SqlDataReader reader2 = command.ExecuteReader();
                                if (reader2.Read())
                                {
                                    id = Convert.ToInt16(reader2[0].ToString());
                                }
                                id++;

                                command.Connection.Close();
                            }

                            //INSERT
                            string sqlInsertString =
                                "INSERT INTO [dbo].[routes] ([id], [route_code], [route_description]) VALUES (@ID, @route_code, @route_description)";

                            command = new SqlCommand();
                            command.Connection = conn;
                            command.Connection.Open();
                            command.CommandText = sqlInsertString;

                            SqlParameter route_codeparam = new SqlParameter("@route_code", route.Route_code.ToString());
                            SqlParameter route_descriptionparam = new SqlParameter("@route_description", route.Route_description.ToString());
                            SqlParameter IDparam = new SqlParameter("@ID", id.ToString());

                            command.Parameters.AddRange(new SqlParameter[] { IDparam, route_codeparam, route_descriptionparam });
                            command.ExecuteNonQuery();
                            command.Connection.Close();

                            string FromPoint = "";
                            int iSeq = 0;
                            foreach (Models.WayPoint wp in route.Waypoint)
                            {
                                if (iSeq != 0)
                                {
                                    command = new SqlCommand();
                                    sqlSelectString = "select a.[id] from [dbo].[points] a where a.[point_code] = '" + FromPoint.ToString() + "'";
                                    command = new SqlCommand(sqlSelectString, conn);
                                    command.Connection.Open();

                                    int iFromdpoint = 0;

                                    SqlDataReader Fromreader = command.ExecuteReader();
                                    if (Fromreader.Read())
                                    {
                                        iFromdpoint = Convert.ToInt16(Fromreader[0].ToString());
                                    }

                                    command.Connection.Close();

                                    sqlSelectString = "select a.[id] from [dbo].[points] a where a.[point_code] = '" + wp.Rp_Code.ToString() + "'";
                                    command = new SqlCommand(sqlSelectString, conn);
                                    command.Connection.Open();

                                    int iTopoint = 0;

                                    SqlDataReader Toreader = command.ExecuteReader();
                                    if (Toreader.Read())
                                    {
                                        iTopoint = Convert.ToInt16(Toreader[0].ToString());
                                    }

                                    command.Connection.Close();

                                    //INSERT
                                    sqlInsertString =
                                        "INSERT INTO [dbo].[route_points] ([idroute], [fromidpoint], [toidpoint], [rp_cost], [rp_time]) VALUES (@ID, @fromidpoint, @toidpoint, @rp_cost, @rp_time)";

                                    command = new SqlCommand
                                    {
                                        Connection = conn
                                    };
                                    command.Connection.Open();
                                    command.CommandText = sqlInsertString;

                                    SqlParameter Fromparam = new SqlParameter("@fromidpoint", iFromdpoint.ToString());
                                    SqlParameter Toparam = new SqlParameter("@toidpoint", iTopoint.ToString());
                                    SqlParameter Costaram = new SqlParameter("@rp_cost", wp.Rp_cost.ToString());
                                    SqlParameter Timeparam = new SqlParameter("@rp_time", wp.Rp_time.ToString());
                                    IDparam = new SqlParameter("@ID", id.ToString());

                                    command.Parameters.AddRange(new SqlParameter[] { IDparam, Fromparam, Toparam, Costaram, Timeparam });
                                    command.ExecuteNonQuery();
                                    command.Connection.Close();
                                }

                                iSeq++;
                                FromPoint = wp.Rp_Code.ToString();
                            }

                            errHandler.ErrorMessage = "Route created";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }
        }

        /// <summary>
        /// Method - Get list of all employees
        /// </summary>
        /// <param name="Waypoint"></param>
        /// <returns></returns>
        private bool DBPoint(List<Models.WayPoint> Waypoint)
        {
            bool bResult = true;

            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    foreach (Models.WayPoint way in Waypoint)
                    {
                        string sqlSelectString = "select a.[id] from [dbo].[points] a where a.[point_code] = '" + way.Rp_Code.ToString() + "'";
                        command = new SqlCommand(sqlSelectString, conn);
                        command.Connection.Open();

                        int idpoint = 0;

                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            idpoint = Convert.ToInt16(reader[0].ToString());
                        }

                        command.Connection.Close();

                        if (idpoint != 0)
                        {
                            //UPDATE
                            string sqlUpdateString =
                                "UPDATE [dbo].[points] SET [point_description]=@point_description WHERE ID=@ID ";

                            command = new SqlCommand();
                            command.Connection = conn;
                            command.Connection.Open();
                            command.CommandText = sqlUpdateString;

                            SqlParameter point_descriptionparam = new SqlParameter("@point_description", way.Rp_Description.ToString());
                            SqlParameter IDparam = new SqlParameter("@ID", idpoint.ToString());

                            command.Parameters.AddRange(new SqlParameter[] { point_descriptionparam, IDparam });
                            command.ExecuteNonQuery();
                            command.Connection.Close();
                        }
                        else
                        {
                            sqlSelectString = "select MAX(a.[id]) from [dbo].[points] a";
                            command = new SqlCommand(sqlSelectString, conn);
                            command.Connection.Open();

                            int iidpoint = 0;

                            SqlDataReader reader2 = command.ExecuteReader();
                            if (reader2.Read())
                            {
                                iidpoint = Convert.ToInt16(reader2[0].ToString());
                            }
                            iidpoint++;

                            command.Connection.Close();

                            //INSERT
                            string sqlInsertString =
                                "INSERT INTO [dbo].[points] ([id], [point_code], [point_description]) VALUES (@ID, @point_code, @point_description)";

                            command = new SqlCommand();
                            command.Connection = conn;
                            command.Connection.Open();
                            command.CommandText = sqlInsertString;

                            SqlParameter point_codeparam = new SqlParameter("@point_code", way.Rp_Code.ToString());
                            SqlParameter point_descriptionparam = new SqlParameter("@point_description", way.Rp_Description.ToString());
                            SqlParameter IDparam = new SqlParameter("@ID", iidpoint.ToString());

                            command.Parameters.AddRange(new SqlParameter[] { IDparam, point_codeparam, point_descriptionparam });
                            command.ExecuteNonQuery();
                            command.Connection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                bResult = false;
                throw;
            }

            return bResult;
        }

        /// <summary>
        /// Method - Get list of all employees
        /// </summary>
        /// <param name="idroute"></param>
        /// <returns></returns>
        private void DelRoute(int idroute, bool bMessage = true)
        {
            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select a.[id] from [dbo].[routes] a where a.[id] = " + idroute.ToString();
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    idroute = 0;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idroute = Convert.ToInt16(reader[0].ToString());
                    }
                    else
                    {
                        if (bMessage)
                        {
                            errHandler.ErrorMessage = "There is no such route";
                        }
                    }
                    command.Connection.Close();

                    if (idroute != 0)
                    {
                        string sqlDeleteString = "delete from [dbo].[route_points] where [idroute]=@ID";

                        command = new SqlCommand()
                        {
                            Connection = conn
                        };
                        command.Connection.Open();
                        command.CommandText = sqlDeleteString;

                        SqlParameter IDparam = new SqlParameter("@ID", idroute);
                        command.Parameters.Add(IDparam);
                        command.ExecuteNonQuery();
                        command.Connection.Close();

                        sqlDeleteString = "delete from [dbo].[routes] where [id]=@ID";

                        command = new SqlCommand()
                        {
                            Connection = conn
                        };
                        command.Connection.Open();
                        command.CommandText = sqlDeleteString;

                        IDparam = new SqlParameter("@ID", idroute);
                        command.Parameters.Add(IDparam);
                        command.ExecuteNonQuery();
                        command.Connection.Close();

                        if (bMessage)
                        {
                            errHandler.ErrorMessage = "Route deleted";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }
        }

        /// <summary>
        /// Method - Get list of all employees
        /// </summary>
        /// <param name="idroute"></param>
        /// <returns>RouteList</returns>
        private RouteID GetUniqueRoute(int idroute)
        {
            RouteID route = new RouteID();

            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select a.[id], a.[route_code], a.[route_description] from [dbo].[routes] a where a.[id] = " + idroute.ToString();
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    route.idroute = 0;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        route.idroute = Convert.ToInt16(reader[0].ToString());
                        route.route_code = reader[1].ToString();
                        route.route_description = reader[2].ToString();
                    }
                    else
                    {
                        errHandler.ErrorMessage = "There is no such route";
                    }
                    command.Connection.Close();

                    if (route.idroute != 0)
                    {
                        List<WayPoint> WayPointsList = new List<WayPoint>();
                        WayPointsList = GetRouteWayPoints(route.idroute);

                        foreach (WayPoint wayPoint in WayPointsList)
                        {
                            route.waypoint.Add(wayPoint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }

            return route;
        }

        /// <summary>
        /// Method - Get list of all employees
        /// </summary>
        /// <param></param>
        /// <returns>RouteList</returns>
        private List<Route> GetAllRoutes()
        {
            List<Route> RouteList = new List<Route>();

            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select a.[id], a.[route_code], a.[route_description] from [dbo].[routes] a order by a.[id]";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Route route = new Route
                        {
                            idroute = Convert.ToInt16(reader[0].ToString()),
                            route_code = reader[1].ToString(),
                            route_description = reader[2].ToString()
                        };
                        RouteList.Add(route);
                    }
                    command.Connection.Close();

                    if (RouteList.Count != 0)
                    {
                        foreach (Route route in RouteList)
                        {
                            if (route.idroute != 0)
                            {
                                List<WayPoint> WayPointsList = new List<WayPoint>();
                                WayPointsList = GetRouteWayPoints(route.idroute);

                                foreach (WayPoint wayPoint in WayPointsList)
                                {
                                    route.waypoint.Add(wayPoint);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }

            return RouteList;
        }

        /// <summary>
        /// Method - Get list of route WayPoints
        /// </summary>
        /// <param name="idroute"></param>
        /// <returns>WayPointsList</returns>
        private List<WayPoint> GetRouteWayPoints(int idroute)
        {
            List<WayPoint> WayPointsList = new List<WayPoint>();

            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "SELECT b.[point_code], c.[point_code], a.[rp_cost], a.[rp_time] from [dbo].[route_points] a, [dbo].[points] b, [dbo].[points] c where [idroute] = " + idroute.ToString() + " and b.id = a.fromidpoint and c.id = a.toidpoint order by a.[id]";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        WayPoint wayPoint = new WayPoint
                        {
                            fromidpoint = reader[0].ToString(),
                            toidpoint = reader[1].ToString(),
                            rp_cost = Convert.ToInt16(reader[2].ToString()),
                            rp_time = Convert.ToInt16(reader[3].ToString())
                        };
                        WayPointsList.Add(wayPoint);
                    }
                    command.Connection.Close();
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }

            return WayPointsList;
        }

        #endregion
    }
}
