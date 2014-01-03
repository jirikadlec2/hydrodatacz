using Data;
using HydroData.admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
    public class RadarController : BaseController
    {
        //
        // GET: /Radar/

        public ActionResult Index(int? pg, int? rnid)
        {

            var p = pg.GetValueOrDefault(1);

            var model = new RadarViewModel();

            var rn = db.Fetch<radarnetwork>("");
            model.RadarNets = new SelectList(rn, "radarnet_id", "radarnet_name", 1);

            model.UrlTempl = "/Radar/Index?pg={0}";

            var sql = PetaPoco.Sql.Builder
               .Append("SELECT * FROM radarfiles");

            if (rnid.HasValue)
            {
                model.RNID = rnid.Value;
                model.UrlTempl = "/Radar/Index?rnid=" + rnid.Value + "&pg={0}";
                sql = sql.Where("radnet_id=@0", rnid.Value);
            }


            model.Page = db.Page<radarfile>(p, Helper.PageSize, sql);
            return View(model);
        }

    }
}
