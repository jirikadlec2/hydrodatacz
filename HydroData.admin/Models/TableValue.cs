using PetaPoco;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HydroData.admin.Models
{
	public class TableValue
	{
		public DateTime DateTime { get; set; }
		public float Val { get; set; }
		public static float ModifyValue(int varid, float val)
		{
			if (val != -1 && val != -2 && (varid == 16 || varid == 1 || varid == 2))
				return val / 10;

			else return val;
		}

		public float ModValue(int VarId)
		{
			return TableValue.ModifyValue(VarId, Val);

		}
		public float ValueForDB(int VarId)
		{
			if (VarId == 16 || VarId == 1 || VarId == 2)
				return Val * 10;

			else return Val;
		}

		public float ValueForCalc(int VarId)
		{
			if (Val == -9999) return 0;
			if (VarId == 8)
			{
				if (Val == -1) return 0;
				if (Val == -2) return 0;
			}
			if (VarId == 1 || VarId == 2)
			{
				if (Val == -1) return 0;
				if (Val == -2) return 0;
			}

			return ModValue(VarId);
		}
		public string TextValue(int VarId)
		{
			if (Val == -9999) return "No data";
			if (VarId == 8)
			{
				if (Val == -1) return "dusting of snow";
				if (Val == -2) return "remains of snow";
			}
			if (VarId == 1 || VarId == 2)
			{
				if (Val == -1) return "error";
				if (Val == -2) return "trace";
			}

			return ModValue(VarId) + "";
		}
		public int value_accuracy { get; set; }
		public int qualifier_id { get; set; }
	}
}