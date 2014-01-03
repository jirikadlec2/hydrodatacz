using Data;
using GeoApi;
using Helpers;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
    public class MapController : BaseController
    {
        //
        // GET: /Map/

        public ActionResult Index(int? varid, int? Operator, int? period, int? pg)
        {


            var opers = Repo.HydroData.Fetch<@operator>("");

            ViewBag.Operator = new SelectList(opers, "id", "name", Operator);

            ViewBag.Vars = new SelectList(Helper.VarsTable.Select(x =>
                new { Id = x.Id, PageName = T.Text(x.PageName) }), "Id", "PageName", varid);

            var periods = new Dictionary<int, string>{
                {-1,T.Text("all stations")},
                {1, string.Format(T.Text("{0} day old"),"<1")},
                {7, string.Format(T.Text("{0} days old"),"1-7")},
                {30,string.Format(T.Text("{0} days old"),"7-30")},
                {60,string.Format(T.Text("{0} days old"),">30")},
            };

            ViewBag.period = new SelectList(periods, "Key", "Value");

            ViewBag.varid = varid;


            //chached db query
            var sw = Stopwatch.StartNew();

            IEnumerable<station> stats = Repo.GetStationsWithObservDates(varid.GetValueOrDefault(0));

            if (Operator.HasValue)
                stats = stats.Where(x => x.operator_id == Operator);
            stats = stats.Where(x => x.lat.HasValue);
            ViewBag.elapsed = string.Format("[GetStations] = {0} ms", sw.ElapsedMilliseconds);

            var obsInfo = "";

            if (!string.IsNullOrEmpty(Request.Params["show"]))
            {
				//return View(stats.ToList());
            }

            if (!string.IsNullOrEmpty(Request.Params["findbyaddr"]))
            {
                var addr = Request.Params["address"];
                if (!string.IsNullOrWhiteSpace(addr))
                {
                    var loc = FindbyAddress(addr);
                    if (loc != null)
                    {
                        ViewBag.Place = loc;
                    }
                }
            }

			return View(stats.FilterByPeriod(period));
        }

        private string[] FindbyAddress(string addr)
        {
            var res = GeoLocationHelper.FindByAddress(addr);
            var loc = new[] { "", "" };
            var ff = res.FirstOrDefault();
            ViewBag.findresult = string.Join("<br />", res.Select(x => x.formatted_address));

            if (ff != null)
                return new[] { ff.geometry.location.lat, ff.geometry.location.lng };
            else return null;

        }



    }
}
