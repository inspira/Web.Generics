using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Generics.Core.CustomTypes
{
    public class EmailAddress : CustomType
    {
        public override bool IsValid()
        {
            return base.ValidateByRegex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}
