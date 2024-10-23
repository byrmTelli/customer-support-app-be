using customer_support_app.CORE.DataAccess;
using customer_support_app.CORE.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.User;

namespace customer_support_app.DAL.Abstract
{
    public interface IUserDal:IEntityRepository<AppUser>
    {
        Task<IDataResult<List<HelpdeskViewModel>>>GetHelpdesksAsync();
        Task<IDataResult<List<UserProfileViewModel>>> GetHelpDesksForAdminPanelAsync();
        Task<IDataResult<List<CustomerProfileViewModel>>> GetCustomersForAdminPanelAsync();
    }
}
