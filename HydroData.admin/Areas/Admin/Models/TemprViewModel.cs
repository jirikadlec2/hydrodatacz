using Data;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.admin.Models
{
    public class TemprViewModel<T>
    {
        public TemprViewModel()
        {
            Page = new Page<T>();
            Page.CurrentPage = 1;
            Page.Items = new List<T>();
        }
        public int StatId { get; set; }
        public DateTime? Date { get; set; }

        public SelectList Stations { get; set; }
        public string UrlTempl { get; set; }
        public string DateFormat = "dd.mm.yy";

        public Page<T> Page { get; set; }
    }
}