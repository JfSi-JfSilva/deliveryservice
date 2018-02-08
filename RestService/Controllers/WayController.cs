using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace RestService.Controllers
{
    public class WayController : ApiController
    {
        #region Private Members

        private string Host = "localhost";
        private SqlConnection conn;
        private SqlCommand command;
        private ConnectionStringSettings connString;
        private ErrorHandler.ErrorHandler errHandler;

        #endregion

        #region Main Functions

        // POST: api/Way
        // json body: {"frompoint":"A", "topoint": "B", "sort_by": "cost"}
        [HttpPost]
        public string Post([FromBody] Models.Way way)
        {
            errHandler = new ErrorHandler.ErrorHandler();
            bool bContinue = true;

            try
            {
                if (string.IsNullOrEmpty(way.Frompoint))
                {
                    errHandler.ErrorMessage = "Frompoint Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (way.Frompoint.ToString().Trim().Length >= 32)
                    {
                        errHandler.ErrorMessage = "frompoint Field length > 32";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(way.Topoint))
                {
                    errHandler.ErrorMessage = "Topoint Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if (way.Topoint.ToString().Trim().Length >= 32)
                    {
                        errHandler.ErrorMessage = "topoint Field length > 32";
                        bContinue = false;
                    }
                }

                if (string.IsNullOrEmpty(way.Sort_by))
                {
                    errHandler.ErrorMessage = "Sort_by Field is requiered";
                    bContinue = false;
                }
                else
                {
                    if ( !(way.Sort_by.ToString().Trim().Equals("time") || way.Sort_by.ToString().Trim().Equals("cost")) )
                    {
                        errHandler.ErrorMessage = "sort_by values allowed are time and cost";
                        bContinue = false;
                    }
                }

                if (bContinue)
                {
                    GetConectionString();

                    way = GetWay(way);
                }
            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
            }

            way.Err_message = errHandler.ErrorMessage;

            return JsonConvert.SerializeObject(way);
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
        /// Database SELECT - To get best way between 2 point using time or cost
        /// Uses recursive function: GetWay
        /// </summary>
        /// <param name="way"></param>
        /// <returns name="way">></returns>
        private Models.Way GetWay(Models.Way way)
        {
            try
            {
                using (conn)
                {
                    conn = new SqlConnection(connString.ConnectionString);

                    string sqlSelectString = "select TOP 1 w_path from[dbo].GetWay('" + way.Frompoint.ToString() + "', '" + way.Topoint.ToString() + "', '" + way.Sort_by + "') order by rp_value";
                    command = new SqlCommand(sqlSelectString, conn);
                    command.Connection.Open();

                    way.Path = "";

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        way.Path = reader[0].ToString();
                    }
                    else
                    {
                        errHandler.ErrorMessage = "Could not find way between those points";
                    }

                    command.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                errHandler.ErrorMessage = ex.Message.ToString();
                throw;
            }

            return way;
        }

        #endregion
    }
}
