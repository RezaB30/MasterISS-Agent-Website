using System;
using System.Collections.Generic;
using System.Globalization;
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

        private static string ConvertDatetimeForFilter(string convertedDateTime)
        {
            DateTime date;
            var convertedValue = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (convertedValue)
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }
        public static string ConvertedDateTimeForGetTask(string convertedDateTime)
        {
            DateTime date;
            var convertedValue = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (convertedValue)
            {
                return date.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return null;
        }

        private static string ConvertDateForFilter(string convertedDateTime)
        {
            DateTime date;
            var convertedValue = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (convertedValue)
            {
                return date.ToString("yyyy-MM-dd");
            }
            return null;
        }

        public static DateTime? ConvertDate(string convertedDateTime)
        {
            DateTime convertedDate;
            var validDate = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate);
            if (validDate)
            {
                return convertedDate;
            }
            return null;
        }

        public static DateTime? ConvertDateTime(string convertedDateTime)
        {
            DateTime convertedDate;
            var validDate = DateTime.TryParseExact(convertedDateTime, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate);
            if (validDate)
            {
                return convertedDate;
            }
            return null;
        }
        public static bool DateIsCorrrect(bool IsOnlyDate, params string[] dateTimes)
        {
            DateTime convertedDate;
            if (IsOnlyDate == false)
            {
                foreach (var date in dateTimes)
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        if (!DateTime.TryParseExact(ConvertDatetimeForFilter(date), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                foreach (var date in dateTimes)
                {
                    if (!string.IsNullOrEmpty(date))
                    {
                        if (!DateTime.TryParseExact(ConvertDateForFilter(date), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDate))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

        }
    }
}