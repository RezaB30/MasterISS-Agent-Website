﻿using MasterISS_Agent_Website_Core.Utilities;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_Business.Abstract
{
    public interface IPermissionService
    {
        IDataResult<List<Permission>> GetAll();
    }
}
