using Data;

using HydroData.admin.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

//namespace HydroData

public class CSVHelper
{
    //“DateTimeUTC”, “DataValue”, “QualifierID”
    public static List<TableValue> ParseValuesCSV(string ftext, out int errCount)
    {
        var lines = ftext.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        List<TableValue> vals = new List<TableValue>();
        errCount = 0;
        foreach (var ll in lines)
        {
            try
            {
                var vv = ll.Split(',');
                var val = new TableValue();
                val.DateTime = DateTime.ParseExact(vv[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                val.Val = float.Parse(vv[1]);
                val.qualifier_id = int.Parse(vv[2]);

                vals.Add(val);
            }
            catch (Exception)
            {
                errCount++;
            }

        }
        return vals;
    }

    public static IEnumerable<string> GenValuesCSV(int varid, int statid)
    {
        var sql = DBHelper.SQLGetValByVarAndStat(varid, statid);

        sql = sql.OrderBy("time_utc asc");

        var Items = Repo.HydroData.Page<TableValue>(1, 100000, sql).Items;

        foreach (var item in Items)
        {
            var line = string.Format("{0},{1},{2}", item.DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                item.ModValue(varid).ToStrWith(), item.qualifier_id);
            yield return line;
        }
    }

    public static bool ProcessData(int varid, int sid, string ftext, bool overwrite, out string strRes)
    {
        var db = Repo.HydroData;
        int counter = 0;
        int errCounter = 0;
        try
        {
            int err;
            var values = CSVHelper.ParseValuesCSV(ftext, out err);

            var tName = Helper.Vars[varid].TableName;
            var cName = Helper.Vars[varid].ColumnName;

            var all_sql = DBHelper.SQLGetValByVarAndStat(varid, sid);
            var cacheVals = db.Page<TableValue>(1, 100000, all_sql).Items;

            foreach (var item in values)
            {
                try
                {
                    if (overwrite)
                    {
                        if (cacheVals.Any(x => x.DateTime == item.DateTime))
                        {
                            //update
                            var sql = string.Format("update {0} set {1}=@2,qualifier_id=@3 where time_utc=@0 and station_id=@1", tName, cName);
                            var res = db.Execute(sql, item.DateTime, sid, item.ValueForDB(varid), item.qualifier_id);
                            counter++;
                        }
                        else
                        {
                            //insert new when overwriting values
                            var sql = string.Format(
                     "INSERT INTO {0} ([station_id],[time_utc],[{1}],[qualifier_id]) VALUES (@0,@1,@2,@3)", tName, cName);
                            var res = db.Execute(sql, sid, item.DateTime, item.ValueForDB(varid), item.qualifier_id);
                            counter++;
                        }
                    }

                    if (!overwrite && !cacheVals.Any(x => x.DateTime == item.DateTime))
                    {
                        var sql = string.Format(
                       "INSERT INTO {0} ([station_id],[time_utc],[{1}],[qualifier_id]) VALUES (@0,@1,@2,@3)", tName, cName);
                        var res = db.Execute(sql, sid, item.DateTime, item.ValueForDB(varid), item.qualifier_id);
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    errCounter++;
                }
            }
        }
        catch (Exception ex)
        {
            strRes = ex.Message;
            return false;
        }

        if (overwrite)
            strRes = string.Format("updated={0} lines, non updated values={1}", counter, errCounter);
        else
            strRes = string.Format("inserted={0} lines, non inserted values={1}", counter, errCounter);

        return true;
    }
}

