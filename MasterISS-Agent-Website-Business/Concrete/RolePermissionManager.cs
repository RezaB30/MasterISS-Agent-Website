using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Core.Utilities;
using MasterISS_Agent_Website_DataAccess.Abstract;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            //    var validRole = _rolePermissionDal.GetAll(rp => rp.RoleId == rolePermissionList.FirstOrDefault().RoleId);
            //    if (validRole != null)
            //{
                _rolePermissionDal.AddRange(rolePermissionList);
                return new SuccessResult();
            //}
            //return new ErrorResult(MasterISS_Agent_Website_Localization.View.GenericErrorMessage);
        }

        public IDataResult<List<RolePermission>> GetByFilter(Expression<Func<RolePermission, bool>> filter)
        {
            return new SuccessDataResult<List<RolePermission>>(_rolePermissionDal.GetAll(filter), MasterISS_Agent_Website_Localization.View.Successful);
        }

        public IResult RemoveRange(IEnumerable<RolePermission> rolePermissionList)
        {
            //var validRole = _rolePermissionDal.GetAll(rp => rp.RoleId == rolePermissionList.FirstOrDefault().RoleId);
            //if (validRole != null)
            //{
                _rolePermissionDal.RemoveRange(rolePermissionList);
                return new SuccessResult();
            //}

            //return new ErrorResult(MasterISS_Agent_Website_Localization.View.GenericErrorMessage);
        }
    }
}
