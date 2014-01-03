using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
	public class LogController : BaseController
	{
		//
		// GET: /Log/

		public ActionResult Index(int? pg)
		{
			var p = db.Page<logging>(pg.GetValueOrDefault(1), 20,
						   "select * from logging order by time_utc desc");

			ViewBag.PagingUrl = "/Log/Index?pg={0}";

			return View(p);

		}
		public ActionResult View(long? time)
		{
			var dt = new DateTime(time.Value);
			var p = db.FirstOrDefault<logging>(
						   "select * from logging where time_utc=@0", dt);
			if (p != null)
				ViewBag.Text = p.log_text;
			else ViewBag.Text = "didn't find";

			return View();
		}

	}
}
