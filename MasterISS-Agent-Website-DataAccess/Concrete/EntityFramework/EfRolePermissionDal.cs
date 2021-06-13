using MasterISS_Agent_Website_Core.DataAccess.EntityFramework;
using MasterISS_Agent_Website_DataAccess.Abstract;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_DataAccess.Concrete.EntityFramework
{
    public class EfRolePermissionDal : EfEntityRepositoryBase<RolePermission, MasterISSAgentWebSiteEntities>, IRolePermissionDal
    {
        public void AddRange(IEnumerable<RolePermission> entity)
        {
            using (var db = new MasterISSAgentWebSiteEntities())
            {
                db.RolePermission.AddRange(entity);
                db.SaveChanges();
            }
        }
    }
}
