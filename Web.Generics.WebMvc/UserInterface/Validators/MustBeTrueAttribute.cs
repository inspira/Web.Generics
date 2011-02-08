using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Web.Generics.WebMvc.UserInterface.Validators
{
    public class MustBeTrueAttribute : ValidationAttribute
    {
        public MustBeTrueAttribute()
        {
        }

        public MustBeTrueAttribute(String errorMessage) : base(errorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            return value is bool ? (bool)value : false;
        }
    }
}
