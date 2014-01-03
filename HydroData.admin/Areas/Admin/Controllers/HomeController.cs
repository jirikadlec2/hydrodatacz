using Data;
using HydroData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index(string[] vars)
		{
			ViewBag.vars = new SelectList(Helper.VarsTable.Select(x =>
			new { TableName = x.TableName, PageName = T.Text(x.PageName) }), "TableName", "PageName");

			ViewBag.CacheItems = string.Join("<br />", Repo.CacheHits.Select(x => x.Key + ":" + x.Value));

			if (!string.IsNullOrEmpty(Request.Params["update"]))
			{

				var sw = Stopwatch.StartNew();
				foreach (var item in vars)
					Repo.UpdateLastObservationDates(Helper.FindVarByName(item).Id);
				ViewBag.elapsed = string.Format("[update] = {0} ms", sw.ElapsedMilliseconds);
			}
			return View();
		}
		public ActionResult ClearCache()
		{
			Repo.InvalidateByPattern("");
			Repo.CacheHits.Clear();
			return RedirectToAction("Index");
		}
		public ActionResult About()
		{
			return View();
		}
		public ActionResult Chart()
		{
			return View();
		}

		public ActionResult autocomplete()
		{
			var river = Repo._GetRivers();
			ViewBag.river1 = new SelectList(river, "riv_id", "riv_name");

			return View();
		}
	}
}
