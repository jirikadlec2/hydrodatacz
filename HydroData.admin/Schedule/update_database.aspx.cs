using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HydroData.Schedule
{
    public partial class update_database : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            foreach (var item in Helper.VarsTable)
                Repo.UpdateLastObservationDates(item.Id);
        }
    }
}