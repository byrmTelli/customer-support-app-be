using customer_support_app.CORE.RequestModels.Category;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace customer_support_app.SERVICE.Abstract
{
    public interface ICategoryService
    {
        Task<IDataResult<List<CategoryViewModel>>> GetCategories();
        Task<IDataResult<CategoryViewModel>> GetCategoryById(int id);
        Task<IDataResult<CategoryViewModel>> CreateCategory(CreateCategoryRequestModel model);
        Task<IResult> DeleteCategory(int id);
        Task<IDataResult<CategoryViewModel>> UpdateCategory(UpdateCategoryRequestModel model);
    }
}
