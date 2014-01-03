using PetaPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.admin.Models
{
    public class ChartViewModel
    {
        public ChartViewModel()
        {

            Items = new List<TableValue>();
            //Date = DateTime.Now.Date;

            Page = new Page<TableValue>();
            Page.CurrentPage = 1;
            Page.Items = new List<TableValue>();
        }
        public int StatId { get; set; }
        public int VarId { get; set; }
        public string VarName { get; set; }
        public bool NeedCorrectVal
        {
            get
            {
                return (VarId == 16 || VarId == 1 || VarId == 2);
            }
        }


        public DateTime? Date { get; set; }
        public SelectList Vars { get; set; }
        public SelectList Stations { get; set; }

        public string UrlTempl { get; set; }
        public Page<TableValue> Page { get; set; }
        public List<TableValue> Items { get; set; }
    }

  
}