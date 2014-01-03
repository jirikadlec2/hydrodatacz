using Data;
using HydroData;
using HydroData.admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

//namespace HydroData

public class DBHelper
{

	const string UPD_SUCCESS = "updated successfull";
	const string ADDED_SUCCESS = "added successfull";
	const string DUPLICATED = "duplicate value";

	public static bool IsExistVar(int vid, int sid, DateTime time)
	{
		var tName = Helper.Vars[vid].TableName;

		var res = Repo.HydroData.FirstOrDefault<DateTime?>(
		  string.Format("select time_utc from {0}", tName) +
		  " where time_utc=@0 and station_id=@1", time, sid);

		return res != null;
	}

	public static string UpdateVarValue(int varid, int sid, DateTime time, double val, int vacc, byte qid)
	{
		var db = Repo.HydroData;
		try
		{

			var tName = Helper.Vars[varid].TableName;
			var cName = Helper.Vars[varid].ColumnName;
			if (varid != 8)
			{
				var res = db.Execute(
					string.Format("update {0} set {1}=@2,qualifier_id=@3 where time_utc=@0 and station_id=@1", tName, cName),
				 time, sid, CorrectVal(varid, val), qid);
				return UPD_SUCCESS;
			}
			else
			{
				var res = db.Execute(
					 string.Format(
					 "update {0} set {1}=@2, value_accuracy=@3, qualifier_id=@4 where time_utc=@0 and station_id=@1", tName, cName),
					time, sid, CorrectVal(varid, val), vacc, qid);
				return UPD_SUCCESS;
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}

	}

	public static string AddVarValue(int varid, short sid, DateTime time, double val, int vacc, byte qid = 4)
	{
		var db = Repo.HydroData;

		try
		{

			var tName = Helper.Vars[varid].TableName;
			var cName = Helper.Vars[varid].ColumnName;

			var res = IsExistVar(varid, sid, time);

			if (res == null)
			{
				//snow
				if (varid != 8)
				{
					var sql = string.Format(
						"INSERT INTO {0} ([station_id],[time_utc],[{1}],[qualifier_id]) VALUES (@0,@1,@2,@3)", tName, cName);
					db.Execute(sql, sid, time, CorrectVal(varid, val), qid);
					return ADDED_SUCCESS;
				}
				else
				{
					var sql = string.Format(
						   "INSERT INTO {0} ([station_id],[time_utc],[{1}],[value_accuracy],[qualifier_id]) VALUES (@0,@1,@2,@3,@4)",
						   tName, cName);

					db.Execute(sql, sid, time, CorrectVal(varid, val), vacc, qid);
					return ADDED_SUCCESS;
				}


			}
			else return DUPLICATED;

		}
		catch (Exception ex)
		{
			return ex.Message;
		}

	}


	public static PetaPoco.Sql SQLGetAllValues(int varid)
	{
		var sql = "";
		string tname = "temperature";
		string val_column = "temperature";

		if (Helper.Vars.ContainsKey(varid))
		{
			tname = Helper.Vars[varid].TableName;
			val_column = Helper.Vars[varid].ColumnName;
		}

		string val_sql = "";

		if (varid == 8)
			val_sql = string.Format(",{0} as val, value_accuracy", val_column);
		else
			val_sql = string.Format(",{0} as val", val_column);

		sql = string.Format("SELECT time_utc as DateTime, qualifier_id {0} FROM {1}", val_sql, tname);

		var sqlb = PetaPoco.Sql.Builder.Append(sql);
		return sqlb;
	}

	public static PetaPoco.Sql SQLGetValByVarAndStat(int varid, int stid)
	{
		var sql = DBHelper.SQLGetAllValues(varid);
		sql = sql.Where("station_id=@0", stid);
		return sql;
	}

	static double CorrectVal(int varid, double val)
	{
		if (varid == 16 || varid == 1 || varid == 2)
			return val * 10;
		else return val;
	}

}

