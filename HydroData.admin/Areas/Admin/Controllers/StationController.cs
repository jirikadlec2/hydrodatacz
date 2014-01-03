using Data;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.Areas.Admin.Controllers
{
    public class StationController : BaseController
    {
        //
        // GET: /Station/

        public ActionResult Index(int? Operator, int? Stat,int? pg)
        {
            var opers = Repo.HydroData.Fetch<@operator>("");

            ViewBag.Operator = new SelectList(opers, "id", "name", Operator);
            ViewBag.Stat = new SelectList(Repo.GetStations(), "st_id", "st_name", Stat);

			IEnumerable<station> stats = Repo.GetStations();

            if (Stat.HasValue)
				stats = stats.Where(x => x.st_id == Stat);
            else if (Operator.HasValue)
				stats = stats.Where(x => x.operator_id == Operator);
           

            var p = new Page<station>();
            p.CurrentPage = pg.GetValueOrDefault(1);
            //p.Items = stats.Skip((int)p.CurrentPage*20-20).Take(20).ToList();
           
            return View(stats.ToList());
        }

        public ActionResult List()
        {
            return View(Repo.GetStations());
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Default1/Create
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            ViewBag.StatVars = null;
            ViewBag.Vars = Repo.HydroData.Fetch<variable>("where var_id in (@0)", Helper.VarsID);

            var st = new station();
            st.Location = new location();
            st.River = new river();
            st.Operator = new @operator();

            var river = Repo._GetRivers();
            var operators = Repo.HydroData.Fetch<@operator>("");

            ViewBag.river1 = new SelectList(river, "riv_id", "riv_name", st.riv_id);
            ViewBag.Operator1 = new SelectList(operators, "id", "name", st.operator_id);



            var max = Repo.HydroData.ExecuteScalar<short>("SELECT max(st_id) from stations");
            st.st_id = ++max;

            return View("Edit", st);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection, int[] vars, int[] publ)
        {
            try
            {
                var st = new station();


                if (TryUpdateModel(st))
                {
					Repo.InvalidateByPattern("cache_stations");

                    st.Insert();
                    Repo.UpdateStationVars(st.st_id, vars, publ);

                    ViewBag.Message = "New station added successfully";
                    return RedirectToAction("Edit", new { id = st.st_id });
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id)
        {
            var st = Repo.GetStation(id);

            ViewBag.StatVars = Repo.GetStationsVariablesBy(id);

            ViewBag.Vars = Repo.HydroData.Fetch<variable>("where var_id in (@0)", new[] { 4, 5, 16, 8, 1, 2, });

            var river = Repo._GetRivers();
            var operators = Repo.HydroData.Fetch<@operator>("");

            ViewBag.river1 = new SelectList(river, "riv_id", "riv_name", st.riv_id);
            ViewBag.Operator1 = new SelectList(operators, "id", "name", st.operator_id);

            //.Select(x => new CheckBoxItem { Id = x.var_id, Name = x.var_id + "", Checked = true }).ToList();

            return View(st);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection, int[] vars, int[] publ)
        {
            try
            {
                var st = Repo.GetStation(id);

                if (TryUpdateModel(st))
                {					   
					Repo.InvalidateByPattern("cache_stations");

                    st.Update();
                    Repo.UpdateStationVars(id, vars, publ);
                    ViewBag.Message = "station edited successfully";

                    return RedirectToAction("Edit", new { id = id });
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "error when edit";
                return RedirectToAction("Edit", new { id = id });
            }
        }

        public string GetRiverId(string name)
        {
            var rivers = Repo._GetRivers();
            var r = rivers.FirstOrDefault(x => x.riv_name == name);

            if (r != null) return r.riv_id.ToString();
            else return "noid";
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
