using Data;
using HydroData.Data;
using PetaPoco;
using PetaPoco.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Data
{
	public class Repo
	{
		public static Database HydroData { get { return new Database("DataContext"); } }

		public static ICacheProvider Cache = new DefaultCacheProvider();
		public static Dictionary<string, int> CacheHits = new Dictionary<string, int>();
		const int cacheTime = 30;

		public static void InvalidateByPattern(string pattern)
		{
			Cache.InvalidateByPattern(pattern);

			var keys = 	CacheHits.Select(x=>x.Key).ToList();
			foreach (var key in keys)
            {
				if (key.StartsWith(pattern)) CacheHits.Remove(key);
            }
	
		}

		public static List<station> GetStations(int varId = 0, bool isPublic = false)
		{
			var key = "cache_stations";

			var data = Cache.Get(key) as List<station>;
			var vars = GetStationVariables();

			UpdateCacheInfo(key, data == null);

			if (data == null)
			{
				var sql = @"select s.*,l.*,r.*,o.* from stations s
left join locations l on s.location_id = l.loc_id
left join river r on s.riv_id = r.riv_id
left join operator o on s.operator_id = o.id
order by s.st_name";

				data = HydroData.Fetch<station, location, river, @operator, station>(
			   (s, l, r, o) => { s.Location = l; s.River = r; s.Operator = o; return s; }
			   , sql);

				if (data.Any())
				{
					Cache.Set(key, data, cacheTime);
				}
			}

			return FilterStationsBy(varId, isPublic, data, vars).ToList();
		}

		public static List<stationsvariable> GetStationVariables()
		{
			var key = "cache_stationsvariable";

			var data = Cache.Get(key) as List<stationsvariable>;
			UpdateCacheInfo(key, data == null);

			if (data == null)
			{
				data = HydroData.Fetch<stationsvariable>("");
				if (data.Any()) Cache.Set(key, data, cacheTime);
			}

			return data;
		}

		public static Dictionary<string, SiteBigText> GetSiteBigTexts()
		{
			var key = "cache_SiteBigTexts";

			var data = Cache.Get(key) as Dictionary<string, SiteBigText>;
			UpdateCacheInfo(key, data == null);
			if (data == null)
			{
				data = HydroData.Fetch<SiteBigText>("").ToDictionary(x => x.TextId, x => x);
				if (data.Any()) Cache.Set(key, data, cacheTime);
			}
			return data;
		}
		public static Dictionary<string, SiteText> GetSiteTexts()
		{
			var key = "cache_SiteTexts";

			var data = Cache.Get(key) as Dictionary<string, SiteText>;
			UpdateCacheInfo(key, data == null);

			if (data == null)
			{
				data = HydroData.Fetch<SiteText>("").ToDictionary(x => x.TextId, x => x);
				if (data.Any()) Cache.Set(key, data, cacheTime);
			}
			return data;
		}

		private static void UpdateCacheInfo(string key, bool isNew)
		{
			if (isNew) CacheHits[key] = 1;
			else CacheHits[key] += 1;
		}

		private static List<station> FilterStationsBy(int varid, bool isPublic, List<station> stations, List<stationsvariable> vars)
		{
			List<short> filtered;
			IEnumerable<stationsvariable> filteredvars = vars;

			if (isPublic)
				filteredvars = filteredvars.Where(x => x.is_public == true);
			if (varid != 0)
				filteredvars = filteredvars.Where(x => x.var_id == varid);

			filtered = filteredvars.GroupBy(x => x.st_id).Select(x => x.Key).ToList();

			return stations.Where(x => filtered.Contains(x.st_id)).ToList();
		}

		public static List<station> GetStationsWithObservDates(int varid = 0, bool isPublic = false)
		{
			var stats = GetStations(varid, isPublic);

			var obsdates = GetLastObservations(varid);

			if (obsdates == null) return stats;

			foreach (var st in stats)
			{
				observstationdate d;
				//get last date
				if (obsdates.TryGetValue(st.st_id, out d))
				{
					st.StartObservDate = d.start_date;
					st.LastObservDate = d.last_date;
				}
			}
			return stats;
		}

		public static Dictionary<int, Dictionary<int, observstationdate>> GetLastObservations(int[] varid_)
		{

			var data = new Dictionary<int, Dictionary<int, observstationdate>>();
			foreach (var varid in varid_)
			{
				var obs = GetLastObservations(varid);
				data[varid] = obs;
			}
			return data;
		}

		public static Dictionary<int, observstationdate> GetLastObservations(int varid)
		{
			//if (varid == 0) return null;

			var key = "cache_LastObservDates";

			var data = Cache.Get(key) as List<observstationdate>;
			UpdateCacheInfo(key, data == null);

			if (data == null)
			{
				data = HydroData.Fetch<observstationdate>("");

				if (data.Any()) Cache.Set(key, data, cacheTime);
			}

			if (varid != 0)
				return data.Where(x => x.varid == varid).ToDictionary(x => x.stid, x => x);
			else
				return data.GroupBy(x => x.stid).
					ToDictionary(x => x.Key, x => new observstationdate
					{
						stid = x.Key,
						start_date = x.Min(l => l.start_date),
						last_date = x.Min(l => l.last_date),
					});
		}

		public static Dictionary<int, DateTime?> ProcessLastObservBy(string tname)
		{

			var sql = @"SELECT station_id as sid, Max(time_utc) as date FROM " + tname + " group by station_id";
			var data = HydroData.Fetch<ObservDate>(sql).ToDictionary(x => x.Sid, x => x.Date);
			return data;
		}

		public static bool UpdateLastObservationDates(int varid)
		{
			var db = HydroData;
			db.CommandTimeout = 180;

			var sql = @"
update 	t1
	set	t1.last_date	= (select max(time_utc) from {0} t2 where t2.station_id = t1.stid)
from	observstationdates t1 where t1.varid={1}";


			var varinf = Helper.Vars[varid];
			db.Execute(string.Format(sql, varinf.TableName, varinf.Id));

			return true;
		}

		public static List<river> _GetRivers()
		{
			var key = "Rivers";

			var data = Cache.Get(key) as List<river>;
			UpdateCacheInfo(key, data == null);

			if (data == null)
			{
				data = HydroData.Fetch<river>("select riv_id, riv_name from river order by riv_name asc");
				if (data.Any()) Cache.Set(key, data, cacheTime);
			}

			return data;
		}

		public static station GetStation(int sid)
		{
			var st = HydroData.Fetch<station, location, river, @operator, station>(
				(s, l, r, o) => { s.Location = l; s.River = r; s.Operator = o; return s; },
			@"select s.*,l.*,r.*,o.* from stations s
left join locations l on s.location_id = l.loc_id
left join river r on s.riv_id = r.riv_id
left join operator o on s.operator_id = o.id
            where st_id=@0", sid);

			return st.FirstOrDefault();
		}

		public static List<stationsvariable> GetStationsVariablesBy(int sid)
		{
			return HydroData.Fetch<stationsvariable>("where st_id=@0", sid);
		}

		public static void UpdateStationVars(int sid, int[] vars, int[] publ)
		{
			var db = HydroData;
			var old_vars = db.Fetch<stationsvariable>("where st_id=@0", sid);

			foreach (var var in vars)
			{
				var ov = old_vars.FirstOrDefault(x => x.var_id == var);
				var isPubl = publ != null && publ.Contains(var);

				if (ov == null)
					db.Insert(new stationsvariable
					{
						st_id = (short)sid,
						var_id = (byte)var,
						ch_id = null,
						is_public = isPubl,
					});
				else
				{
					old_vars.Remove(ov);
					db.Execute("update stationsvariables set is_public=@2 where st_id=@0 and var_id=@1", sid, var, isPubl);
				}
			}
			foreach (var item in old_vars)
			{
				db.Execute("delete from stationsvariables where st_id=@0 and var_id=@1", sid, item.var_id);
			}
		}

		const string UPD_SUCCESS = "updated successfull";
		const string ADDED_SUCCESS = "added successfull";
		const string NOT_EXIST = "don't exist";
		const string DUPLICATED = "duplicate value";


	}
}
