using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate;
using System.Data;
using Web.Generics.Core.CustomTypes;

namespace Web.Generics.Core.NHibernate
{
    public class TypeWithValue<T> : IUserType where T : CustomType, new()
    {
        private static readonly global::NHibernate.SqlTypes.SqlType[] SQL_TYPES =
        { NHibernateUtil.String.SqlType };

        public global::NHibernate.SqlTypes.SqlType[] SqlTypes
        {
            get { return SQL_TYPES; }
        }
        public Type ReturnedType { get { return typeof(T); } }

        public new bool Equals( object x, object y ) {
            if ( object.ReferenceEquals(x,y) ) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }
        public object DeepCopy(object value) { return value; }

        public object NullSafeGet(IDataReader dr, string[] names, object owner){
            object obj = NHibernateUtil.String.NullSafeGet(dr, names[0]);
            if ( obj==null ) return null;
            String strObj = (String) obj;
            return new T { Value = strObj };
        }

        public void NullSafeSet(IDbCommand cmd, object obj, int index) {
            if (obj == null) {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
            } else {
                var entity = (T)obj;
                ((IDataParameter)cmd.Parameters[index]).Value = entity.Value;
            }
        }

        public static T Convert(T obj, string strObj)
        {
            return new T { Value = strObj };
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public object Replace(object original, object target, object owner)
        {
            //As our object is immutable we can just return the original  
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            //Used for casching, as our object is immutable we can just return it as is  
            return cached;
        }

        public object Disassemble(object value)
        {
            //Used for casching, as our object is immutable we can just return it as is  
            return value;
        }  

        public int GetHashCode(object x)
        {
            return this.GetHashCode();
        }
    }
}
