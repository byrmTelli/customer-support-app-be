using customer_support_app.CORE.DataAccess.EntityFramework;
using customer_support_app.CORE.DBModels;
using customer_support_app.DAL.Abstract;
using customer_support_app.DAL.Context.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.DAL.Concrete
{
    public class CategoryDal:EfEntityRepositoryBase<Category,AppDbContext>,ICategoryDal
    {
        private readonly AppDbContext _context;
        public CategoryDal(AppDbContext context):base(context)
        {
            _context = context;
        }
    }
}
