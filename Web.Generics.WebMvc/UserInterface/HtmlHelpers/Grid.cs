﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Generics.WebMvc.UserInterface.HtmlHelpers;

namespace Web.Generics.UserInterface.HtmlHelpers
{
    public class Grid : RowList
    {
		public Grid() {
			this.Columns = new List<GridColumn>();
			this.Rows = new List<GridRow>();
		}

        public IList<GridColumn> Columns { get; set; }
        public IList<GridRow> Rows { get; set; }
    }
}