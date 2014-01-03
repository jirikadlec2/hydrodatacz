using Data;
using HydroData.admin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
    public class ChartsController : BaseController
    {

        public ActionResult Index(int? pg, int? varId, int? StatId, string date, HttpPostedFileBase file)
        {
            var act = Request.Params["act"];

            if (StatId.HasValue)
            {
                if (!string.IsNullOrEmpty(Request.Params["download"]))
                    return DownloadData(varId.Value, StatId.Value);

                if (!string.IsNullOrEmpty(Request.Params["upload"]))
                    return UploadData(varId.Value, StatId.Value);
            }

            if (!string.IsNullOrWhiteSpace(date)) { pg = 1; }

			var model = InitModelWithVarAndStation(varId, StatId);

            if (StatId.HasValue)
                LoadData(date, act, pg.GetValueOrDefault(1), model);

            //TempData["model"] = model;

            return View(model);
        }


        private void LoadData(string date, string act, int pg, ChartViewModel model)
        {
            var sql = DBHelper.SQLGetValByVarAndStat(model.VarId, model.StatId);
			var d = ParseDate(date);
            d = new DateTime(d.Year, d.Month, 1);
            if (act == "prev") d = d.AddMonths(-1);
            if (act == "next") d = d.AddMonths(1);
            model.Date = d;
            sql = sql.Where(" time_utc>=@0 and time_utc<@1", d, d.AddMonths(1));

            sql = sql.OrderBy("time_utc asc");

            model.Items = db.Page<TableValue>(pg, 3000, sql).Items;
        }

        public ActionResult Table(int? varId, int? StatId, string date)
        {
            var act = Request.Params["act"];

			var model = InitModelWithVarAndStation(varId, StatId);

            if (StatId.HasValue)
                LoadData(date, act, 1, model);

            //return PartialView(model.Page.Items);
            return View(model);
        }

        public ActionResult Search(int? varId, int? StatId, string date)
        {
            var model = new ChartViewModel();

            model.VarId = varId.GetValueOrDefault(16);
            model.StatId = StatId.GetValueOrDefault(0);
            var act = Request.Params["act"];
            if (StatId.HasValue)
                LoadData(date, act, 1, model);

            return PartialView("_partChartTable", model);

        }

        public ActionResult GetStations(int varId)
        {
            var st = Repo.GetStations(varId);

            return Json(
               st.Select(x => new { value = x.st_id, text = x.st_name }),
               JsonRequestBehavior.AllowGet
           );
        }

        public ActionResult DownloadData(int varId, int StatId)
        {
            var st = Repo.GetStation(StatId);
            var varName = Helper.Vars[varId].PageName;

            var csv = string.Join(Environment.NewLine, CSVHelper.GenValuesCSV(varId, StatId));

            var byteArray = Encoding.ASCII.GetBytes(csv);
            var stream = new MemoryStream(byteArray);

            var fname = string.Format("{0}_{1}.csv", varName, st.st_name);
            return File(stream, "text/plain", fname);
        }



        public ActionResult UploadData(int varId, int sid)
        {
            string ftext = "";
            TempData["varid"] = varId;
            TempData["var_name"] = Helper.Vars[varId].PageName;
            TempData["statid"] = sid;
            TempData["stat_name"] = Repo.GetStations(varId).First(x => x.st_id == sid).st_name;
            foreach (string f in Request.Files.Keys)
            {
                if (Request.Files[f].ContentLength > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        Request.Files[f].InputStream.CopyTo(ms);
                        byte[] buf = ms.GetBuffer();
                        ftext = Encoding.UTF8.GetString(buf, 0, buf.Length);
                        int err;
                        TempData["uploadfile"] = string.Format("size(bytes)={0}, lines={1}, wrong lines={2} ", ftext.Length,
                            CSVHelper.ParseValuesCSV(ftext, out err).Count, err);
                        Session["uploadfile"] = ftext;
                    }
                }
            }
            return View("_UploadResult", true);
        }

        public ActionResult ProcessFile(int? varid, int? sid, bool? overwrite)
        {
            var ftext = (string)Session["uploadfile"];
            string resStr;
            TempData["varid"] = varid;
            TempData["statid"] = sid;

            if (!varid.HasValue || !sid.HasValue)
                ViewBag.result = T.Text("text.var or station isn't selected");

            else if (CSVHelper.ProcessData(varid.Value, sid.Value, ftext, overwrite.GetValueOrDefault(false), out resStr))
                ViewBag.result = string.Format("<SPAN style=\"color:green\">{0}</SPAN>", T.Text("successfull") + ", " + resStr);
            else
                ViewBag.result = string.Format("<SPAN style=\"color:red\">{0}</SPAN>", T.Text("failed") + ", " + resStr);

            return View("_UploadResult", false);
        }


    }
}
