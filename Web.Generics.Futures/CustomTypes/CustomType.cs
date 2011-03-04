using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace Web.Generics.Core.CustomTypes
{
    public abstract class CustomType : IValidatableObject
    {
        private String _value;
        public String Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public virtual Boolean IsValid()
        {
            return true;
        }

        public override string ToString()
        {
            return this.Value;
        }

        internal bool ValidateByRegex(string pattern)
        {
            if (this._value == null) return false;
            return new Regex(pattern).IsMatch(this._value);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (String.IsNullOrEmpty(this._value))
            {
                return new List<ValidationResult> {
                    new ValidationResult(String.Format("The {0} field is required", validationContext.DisplayName), new [] { validationContext.MemberName, this.GetType().Name })
                };
            }

            if (!this.IsValid())
            {
                return new List<ValidationResult> {
                    new ValidationResult(String.Format("The value '{0}' is not valid for {1}", this._value, validationContext.DisplayName), new [] { validationContext.MemberName, this.GetType().Name })
                };
            }
            return new List<ValidationResult>();
        }
    }
}
