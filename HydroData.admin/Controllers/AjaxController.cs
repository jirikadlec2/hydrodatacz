
using Data;
using HydroData.Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Controllers
{
    public class AjaxController : BaseController
    {
		public ActionResult GetStations(string varname)
		{
			var varid = GetVarIdByName(varname);

			var st = Repo.GetStations(varid, true);

			return Json(
			   st.Select(x => new { value = x.UrlParm, text = x.st_name }),
			   JsonRequestBehavior.AllowGet
		   );
		}

    }
}
