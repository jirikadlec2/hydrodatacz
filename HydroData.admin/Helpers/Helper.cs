using HydroData;
using HydroData.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc.Html;
using System.Web.Mvc;

//namespace HydroData


public class Helper
{
	public const string ADMIN_AREA = "admin";
	public const string PUBLIC_AREA = "public";
	public const int TRANSL_PAGESIZE = 30;

	public static string DATE_FORMAT
	{
		get
		{
			if (LangHelper.Locale == LangHelper.EN)
				return "mm/dd/yy";
			else return "dd.mm.yy";
		}
	}
	public static DateTime CurrentDate { get { return DateTime.Now.ToUniversalTime().Date; } }

	public static string Sub
	{
		get
		{
			return ConfigurationManager.AppSettings["subfolder"];
		}
	}
	public static string[] Cultures { get { return new[] { LangHelper.EN, LangHelper.CZ }; } }



	public static bool IsAdmin(string uname)
	{
		if (string.IsNullOrEmpty(uname)) return false;
		else
			return Roles.IsUserInRole(uname, "Administrator");
	}

	public static string PageUrl(string page)
	{
		return string.Format("/{0}/{1}/", LangHelper.SiteLang, page);
	}
	public static string GetRandomStationUrl(int varid)
	{
		var stats = Data.Repo.GetStations(varid, true);
		var rand = new Random();
		var stat = stats[rand.Next() % stats.Count];

		return string.Format("/{0}/charts/{1}/{2}", LangHelper.SiteLang,
			Helper.Vars[varid].TableName, stat.UrlParm);

	}

	#region Variables methods

	public static Dictionary<int, string> GraphVars
	{
		get
		{
			return new Dictionary<int, string>{
            {1,"p1h"},{2,"pcp"},{4,"wtr"},{5,"flw"},{8,"snw"},{16,"tmp"},
        
            };
		}
	}

	public static VariableInfo[] VarsTable
	{
		get
		{
			return HydroData.Data.Helper.Vars.Values.ToArray();
		}
	}

	public static int[] VarsID { get { return HydroData.Data.Helper.Vars.Keys.ToArray(); } }

	public static Dictionary<int, VariableInfo> Vars
	{
		get
		{
			return HydroData.Data.Helper.Vars;
		}
	}

	public static string GetRouteVar(int vid)
	{
		return Helper.Vars[vid].TableName;
	}

	public static VariableInfo FindVarByName(string varname)
	{
		return Helper.VarsTable.FirstOrDefault(x => x.TableName == varname.ToLower());
	}

	#endregion


	public static Dictionary<int, string> Qualifiers
	{
		get
		{
			var d = new Dictionary<int, string>();
			d.Add(1, "unchecked");
			d.Add(2, "suspect");
			d.Add(3, "checked");
			d.Add(4, "inserted");
			return d;
		}
	}


	public static string AppConfigValue(string key)
	{
		var v = ConfigurationManager.AppSettings[key];
		if (!string.IsNullOrEmpty(v)) return v;
		else return key;
	}

	public static int PageSize
	{
		get
		{
			return Convert.ToInt32(ConfigurationManager.AppSettings["page_size"]);
		}
	}

	public static string Substring(string str, int len)
	{
		if (!string.IsNullOrWhiteSpace(str) && str.Length > len)
			//return str.Substring(str.Length - len, len);
			return str.Substring(0, len);
		else return str;
	}





}
