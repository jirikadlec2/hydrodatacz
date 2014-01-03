using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class SPage
    {
        public string UrlTempl { get; set; }
        public long CurrPage { get; set; }
        public long TotalPages { get; set; }
    }
}