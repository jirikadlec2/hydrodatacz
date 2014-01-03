using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;


public static class NumberHelper
{
    public static string ToStrWith(this float d, string separator = ".")
    {
        var nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = separator;
        return d.ToString(nfi);
    }
    public static string ToStr(this float d, int places = 2)
    {
        return String.Format("{0:0.##}", d);

    }

    public static string ToStrWithPoint(this decimal? d)
    {
        var nfi = new NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        //nfi.NumberGroupSeparator = ".";

        if (d.HasValue)
            return d.Value.ToString(nfi);
        return "";
    }
    public static string ToShortDate(this DateTime? d)
    {
        if (!d.HasValue) return null;
        return d.Value.ToShortDateString();
    }
}
