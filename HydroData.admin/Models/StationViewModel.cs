using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Public.Models
{
    public class StationViewModel
    {
        public StationViewModel()
        {
            Stations = new List<station>();
            StartDate = DateTime.Now.AddDays(-1).ToUniversalTime().Date;
            EndDate = DateTime.Now.ToUniversalTime().Date;

        }
		public int StatId { get; set; }
		public string sturi { get; set; }
		public station Station { get; set; }
        public int VarId { get; set; }
		public string VarName { get; set; }
		public string VarPageName { get; set; }


        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public SelectList Vars { get; set; }
        public List<station> Stations { get; set; }
        public SelectList SelectListStations { get; set; }

        public string UrlTempl { get; set; }

    }
}