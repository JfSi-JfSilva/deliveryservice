using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestService.Models
{
    public class Way
    {
        public string Frompoint { get; set; }
        public string Topoint { get; set; }
        public string Sort_by { get; set; }
        public string Path { get; set; }
        public string Err_message { get; set; }
    }
}