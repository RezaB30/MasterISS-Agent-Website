using MasterISS_Agent_Website_Core.Utilities;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_Business.Abstract
{
    public interface IRolePermissionService
    {
        IResult AddRange(IEnumerable<RolePermission> rolePermissionList);

        IResult RemoveRange(IEnumerable<RolePermission> rolePermissionList);

        IDataResult<List<RolePermission>> GetByFilter(Expression<Func<RolePermission, bool>> filter);

    }
}
