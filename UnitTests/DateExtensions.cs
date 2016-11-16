using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class DateExtensions
    {
        public static string ToLongDateString(this DateTime dt)
        {
            return dt.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.LongDatePattern, CultureInfo.CurrentUICulture);
        }

        public static string ToLongTimeString(this DateTime dt)
        {
            return dt.ToString(CultureInfo.CurrentUICulture.DateTimeFormat.LongTimePattern, CultureInfo.CurrentUICulture);
        }
    }
}
