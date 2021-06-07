using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
//using System.Web.ModelBinding;
using System.Web.Mvc;

namespace MasterISS_Agent_Website.Binders
{
    public class DateBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            var result = bindingContext.ValueProvider.GetValue(key);
            if (result == null)
            {
                return null;
            }

            if (bindingContext.ModelType == typeof(DateTime?) && string.IsNullOrWhiteSpace(result.AttemptedValue))
            {
                return null;
            }

            DateTime value;
            if (DateTime.TryParse(result.AttemptedValue, Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out value))
            {
                return value.Date;
            }

            bindingContext.ModelState.AddModelError(key, string.Format(MasterISS_Agent_Website_Localization.Setup.SetupView.DateFormatIsNotCorrect, bindingContext.ModelMetadata.DisplayName));

            return null;
        }
    }
}