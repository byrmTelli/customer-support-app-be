using AutoMapper;
using customer_support_app.CORE.DBModels;
using customer_support_app.CORE.RequestModels.Category;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.Results.Concrete;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.DAL.Abstract;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IResult = customer_support_app.CORE.Results.Abstract.IResult;

namespace customer_support_app.SERVICE.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDal _categoryDal;
        private readonly IMapper _mapper;
        public CategoryService(ICategoryDal categoryDal,IMapper mapper)
        {
            _categoryDal = categoryDal;
            _mapper = mapper;
        }
        public async Task<IDataResult<List<CategoryViewModel>>> GetCategories()
        {
            try
            {
                var categories = await _categoryDal.GetListAsync(c => c.IsDeleted == false);
                //var categoriesViewModel =  categories.Select(c => new CategoryViewModel { Id = c.Id, Name = c.Name }).ToList();

                return new SuccessDataResult<List<CategoryViewModel>>(_mapper.Map<List<CategoryViewModel>>(categories),"Categories fetched successfully.",StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                // able to log ex.
                return new ErrorDataResult<List<CategoryViewModel>>("Something went wrong",StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<CategoryViewModel>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryDal.GetAsync(c => c.Id == id);
                
                if (category == null)
                {
                    return new ErrorDataResult<CategoryViewModel>("Bad request.",StatusCodes.Status400BadRequest);
                }

                var categoryDTO = new CategoryViewModel() { Id = category.Id, Name = category.Name };

                return new SuccessDataResult<CategoryViewModel>(categoryDTO,"Category fetched successfully.",StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CategoryViewModel>("Something went wrong", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<CategoryViewModel>> CreateCategory(CreateCategoryRequestModel model)
        {
            try
            {
                var isCategoryExist = await _categoryDal.GetAsync(c => c.Name == model.Name);

                if(isCategoryExist != null)
                {
                    return new ErrorDataResult<CategoryViewModel>("Bad Request.", StatusCodes.Status400BadRequest);
                }

                var category = new Category 
                {
                    Name = model.Name,
                    Creator = model.Creator,
                };

                var result = await _categoryDal.AddAsync(category);

                if (result == null)
                {
                    return new ErrorDataResult<CategoryViewModel>("Something went wrong while creating entity.", StatusCodes.Status500InternalServerError);
                }

                var categoryDTO = new CategoryViewModel { Id = category.Id,Name = category.Name};

                return new SuccessDataResult<CategoryViewModel>(categoryDTO,"Category created successfully.",StatusCodes.Status201Created);
            }
            catch(Exception ex)
            {
                return new ErrorDataResult<CategoryViewModel>("Something went wrong", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IResult> DeleteCategory(int id)
        {
            try
            {
                var isCategoryExist = await _categoryDal.GetAsync(c => c.Id == id && c.IsDeleted == false);

                if (isCategoryExist == null)
                {
                    return new ErrorDataResult<CategoryViewModel>("Bad Request.", StatusCodes.Status400BadRequest);
                }

                isCategoryExist.IsDeleted = true;
                await _categoryDal.UpdateAsync(isCategoryExist, "dummy");

                return new SuccessResult("Entity deleted successfully.", StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CategoryViewModel>("Something went wrong", StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<IDataResult<CategoryViewModel>> UpdateCategory(UpdateCategoryRequestModel model)
        {
            try
            {
                var isCategoryExist = await _categoryDal.GetAsync(c => c.Id == model.Id);

                if (isCategoryExist == null)
                {
                    return new ErrorDataResult<CategoryViewModel>("Bad Request.", StatusCodes.Status400BadRequest);
                }

                isCategoryExist.Name = model.Name;

                await _categoryDal.UpdateAsync(isCategoryExist,"dummy");

                var categoryDTO = new CategoryViewModel { Id = isCategoryExist.Id,Name = isCategoryExist.Name };

                return new SuccessDataResult<CategoryViewModel>(categoryDTO,"Category updated successfully.",StatusCodes.Status200OK);

            }
            catch (Exception ex)
            {
                return new ErrorDataResult<CategoryViewModel>("Something went wrong", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
