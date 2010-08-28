﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Web.Generics.Infrastructure.DataAccess.FluentNHibernate
{
    public class EnumConvention : IUserTypeConvention
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Property.PropertyType.IsEnum);
        }

        public void Apply(IPropertyInstance target)
        {
            target.CustomType(target.Property.PropertyType);
        }
    }
}
