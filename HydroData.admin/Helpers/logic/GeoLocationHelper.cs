using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace GeoApi
{
    public class GeoLocationHelper
    {


        public static results[] FindByAddress(string addr)
        {
            var address = "http://maps.google.com/maps/api/geocode/json?address={0}&sensor=false";
            
            var client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;

            var result = client.DownloadString(string.Format(address, addr));
            //var jss = new JavaScriptSerializer();
            //return jss.Deserialize<dynamic>(result);

            var res = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(result);
            return res.results;


        }


    }

    public class GoogleGeoCodeResponse
    {

        public string status { get; set; }
        public results[] results { get; set; }

    }

    public class results
    {
        public string formatted_address { get; set; }
        public geometry geometry { get; set; }
        public string[] types { get; set; }
        public address_component[] address_components { get; set; }
    }

    public class geometry
    {
        public string location_type { get; set; }
        public location location { get; set; }
    }

    public class location
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }

    public class address_component
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public string[] types { get; set; }
    }
}

