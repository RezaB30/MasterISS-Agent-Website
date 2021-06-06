using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website
{
    public static class FileOperations
    {
        public static KeyValuePair<bool, string> ValidFiles(IEnumerable<HttpPostedFileBase> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    var fileSize = Convert.ToDecimal(file.ContentLength) / 1024 / 1024;
                    if (Properties.Settings.Default.FileSizeLimit > fileSize)
                    {
                        var extension = Path.GetExtension(file.FileName);
                        string[] acceptedExtensions = { ".jpg", ".pdf", ".png", ".jpeg" };

                        if (acceptedExtensions.Contains(extension))
                        {
                            return new KeyValuePair<bool, string>(true, null);
                        }

                        return new KeyValuePair<bool, string>(false, MasterISS_Agent_Website_Localization.Customer.CustomerView.FaultyFormat);
                    }
                    return new KeyValuePair<bool, string>(false, MasterISS_Agent_Website_Localization.Customer.CustomerView.MaxFileSizeError);
                }
            }
            return new KeyValuePair<bool, string>(false, MasterISS_Agent_Website_Localization.Customer.CustomerView.SelectFile);
        }
    }
}