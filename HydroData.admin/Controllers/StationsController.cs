using Data;
using HydroData.Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Public.Controllers
{
	[UrlLang]
	public class StationsController : BaseController
	{
		//
		// GET: /Public/Stations/

		public ActionResult Index(string varname)
		{
			var model = new StationViewModel();
			InitModel(varname, model);
			model.Stations = Repo.GetStationsWithObservDates(model.VarId, true);

			return View(model);
		}


	}
}
