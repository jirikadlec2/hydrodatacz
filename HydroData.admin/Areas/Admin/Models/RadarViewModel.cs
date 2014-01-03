using Data;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.admin.Models
{
    public class RadarViewModel
    {
        public int RNID { get; set; }
        public DateTime? Date { get; set; }
        public SelectList RadarNets { get; set; }
        public string UrlTempl { get; set; }

        public Page<radarfile> Page { get; set; }
    }
}