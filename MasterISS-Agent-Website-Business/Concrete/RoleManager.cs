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
    public class RoleManager : IRoleService
    {
        IRoleDal _roleDal;

        public RoleManager(IRoleDal roleDal)
        {
            _roleDal = roleDal;
        }

        public IResult Add(Role user)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<Role>> GetAll()
        {
            return new SuccessDataResult<List<Role>>(_roleDal.GetAll(), "Başarılı");
        }

        public IDataResult<Role> GetById(long userId)
        {
            throw new NotImplementedException();
        }
    }
}
