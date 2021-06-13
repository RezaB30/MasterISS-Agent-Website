using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Core.Utilities;
using MasterISS_Agent_Website_DataAccess.Abstract;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_Business.Concrete
{
    public class RolePermissionManager : IRolePermissionService
    {
        private readonly IRolePermissionDal _rolePermissionDal;

        public RolePermissionManager(IRolePermissionDal rolePermissionDal)
        {
            _rolePermissionDal = rolePermissionDal;
        }

        public IResult AddRange(IEnumerable<RolePermission> rolePermissionList)
        {
            _rolePermissionDal.AddRange(rolePermissionList);
            return new SuccessResult();
        }
    }
}
