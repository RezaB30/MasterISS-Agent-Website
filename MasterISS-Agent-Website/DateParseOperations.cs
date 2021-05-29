using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website
{
    public static class DateParseOperations
    {
        public static string ConvertDatetimeByWebService(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy", null).ToString("yyyy-MM-dd");
            return convertedValue;
        }
        public static string ConvertDatetimeByExpiryDate(string convertedDatetime)
        {
            var convertedValue = DateTime.ParseExact(convertedDatetime, "dd.MM.yyyy", null).AddYears(-10).ToString("yyyy-MM-dd");
            return convertedValue;
        }
    }
}