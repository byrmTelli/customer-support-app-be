using customer_support_app.CORE.DataAccess;
using customer_support_app.CORE.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Abstract
{
    public interface ICategoryDal:IEntityRepository<Category>
    {
    }
}
