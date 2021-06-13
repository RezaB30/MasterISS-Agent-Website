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
    public interface IUserService
    {
        IDataResult<List<User>> GetAll();

        IResult Add(User user);

        IDataResult<List<User>> GetByFilter(Expression<Func<User, bool>> filter);

        IDataResult<User> GetById(long userId);
    }
}
