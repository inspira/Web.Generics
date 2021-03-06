﻿/*
Copyright 2010-2011 Inspira Tecnologia.
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
using System.Web.Mvc.Html;
using System.Web.Mvc;
using System.Reflection;
using System.Resources;

namespace Web.Generics.UserInterface.Extensions
{
    public static class SelectListHelper
    {
        public static SelectListItem ToSelectListItem<T, TResult, TResult2>(T obj, Func<T, TResult> value, Func<T, TResult2> text)
        {
            SelectListItem result = new SelectListItem();
            result.Value = value.Invoke(obj).ToString();
            result.Text = text.Invoke(obj).ToString();
            return result;
        }

        public static SelectList ToSelectList<T, TResult, TResult2>(this IEnumerable<T> list, Func<T, TResult> value, Func<T, TResult2> text)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            list.ToList<T>().ForEach(item => result.Add(ToSelectListItem(item, value, text)));

            return new SelectList(result, "Value", "Text");
        }

        public static SelectList ToSelectList<T>(this Enum obj)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            Type type = typeof(T);

            string enumName = type.Name;

            FieldInfo[] fields = type.GetFields();

            List<FieldInfo> enumFields = new List<FieldInfo>();

            List<string> keys = new List<string>();

            foreach (FieldInfo info in fields)
            {
                if (info.Name == "value__")
                    continue;

                string key = enumName + "_" + info.Name;
                enumFields.Add(info);
                keys.Add(key);


            }

            string[] resourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            ResourceManager rm = null;

            for (int i = 0; i < resourceNames.Length; i++)
                if (resourceNames[i].Contains("Enums.resources"))
                {
                    rm = new ResourceManager(resourceNames[i].Substring(0, resourceNames[i].Length - 10), Assembly.GetExecutingAssembly());

                    for (int j = 0; j < keys.Count; j++)
                    {
                        string value = rm.GetString(keys[j]);

                        if (value == null)
                            value = enumFields[j].Name;

                        SelectListItem lItem = new SelectListItem();

                        lItem.Text = value;
                        lItem.Value = enumFields[j].GetRawConstantValue().ToString();

                        list.Add(lItem);
                    }

                }

            return new SelectList(list, "Value", "Text", 0);
        }
    }
}
