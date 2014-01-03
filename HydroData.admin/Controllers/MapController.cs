using Data;
using GeoApi;
using Helpers;
using HydroData.Public.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Public.Controllers
{

	[UrlLang]
	public class MapController : BaseController
	{
		//
		// GET: /Map/

		public ActionResult Index(string varname, string address)
		{
			var model = new StationViewModel();
			if (string.IsNullOrEmpty(varname)) varname = Helper.GetRouteVar(4);
			InitModel(varname, model);

			//chached db query
			var sw = Stopwatch.StartNew();
			IEnumerable<station> stats = Repo.GetStationsWithObservDates(model.VarId, true);
			stats = stats.Where(x => x.lat.HasValue);

			ViewBag.elapsed = string.Format("[GetStations] = {0} ms", sw.ElapsedMilliseconds);

			string obsInfo = "";

			if (!string.IsNullOrEmpty(Request.Params["show"]))
			{
				//return View(stats.ToList());
			}

			if (!string.IsNullOrEmpty(address))
			{
				var loc = FindbyAddress(address);
				if (loc != null)
				{
					ViewBag.Place = loc;
				}
			}

			model.Stations = stats.ToList();
			return View(model);
		}


		public ActionResult FindByAddress(string varname, string address)
		{
			return RedirectToAction("Index", new { varname = varname, address = address });
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
