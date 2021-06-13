using MasterISS_Agent_Website_Business.Abstract;
using MasterISS_Agent_Website_Core.Utilities;
using MasterISS_Agent_Website_DataAccess.Abstract;
using MasterISS_Agent_Website_Enums.Enums;
using MasterISS_Partner_Website_Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MasterISS_Agent_Website_Business.Concrete
{
    public class UserManager : IUserService
    {
        IUserDal _userdal;

        public UserManager(IUserDal userdal)
        {
            _userdal = userdal;
        }

        public IResult Add(User user)
        {
            if (Enum.IsDefined(typeof(PermissionList), user.RoleId))
            {
                var result = _userdal.Get(u => u.Username == user.Username);

                if (result == null)
                {
                    _userdal.Add(user);
                    return new SuccessResult(MasterISS_Agent_Website_Localization.View.Successful);
                }
                else
                {
                    return new ErrorResult(MasterISS_Agent_Website_Localization.User.UserView.EmailCouldNotBeVerified);
                }
            }
            else
            {
                return new ErrorResult(MasterISS_Agent_Website_Localization.View.GenericErrorMessage);
            }

        }

        public IDataResult<List<User>> GetAll()
        {
            //if (DateTime.Now.Hour == 14)
            //{
            //    return new ErrorDataResult<List<User>>("hata var");
            //}
            return new SuccessDataResult<List<User>>(_userdal.GetAll(), MasterISS_Agent_Website_Localization.View.Successful);
        }

        public IDataResult<List<User>> GetByFilter(Expression<Func<User, bool>> filter)
        {
            return new SuccessDataResult<List<User>>(_userdal.GetAll(filter), MasterISS_Agent_Website_Localization.View.Successful);
        }

        public IDataResult<User> GetById(long userId)
        {
            //if (DateTime.Now.Hour == 1)
            //{
            //    return new ErrorDataResult<User>("hata var");
            //}
            return new SuccessDataResult<User>(_userdal.Get(u => u.Id == userId), MasterISS_Agent_Website_Localization.View.Successful);
        }
    }
}
