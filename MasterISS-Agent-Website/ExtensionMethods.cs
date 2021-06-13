using MasterISS_Agent_Website_Enums.Enums;
using Microsoft.Extensions.Logging;
using NLog;
using RezaB.Data.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterISS_Agent_Website
{

    public static class ExtensionMethods
    {
        private static Logger LoggerError = LogManager.GetLogger("AppLoggerError");

        public static bool EnumIsDefinedforList<TEnum>(int[] list)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            foreach (var item in list)
            {
                if (!Enum.IsDefined(typeof(TEnum), item))
                {
                    return false;
                }
            }
            return true;
        }

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
                LoggerError.Fatal(ex, $"An error occurred while EnumSelectList");
                return null;
            }
        }
        public static string ConvertToBase64(HttpPostedFileBase file)
        {
            try
            {
                string theFileName = Path.GetFileName(file.FileName);
                byte[] theFileAsBytes = new byte[file.ContentLength];
                using (BinaryReader theReader = new BinaryReader(file.InputStream))
                {
                    theFileAsBytes = theReader.ReadBytes(file.ContentLength);
                }
                var thePFileDataAsString = Convert.ToBase64String(theFileAsBytes);
                return thePFileDataAsString;
            }
            catch (Exception ex)
            {
                LoggerError.Fatal(ex, "An error occurred while Stream ConvertToBase64");
                return null;
            }
        }

        public static string EnumDescription<TEnum, TResource>(int convertedValue) where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            try
            {
                var displayText = new LocalizedList<TEnum, TResource>().GetDisplayText(convertedValue, CultureInfo.CurrentCulture);

                return displayText;
            }
            catch (Exception ex)
            {
                LoggerError.Fatal(ex, $"An error occurred while EnumDescription");
                return null;
            }

        }

        public static SelectList NameValuePairSelectList<Tkey, TValue>(this IEnumerable<KeyValuePair<Tkey, TValue>> keyValuePairs, Tkey? selectedValue)
            where Tkey : struct
            where TValue : class
        {
            try
            {
                var selectList = new SelectList(keyValuePairs.Select(kvp => new { Name = kvp.Value, Value = kvp.Key }).ToArray(), "Value", "Name", selectedValue);
                return selectList;
            }
            catch (Exception ex)
            {
                LoggerError.Fatal(ex, $"An error occurred while NameValuePairSelectList");
                return null;
            }
        }

        public static string GetConvertedErrorMessage(int errorCode)
        {
            try
            {
                var convertedMessage = new LocalizedList<ErrorCodes, MasterISS_Agent_Website_Localization.Generic.ErrorCodeList>().GetDisplayText(errorCode, CultureInfo.CurrentCulture);
                return convertedMessage;
            }
            catch (Exception ex)
            {
                LoggerError.Fatal(ex, $"An error occurred while GetConvertedErrorMessage");
                throw;
            }

        }

    }
}