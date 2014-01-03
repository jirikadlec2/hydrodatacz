using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HydroData
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//routes.MapRoute(
			//	"Default", // Route name
			//	"{controller}/{action}/{id}", // URL with parameters
			//	new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			//	, new string[] { "HydroData.Public.Controllers" }
			//);

			string lang = "cz";//LangHelper.SiteLang;

			routes.MapRoute(
			   name: "Default_variables",
			   url: "{language}/{controller}/{varname}/{sturi}",
			   defaults: new
			   {
				   language = lang,
				   action = "Index",
				   varname = UrlParameter.Optional,
				   sturi = UrlParameter.Optional,
			   },
			   namespaces: new string[] { "HydroData.Public.Controllers" },
			   constraints: new { 
				   language = "en|cz", 
				   controller="Stations|Charts|Map",
				   varname = "^(?!FindByAddress$).*$"
			   }

			);
			routes.MapRoute(
				name: "Default_simple",
				url: "{language}/{controller}/{action}/{id}",
				defaults: new
				{
					language = lang,
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional
				},
				namespaces: new string[] { "HydroData.Public.Controllers" },
				constraints: new { language = "en|cz" }

			 );

		}

		protected void Application_BeginRequest()
		{
			//Thread.CurrentThread.CurrentCulture = new CultureInfo("cs-CZ");


		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}

		protected void Application_AcquireRequestState()
		{
			//var routes = RouteTable.Routes;

			//var httpContext = Request.RequestContext.HttpContext;
			//if (httpContext == null) return;

			//string lang = "en";

			//var routeData = routes.GetRouteData(httpContext);
			//if (routeData != null)
			//	lang = routeData.Values["language"] as string;
			//if ((lang == "en" || lang == "cz"))
			//{
			//	var cultureInfo = new CultureInfo(Helper.LangToCulture[lang]);

			//	System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
			//	System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
			//}
		}

	}
}