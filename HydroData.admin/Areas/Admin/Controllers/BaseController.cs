using Data;
using HydroData.admin.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
	[UrlLang]
	[Authorize(Roles = "Administrator")]
    public class BaseController : Controller
    {
		protected Database db = Repo.HydroData;

		protected ChartViewModel InitModelWithVarAndStation(int? varId, int? StatId)
		{
			var model = new ChartViewModel();

			model.VarId = varId.GetValueOrDefault(16);
			model.VarName = Helper.Vars[model.VarId].PageName;
			model.Vars = new SelectList(Helper.VarsTable.Select(x =>
			   new { Id = x.Id, PageName = T.Text(x.PageName) }), "Id", "PageName", model.VarId);

			model.StatId = StatId.GetValueOrDefault(0);
			var st = Repo.GetStations(model.VarId);
			model.Stations = new SelectList(st, "st_id", "st_name", model.StatId);

			model.Date = DateTime.Now;

			return model;
		}


		public DateTime ParseDate(string date)
		{
			DateTime d;
			if (string.IsNullOrWhiteSpace(date))
				d = DateTime.Now.ToUniversalTime();
			else
			{
				DateTime.TryParse(date, out d);
			}
			return d;
		}

    }
}
