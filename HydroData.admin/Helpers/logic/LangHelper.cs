using Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


public class LangHelper
{
	const string CacheSiteTexts = "SiteTexts";
	const bool EnableDB = true;

	public const string EN = "en-US";
	public const string UrlLangEN = "en";
	public const string CZ = "cs-CZ";
	public const string UrlLangCZ = "cz";

	public static string Locale
	{
		get
		{
			//var sessCulture = HttpContext.Current.Session["Culture"];
			var cult = System.Threading.Thread.CurrentThread.CurrentUICulture;
			if (cult == null) return CZ;
			else return cult.Name;
		}
		set
		{
			//HttpContext.Current.Session["Culture"] = value;
			var cultureInfo = new CultureInfo(value);

			System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
			System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
		}
	}

	public static string LangToCulture(string lang)
	{
		switch (lang)
		{
			case UrlLangEN: return LangHelper.EN;
			case UrlLangCZ: return LangHelper.CZ;
			default:
				return LangHelper.CZ;
		}

	}

	public static string SiteLang
	{
		get
		{
			var cult = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
			string lang = cult == LangHelper.EN ? UrlLangEN : UrlLangCZ;
			return lang;
		}
	}

	public static bool IsDefaultLocale { get { return LangHelper.Locale == LangHelper.CZ; } }


	public static SiteText GetText(string TextId)
	{
		SiteText text = null;

		if (string.IsNullOrEmpty(TextId)) return null;
		var data = Repo.GetSiteTexts();

		if (!data.TryGetValue(TextId.ToLower(), out  text))
		{
			var tt = TextId.Replace("text.", "");
			text = AddOrUpdateText(TextId, tt, tt, false);
		}
		else
		{
			text.Accessed = DateTime.Now.ToUniversalTime();
		}

		return text;

	}

	public static string LangText(string TextId)
	{

		var tt = GetText(TextId);
		if (tt != null)
		{
			if (Locale == EN) return tt.Text0.Trim();
			if (Locale == CZ) return tt.Text1.Trim();
		}
		return TextId;

	}

	public static void DeleteText(string id)
	{
		var db = Repo.HydroData;

		db.Execute("delete from SiteTexts where id=@0", id);
		Repo.Cache.Invalidate("cache_SiteTexts");

	}

	public static SiteText AddOrUpdateText(string TextId, string text0, string text1, bool needUpd = true)
	{

		var db = Repo.HydroData;
		var key = TextId.ToLower();

		var text = db.SingleOrDefault<SiteText>("where textid=@0", key);

		if (text == null)
		{
			text = new SiteText();
			text.TextId = key;
			text.Text0 = text0;
			text.Text1 = text1;
			text.ModifiedDate = DateTime.Now.ToUniversalTime();
			text.ChangedCount = 1;
			db.Insert(text);
		}
		else
		{
			if (needUpd)
			{
				text.Text0 = text0;
				text.Text1 = text1;
				text.ModifiedDate = DateTime.Now.ToUniversalTime();
				text.ChangedCount += 1;
				db.Update(text);
			}
		}
		Repo.Cache.Invalidate("cache_SiteTexts");

		return text;
	}

	public static SiteText UpdateTextById(string Id, string text0, string text1, bool needUpd = true)
	{

		var db = Repo.HydroData;

		var text = db.SingleOrDefault<SiteText>("where id=@0", Id);

		if (text != null)
		{

			if (needUpd)
			{
				text.Text0 = text0;
				text.Text1 = text1;
				text.ModifiedDate = DateTime.Now.ToUniversalTime();
				text.ChangedCount += 1;
				db.Update(text);
				Repo.Cache.Invalidate("cache_SiteTexts");
			}
		}

		return text;

	}

	public static string BigText(string textid)
	{
		var data = Repo.GetSiteBigTexts(); //(x=>x.TextId == id.ToLower().Trim());
		SiteBigText text = null;
		var key = textid.ToLower().Trim();

		if (data.TryGetValue(key, out text))
		{
			if (Locale == EN) return text.Text0.Trim();
			if (Locale == CZ) return text.Text1.Trim();
		}
		else
		{
			text = AddOrUpdateBigText(key, key, key);
		}
		return key;
	}

	public static SiteBigText AddOrUpdateBigText(string id, string text0, string text1)
	{

		var db = Repo.HydroData;

		var text = db.SingleOrDefault<SiteBigText>("where id=@0", id);

		if (text != null)
		{
			text.Text0 = text0;
			text.Text1 = text1;
			text.ModifiedDate = DateTime.Now.ToUniversalTime();
			text.ChangedCount += 1;
			db.Update(text);
			Repo.InvalidateByPattern("cache_SiteBigText");

		}
		else
		{

			text = new SiteBigText();
			text.TextId = id;
			text.Text0 = text0;
			text.Text1 = text1;
			text.ModifiedDate = DateTime.Now.ToUniversalTime();
			text.ChangedCount = 1;
			db.Insert(text);
			Repo.InvalidateByPattern("cache_SiteBigText");
		}
		return text;

	}

}

