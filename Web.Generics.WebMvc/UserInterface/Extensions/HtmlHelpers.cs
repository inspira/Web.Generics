/*
Copyright 2010 Inspira Tecnologia.
All Rights Reserved.

Contact: Thiago Alves <thiago.alves@inspira.com.br>

This file is part of Web.Generics

Web.Generics is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Web.Generics is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Web.Generics.  If not, see <http://www.gnu.org/licenses/>.
*/

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Collections;
using System.Linq.Expressions;
using System.Web.Routing;

namespace Web.Generics.UserInterface.Extensions
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString List<T>(this HtmlHelper helper, IList<T> list, String format, params Func<T, Object>[] propExpressions)
        {
            var result = "<ul>";
            for (var i = 0; i < list.Count; i++)
            {
                var obj = list[i];
                var lists = propExpressions.Select(f => f.Invoke(obj));
                result += String.Format("<li>" + format + "</li>", lists.ToArray());
            }
            result += "</ul>";
            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> targetProperty, IDictionary dataSource, object htmlOptions)
        {
            var result = new StringBuilder();
            foreach (var key in dataSource.Keys)
            {
                var value = dataSource[key];
                result.Append(htmlHelper.RadioButtonFor(targetProperty, key, htmlOptions) + " " + value);
            }
            return MvcHtmlString.Create(result.ToString());
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty[]>> expression, MultiSelectList multiSelectList, object htmlAttributes = null)
        {
            //Derive property name for checkbox name
            string propertyName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ExpressionHelper.GetExpressionText(expression));

            //Get currently select values from the ViewData model
            TProperty[] list = expression.Compile().Invoke(htmlHelper.ViewData.Model);

            //Convert selected value list to a List<string> for easy manipulation
            List<string> selectedValues = new List<string>();

            if (list != null)
            {
                selectedValues = new List<TProperty>(list).ConvertAll<string>(delegate(TProperty i) { return i.ToString(); });
            }

            //Create div
            TagBuilder divTag = new TagBuilder("div");
            divTag.MergeAttributes(new RouteValueDictionary(htmlAttributes), true);

            //Add checkboxes
            foreach (SelectListItem item in multiSelectList)
            {
                divTag.InnerHtml += String.Format("<div><input type=\"checkbox\" name=\"{0}\" id=\"{0}_{1}\" " +
                                                    "value=\"{1}\" {2} /><label for=\"{0}_{1}\">{3}</label></div>",
                                                    propertyName,
                                                    item.Value,
                                                    selectedValues.Contains(item.Value) ? "checked=\"checked\"" : "",
                                                    item.Text);
            }

            return MvcHtmlString.Create(divTag.ToString());
        }

             
    }
}
