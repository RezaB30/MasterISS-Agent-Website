﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MasterISS_Agent_Website
{
    public class ServiceResponseRelatedPayment<T> : ServiceResponse<T>
    {
        public int TotalPageCount { get; set; }
    }
}