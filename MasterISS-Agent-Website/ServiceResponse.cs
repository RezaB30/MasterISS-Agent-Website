using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}