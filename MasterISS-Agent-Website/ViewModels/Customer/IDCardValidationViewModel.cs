using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website.ViewModels.Customer
{
    public class IDCardValidationViewModel
    {
        public string BirtDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IdCardType { get; set; }
        public string RegistirationNo { get; set; }
        public string TCKNo { get; set; }
    }
}