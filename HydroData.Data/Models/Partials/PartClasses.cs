using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
   public partial class station
   {
       public stationsvariable Variable { get; set; }
       public location Location { get; set; }
       public river River { get; set; }
       public @operator Operator { get; set; }
	   public DateTime? LastObservDate { get; set; }
	   public DateTime? StartObservDate { get; set; }
	   public string LastObservTName { get; set; }
	   public string UrlParm { get { return st_uri.Replace('/', '-'); } }

   }
   public partial class SiteText
   {
	   public DateTime? Accessed { get; set; }
   }
   public class ObservDate
   {
       public int Sid { get; set; }
       public DateTime? Date { get; set; }
   }
}
