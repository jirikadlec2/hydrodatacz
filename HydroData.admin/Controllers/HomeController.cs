using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Public.Controllers
{
	[UrlLang]
    public class HomeController : Controller
    {
        //
        // GET: /Public/Start/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

		public ActionResult SetCulture(string id)
		{

			HttpCookie userCookie = Request.Cookies["Culture"];
			userCookie.Value = id;
			userCookie.Expires = DateTime.Now.AddYears(1);
			Response.SetCookie(userCookie);
			LangHelper.Locale = id;
			var lang = RouteData.Values["Language"];
			return Redirect(Request.UrlReferrer.ToString());
		}
    }
}
