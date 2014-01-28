using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HydroData.Schedule
{
    public partial class update_obsdates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["varid"]))
                Repo.UpdateLastObservationDates(Convert.ToInt32(Request.QueryString["varid"]));


            //foreach (var item in Helper.VarsTable)
            //    Repo.UpdateLastObservationDates(item.Id);

            //Repo.InsertNewStationsIntoObservDates(16);
        }
    }
}