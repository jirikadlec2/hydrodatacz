using System.Web.Mvc;
using MvcMembership;

[assembly: WebActivator.PreApplicationStartMethod(typeof(HydroData.admin.App_Start.MvcMembership), "Start")]

namespace HydroData.admin.App_Start
{
	public static class MvcMembership
	{
		public static void Start()
		{
			GlobalFilters.Filters.Add(new TouchUserOnEachVisitFilter());
		}
	}
}
