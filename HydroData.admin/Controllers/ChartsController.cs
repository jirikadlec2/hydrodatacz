using Data;
using HydroData.admin.Models;
using HydroData.Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HydroData.Public.Controllers
{
	[UrlLang]
	public class ChartsController : BaseController
	{

		public ActionResult Index(string varname, string sturi, DateTime? startdate, DateTime? enddate)
		{

			var model = CreateAndInitModel(varname, sturi);

			if (startdate.HasValue) model.StartDate = startdate.Value;
			if (enddate.HasValue) model.EndDate = enddate.Value;

			if (model.VarId > 0 && !string.IsNullOrEmpty(model.sturi)
				&& model.StartDate.HasValue && model.EndDate.HasValue)
			{
				var lang = GetUrlLang();

				var var2 = Helper.GraphVars[model.VarId];
				var graphName = string.Format("{4}-{0}-{1}-{2}-{3}.ashx",
					var2, model.StatId,
					model.StartDate.Value.ToString("yyyyMMdd")
					, model.EndDate.Value.ToString("yyyyMMdd"), lang);
				ViewBag.GraphName = graphName;

				InitStats(model);
				CalcAverage(model);
			}

			return View(model);
		}

		private void CalcAverage(StationViewModel model)
		{
			var varid = model.VarId;
			if (varid <= 0) return;

			var tname = Helper.Vars[varid].TableName;
			var cname = Helper.Vars[varid].ColumnName;
			string expr1 = "";
			string calcTitle = "";

			if (varid == 8)
			{
				expr1 = "count(*)";
				calcTitle = "number of snow days";
			}
			else if (varid == 1 || varid == 2)
			{
				expr1 = string.Format("sum({0})", cname);
				calcTitle = "Sum";
			}
			else
			{
				expr1 = string.Format("avg({0})", cname);
				calcTitle = "Average";
			}

			var sql = string.Format(@"SELECT  {0} FROM {1} where station_id=@0 
			and time_utc > @1 and time_utc < @2 and {2} > 0", expr1, tname, cname);

			var sqlb = PetaPoco.Sql.Builder.Append(sql, model.StatId, model.StartDate, model.EndDate);
			var res = db.ExecuteScalar<float>(sqlb);

			ViewBag.calc = TableValue.ModifyValue(varid, res);
			ViewBag.calcTitle = calcTitle;
		}



		private void InitStats(StationViewModel model)
		{
			if (model.VarId <= 0) return;
			var cname = Helper.Vars[model.VarId].ColumnName;

			var sql = DBHelper.SQLGetValByVarAndStat(model.VarId, model.StatId);
			sql = sql.Where(" time_utc>=@0 and time_utc<=@1", model.StartDate, model.EndDate);
			sql = sql.Where(string.Format("{0} >0", cname));

			var list = db.Query<TableValue>(sql);
			if (list.Any())
			{
				var min = list.Min(x => x.Val);
				var max = list.Max(x => x.Val);

				ViewBag.minv = TableValue.ModifyValue(model.VarId, min);
				ViewBag.mindate = list.First(x => x.Val == min).DateTime.Date;

				ViewBag.maxv = TableValue.ModifyValue(model.VarId, max); ;
				ViewBag.maxdate = list.First(x => x.Val == max).DateTime.Date;

				ViewBag.MeasureUnit = Helper.Vars[model.VarId].MeasureUnit;
			}
		}



	}
}
