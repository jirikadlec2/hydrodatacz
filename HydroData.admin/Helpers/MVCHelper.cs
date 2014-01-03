using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Text;
using System.Linq;
using System.IO;
using System.Web.UI;
using System.Web.Security;
using HydroData;
using System.Linq.Expressions;


public static class MVCHelper
{

    public static string RenderPartialToString(string controlName, object viewData)
    {
        ViewDataDictionary vd = new ViewDataDictionary(viewData);
        ViewPage vp = new ViewPage { ViewData = vd };
        Control control = vp.LoadControl(controlName);

        vp.Controls.Add(control);

        StringBuilder sb = new StringBuilder();
        using (StringWriter sw = new StringWriter(sb))
        {
            using (HtmlTextWriter tw = new HtmlTextWriter(sw))
            {
                vp.RenderControl(tw);
            }
        }

        return sb.ToString();
    }

    public static MvcHtmlString Span(this HtmlHelper helper, string id)
    {
        return helper.Span(id, id);
    }

    public static MvcHtmlString Span(this HtmlHelper helper, string id, string defText)
    {
        var text = LangHelper.LangText(id);
        if (string.IsNullOrEmpty(text)) text = defText;

        var ctx = helper.ViewContext.RequestContext;

        if (ctx.HttpContext.Request.IsAuthenticated)
        {
            var uname = ctx.HttpContext.User.Identity.Name;
            if (Helper.IsAdmin(uname))
            {
                if (String.IsNullOrEmpty(text)) text = defText;
                return MvcHtmlString.Create(
                    String.Format("<span  id=\"{0}\" class=\"trans_element\">{1}</span>", id, text));
            }
        }
        return MvcHtmlString.Create(text);

    }

    public static MvcHtmlString DescriptionFor<TModel, TValue>(
       this HtmlHelper<TModel> self,
       Expression<Func<TModel, TValue>> expression)
    {
        var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
        var description = T.Text(metadata.Description);

        return MvcHtmlString.Create(string.Format(@"<span class=""help-block"">{0}</span>", description));
    }

    public static MvcHtmlString LabelFor<TModel, TValue>(
        this HtmlHelper<TModel> self,
        Expression<Func<TModel, TValue>> expression,
        bool showToolTip = false
    )
    {
        var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
        var name = T.Text(metadata.DisplayName);

        return MvcHtmlString.Create(string.Format(@"<label for=""{0}"">{1}</label>", metadata.PropertyName, name));
    }



}

public class T
{
    public static string Text(string id)
    {
        string tt = LangHelper.LangText(id);
        return tt ?? id;
    }
	public static string BigText(string id)
	{
		string tt = LangHelper.BigText(id);
		return tt ?? id;
	}
    public static string Text(MvcHtmlString id)
    {
        string tt = LangHelper.LangText(id.ToString());
        return tt ?? id.ToString();
    }
}



