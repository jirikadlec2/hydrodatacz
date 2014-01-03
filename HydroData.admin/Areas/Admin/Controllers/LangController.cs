using Data;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{

	public class LangController : BaseController
	{
		//
		// GET: /Lang/

		public ActionResult Index(string textid, int? pg)
		{
			var p = pg.GetValueOrDefault(1);

			var texts = db.Fetch<SiteText>("");

			ViewBag.PagingUrl = "/Lang/Index?pg={0}";

			var sql = PetaPoco.Sql.Builder
			   .Append("SELECT * FROM SiteTexts order by ModifiedDate desc ");

			if (!string.IsNullOrWhiteSpace(textid))
				sql = sql.Where("TextId=@0", textid);


			var Page = db.Page<SiteText>(p, 40, sql);
			return View(Page);
		}
		public ActionResult TextsPage(int? pg)
		{
			var p = pg.GetValueOrDefault(1);

			ViewBag.PagingUrl = "/lang/TextsPage?pg={0}";

			var data = Repo.GetSiteTexts();
			var Page = new Page<SiteText>();
			var count = 40;// Helper.TRANSL_PAGESIZE;
			Page.Items = data.Values.OrderByDescending(x => x.Accessed).Take(count).ToList();
			Page.CurrentPage = p;
			Page.TotalPages = data.Count / count + 1;

			//var Page = db.Page<SiteText>(p, 40, sql);
			return PartialView(Page);
		}



		[HttpGet]
		public JsonResult GetText(string id)
		{
			var tt = LangHelper.GetText(id);

			return Json(new { en = tt.Text0, cz = tt.Text1 }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public void UpdateText(string id, string text0, string text1)
		{
			LangHelper.UpdateTextById(id, text0, text1);
		}

		[HttpPost, ValidateInput(false)]
		public string DeleteText(string id)
		{
			LangHelper.DeleteText(id);
			return "success";
		}


		public ActionResult BigTexts(string textid, int? pg)
		{
			var p = pg.GetValueOrDefault(1);

			ViewBag.PagingUrl = "/lang/BigTexts?pg={0}";

			var sql = PetaPoco.Sql.Builder
			   .Append("SELECT * FROM SiteBigTexts order by ModifiedDate desc ");

			if (!string.IsNullOrWhiteSpace(textid))
				sql = sql.Where("TextId=@0", textid);


			var Page = db.Page<SiteBigText>(p, 40, sql);
			return View(Page);
		}

		public ActionResult EditBigText(string id)
		{
			var text = db.SingleOrDefault<SiteBigText>("where id=@0", id);
			return View(text);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EditBigText(string id, string text0, string text1)
		{
			LangHelper.AddOrUpdateBigText(id, text0, text1);
			return RedirectToAction("BigTexts");
		}
	}
}
