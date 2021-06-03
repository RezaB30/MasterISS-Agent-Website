using Microsoft.Extensions.Logging;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website
{

    public static class ExtensionMethods
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public static SelectList EnumSelectList<TEnum, TResource>(object selectedValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            try
            {
                var list = new LocalizedList<TEnum, TResource>().GetList(CultureInfo.CurrentCulture);

                var selectList = new SelectList(list.Select(m => new { Name = m.Value, Value = m.Key }).ToArray(), "Value", "Name", selectedValue);

                return selectList;
            }
            catch (Exception ex)
            {
                LoggerError.Fatal($"An error occurred while EnumSelectList => ErrorMessage : {ex.Message}");
                return null;
            }


        }
    }
}