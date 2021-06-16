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
    public interface IRoleService
    {
        IDataResult<List<Role>> GetAll();

        IResult Add(Role role);

        IDataResult<Role> Get(Expression<Func<Role, bool>> filter);


        IDataResult<List<Role>> GetByFilter(Expression<Func<Role, bool>> filter);

    }
}
