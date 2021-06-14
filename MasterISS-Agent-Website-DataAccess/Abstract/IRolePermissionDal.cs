using MasterISS_Agent_Website_Core.DataAccess;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_DataAccess.Abstract
{
    public interface IRolePermissionDal : IEntityRepository<RolePermission>
    {
        void AddRange(IEnumerable<RolePermission> entity);

        void RemoveRange(IEnumerable<RolePermission> entity);

    }
}
