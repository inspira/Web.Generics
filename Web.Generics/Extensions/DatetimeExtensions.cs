using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class DatetimeExtensions
    {
        /// <summary>
        /// Converts the DateTime object to a RFC-822 compliant string.
        /// </summary>
        public static string ToRFC822Date(this DateTime dt)
        {
            string timeZone;
            int offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours;
            if (offset < 0)
            {
                offset *= -1;
                timeZone = String.Format("-{0}", offset.ToString().PadLeft(2, '0'));
            }
            else
                timeZone = String.Format("+{0}", offset.ToString().PadLeft(2, '0'));
            return dt.ToString(String.Format("ddd, dd MMM yyyy HH:mm:ss {0}", timeZone.PadRight(5, '0')), System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
    }
}
