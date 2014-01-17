using Data;
using HydroData.Public.Models;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HydroData.Public.Controllers
{
    public class BaseController : Controller
    {
        protected Database db = Repo.HydroData;

        protected int GetVarIdByName(string varname)
        {
            if (!string.IsNullOrEmpty(varname))
            {
                var variable = Helper.FindVarByName(varname);
                if (variable != null) return variable.Id;
            }
            return 0;
        }
        protected void InitModel(string varname, StationViewModel model)
        {
            //read var cookie
            if (string.IsNullOrWhiteSpace(varname)) varname = ReadCookie("sel_var");
            else { SetCookie("sel_var", varname); }

            var varid = GetVarIdByName(varname);

            if (varid != 0)
            {
                model.VarId = varid;
                model.VarName = Helper.Vars[model.VarId].TableName;
                model.VarPageName = Helper.Vars[model.VarId].PageName;
            }
            model.StartDate = Helper.CurrentDate.AddYears(-1);
            model.EndDate = Helper.CurrentDate;

            model.Vars = new SelectList(Helper.VarsTable.Select(x =>
                new { TableName = x.TableName, PageName = T.Text(x.PageName) }), "TableName", "PageName", model.VarName);

        }

        protected StationViewModel CreateAndInitModel(string varname, string sturi)
        {
            var model = new StationViewModel();

            //read station cookies
            if (string.IsNullOrWhiteSpace(sturi)) sturi = ReadCookie("sel_station");
            else { SetCookie("sel_station", sturi); }

            InitModel(varname, model);

            var stats = Repo.GetStationsWithObservDates(model.VarId, true);
            //sturi = sturi.Replace('-', '/');
            model.Station = stats.FirstOrDefault(x => x.UrlParm == sturi);
            if (model.Station != null)
            {
                model.sturi = model.Station.st_uri;
                model.StatId = model.Station.st_id;
            }

            model.SelectListStations = new SelectList(stats, "UrlParm", "st_name", sturi);

            return model;
        }

        protected StationViewModel InitModel(int? VarId, int? StatId)
        {
            var model = new StationViewModel();

            model.VarId = VarId.GetValueOrDefault(0);
            model.Vars = new SelectList(Helper.VarsTable.Select(x =>
                new { Id = x.Id, PageName = T.Text(x.PageName) }), "Id", "PageName", model.VarId);
            if (model.VarId != 0) model.VarName = Helper.Vars[model.VarId].PageName;

            model.StatId = StatId.GetValueOrDefault(0);

            //if (VarId.HasValue)
            {
                var stats = Repo.GetStationsWithObservDates(model.VarId, true);

                model.Stations = stats;
                model.Station = stats.FirstOrDefault(x => x.st_id == StatId);

                model.SelectListStations = new SelectList(stats, "st_id", "st_name", model.StatId);
            }
            return model;
        }

        protected string GetUrlLang()
        {
            string lang = "en";
            var routes = RouteTable.Routes;

            var httpContext = Request.RequestContext.HttpContext;
            if (httpContext == null) return lang;

            var routeData = routes.GetRouteData(httpContext);
            var l = routeData.Values["language"] as string;
            if (!string.IsNullOrEmpty(l)) return l;
            return lang;
        }

        public string ReadCookie(string key)
        {
            var cookie = Request.Cookies[key];
            return cookie != null ? cookie.Value : "";
        }

        public void SetCookie(string key, string value)
        {
            var cookie = new HttpCookie(key, value);
            cookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(cookie);
        }
    }
}
