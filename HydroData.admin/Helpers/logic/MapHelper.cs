using Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Helpers
{
	public static class MapHelper
	{
		public static List<station> FilterByPeriod(this IEnumerable<station> stats, int? period)
		{
			var now = Helper.CurrentDate;
			var result = new List<station>();

			foreach (var st in stats)
			{
				DateTime? max = st.LastObservDate;

				var period_ = period.GetValueOrDefault(-1);

				bool incl = false;
				if (period_ == 1 && max >= now) incl = true;
				if (period_ == 7 && max >= now.AddDays(-7) && max < now) incl = true;
				if (period_ == 30 && max >= now.AddDays(-30) && max < now.AddDays(-7)) incl = true;
				if (period_ == 60 && max < now.AddDays(-30)) incl = true;


				if (period_ == -1 || incl)
				{
					//st.LastObservTName = obstname;
					result.Add(st);
				}
			}
			return result;
		}

	}
}