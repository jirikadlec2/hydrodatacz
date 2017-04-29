using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroData.Data
{
	public class VariableInfo
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string PageName { get; set; }
		public string ColumnName { get; set; }
		public string TableName { get; set; }
		public string ValueDescr { get; set; }
		public string MeasureUnit { get; set; }
	}

	public class Helper
	{
		public static Dictionary<int, VariableInfo> Vars
		{
			get
			{
				return new Dictionary<int, VariableInfo>{

						{1, new VariableInfo{Id=1, Name="precip_hour", PageName ="HourlyRainfall",
                        ColumnName ="rain_mm_10",TableName="plaveninycz.rain_hourly",ValueDescr="(mm)", MeasureUnit="(mm)"}},

						{2,new VariableInfo{Id=2, Name="precip_day", PageName ="DailyRainfall",
                        ColumnName ="rain_mm_10",TableName="plaveninycz.rain_daily",ValueDescr="(mm)", MeasureUnit="(mm)"}},
   
						{4,new VariableInfo{Id=4, Name="stage",PageName ="Stage",
                        ColumnName ="stage_mm",TableName="stage",ValueDescr="(mm)", MeasureUnit="(mm)"}},
                        
						{5,new VariableInfo{Id=5, Name="discharge_small",PageName ="Discharge",
                        ColumnName ="discharge_m3s",TableName="discharge",ValueDescr="(m3s)", MeasureUnit="(m&sup3;/s)"}},

						{8,new VariableInfo{Id=8, Name="snow",PageName ="Snow",
                        ColumnName ="snow_cm",TableName="plaveninycz.snow",ValueDescr="(cm)", MeasureUnit="(cm)"}},
                 
						{16,new VariableInfo{Id=16, Name="temperature",PageName ="Temperature",
                        ColumnName ="temperature",TableName="temperature",ValueDescr="(°C)", MeasureUnit="(°C)"}},
                    
                };

			}
		}

		public static int GetVarByName(string tname)
		{
			var item = Vars.Values.FirstOrDefault(x => x.TableName == tname);
			if (item != null) return item.Id;
			else return -1;
		}
	}


}
