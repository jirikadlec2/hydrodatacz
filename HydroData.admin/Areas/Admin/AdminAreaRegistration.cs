using System.Web.Mvc;

namespace HydroData.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
		{
			string lang = LangHelper.SiteLang;

			context.MapRoute(
				name: "Admin_default",
				url: "{language}/admin/{controller}/{action}/{id}",
				defaults: new
				{
					language = lang,
					controller = "Home",
					action = "Index",
					id = UrlParameter.Optional
				},
				namespaces: new string[] { "HydroData.Areas.Admin.Controllers" },
				constraints: new { language = "en|cz" }

			 );
			//context.MapRoute(
			//	"Admin_default",
			//	"Admin/{controller}/{action}/{id}",
			//	new {Language="en", Controller="Home", action = "Index", id = UrlParameter.Optional }
			//);
        }
    }
}
