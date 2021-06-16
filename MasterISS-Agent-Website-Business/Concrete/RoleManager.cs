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
    public class RoleManager : IRoleService
    {
        IRoleDal _roleDal;

        public RoleManager(IRoleDal roleDal)
        {
            _roleDal = roleDal;
        }

        public IResult Add(Role role)
        {
            _roleDal.Add(role);
            return new SuccessResult("Success");
        }

        public IDataResult<List<Role>> GetByFilter(Expression<Func<Role, bool>> filter)
        {
            return new SuccessDataResult<List<Role>>(_roleDal.GetAll(filter), "Success");

        }

        public IDataResult<List<Role>> GetAll()
        {
            return new SuccessDataResult<List<Role>>(_roleDal.GetAll(), "Success");
        }

        public IDataResult<Role> Get(Expression<Func<Role, bool>> filter)
        {
            return new SuccessDataResult<Role>(_roleDal.Get(filter), MasterISS_Agent_Website_Localization.View.Successful);
        }
    }
}
