using Data;
using HydroData.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
    public class VarsController : BaseController
    {

        public ActionResult Index(int? pg, int? varId, int? StatId, string date)
        {

            var act = Request.Params["act"];

            if (!string.IsNullOrWhiteSpace(date)) { pg = 1; }
            var _pg = pg.GetValueOrDefault(1);

			var model = InitModelWithVarAndStation(varId, StatId);

            if (StatId.HasValue)
                LoadData(date, act, _pg, model);

            model.UrlTempl = string.Format("/Vars/Index?varid={0}&statid={1}", model.VarId, model.StatId) + "&pg={0}";

            TempData["model"] = model;
            return View(model);
        }

		
        public ActionResult Details(int? varId, int? StatId, string date)
        {
			var model = InitModelWithVarAndStation(varId, StatId);

            if (StatId.HasValue)
                LoadData(date, "", 1, model);

            return PartialView("_Details", model);

        }
        private void LoadData(string date, string act, int _pg, ChartViewModel model)
        {
            var sql = DBHelper.SQLGetValByVarAndStat(model.VarId, model.StatId);

            DateTime d;
            if (DateTime.TryParse(date, out d))
            {

                model.Date = d;
                if (model.VarId == 8 || model.VarId == 2)
                    sql = sql.Where(" time_utc>=@0 and time_utc<@1", d, d.AddMonths(1));
                else sql = sql.Where(" time_utc>=@0 and time_utc<@1", d, d.AddDays(1));
            }

            sql = sql.OrderBy("time_utc asc");

            model.Page = db.Page<TableValue>(_pg, Helper.PageSize, sql);
        }

   

        public string AddValue(int varid, short sid, string date, int hour, double val, int vacc)
        {
            try
            {
                var time = Convert.ToDateTime(date);
                if (hour >= 0 && hour < 25) time = time.AddHours(hour);
                
                string valRes = CheckVal(varid, val);
                if (!string.IsNullOrEmpty(valRes)) return valRes;

                return DBHelper.AddVarValue(varid, sid, time, val, vacc);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string EditValue(int varid, short sid, DateTime time, float val, int vacc, byte qid)
        {
            try
            {
                string valRes = CheckVal(varid, val);
                if (!string.IsNullOrEmpty(valRes)) return valRes;

                return DBHelper.UpdateVarValue(varid, sid, time, val, vacc, qid);

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private static string CheckVal(int varid, double val)
        {
            //temperature
            if (varid == 16 && (val < -900 || val > 600)) return "Temperature must be between -90 and +60";
            //snow
            if (varid == 8 && (val <= 0 || val > 1000)) return "Snow must be between 0 and 1000";
            //Discharge
            if (varid == 5 && (val <= 0)) return "Discharge must be > 0";
            if (varid == 4 && (val <= 0)) return "Stage must be > 0";
            if (varid == 2 && (val < 0)) return "Rain must be >= 0";
            if (varid == 1 && (val < 0)) return "Rain must be >= 0";

            return "";
        }

        public string DelValue(int varid, short sid, DateTime time)
        {
            try
            {
                var tName = Helper.Vars[varid].TableName;
                db.Execute("delete from " + tName + " where time_utc=@0 and station_id=@1", time, sid);
                return "deleted successfull";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        const string UPD_SUCCESS = "updated successfull";
      
        const string NOT_EXIST = "don't exist";
       

    }
}
