using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestService.Models
{
    public class WayPoint
    {
        public string Rp_Code { get; set; }
        public string Rp_Description { get; set; }
        public int Rp_cost { get; set; }
        public int Rp_time { get; set; }
    }

    public class Route
    {
        public string Route_code { get; set; }
        public string Route_description { get; set; }
        public string Str_key { get; set; }
        public List<WayPoint> Waypoint { get; set; }       
        public string Err_message { get; set; }
        public Route()
        {
            Waypoint = new List<WayPoint>();
        }
    }
}