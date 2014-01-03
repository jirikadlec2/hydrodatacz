using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HydroData
{
	public class UrlLangAttribute : FilterAttribute, IActionFilter
	{
		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var routes = RouteTable.Routes;

			var httpContext = filterContext.HttpContext;
			if (httpContext == null) return;

			var routeData = routes.GetRouteData(httpContext);
			var lang = routeData != null ?
				routeData.Values["language"] as string
				: "";

			var cultureInfo = new CultureInfo(LangHelper.LangToCulture(lang));

			System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{

		}



	}
}
