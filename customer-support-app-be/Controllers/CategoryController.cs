using customer_support_app.CORE.RequestModels.Category;
using customer_support_app.CORE.Results.Abstract;
using customer_support_app.CORE.ViewModels.Category;
using customer_support_app.SERVICE.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace customer_support_app.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet(nameof(GetCategories))]
        [ProducesResponseType(typeof(IDataResult<List<CategoryViewModel>>),200)]
        [ProducesResponseType(typeof(IDataResult<List<CategoryViewModel>>),500)]
        public async Task<IActionResult> GetCategories()
        {
            var response = await _categoryService.GetCategories();
            return StatusCode(response.Code, response);
        }

        [HttpGet(nameof(GetCategoryById))]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 500)]
        public async Task<IActionResult> GetCategoryById([FromQuery]int id)
        {
            var response = await _categoryService.GetCategoryById(id);
            return StatusCode(response.Code, response);
        }

        [HttpPost(nameof(CreateCategory))]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 204)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 500)]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestModel request)
        {
            var response = await _categoryService.CreateCategory(request);
            return StatusCode(response.Code, response);
        }
        [HttpDelete(nameof(DeleteCategory))]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 204)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 500)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategory(id);
            return StatusCode(response.Code, response);
        }
        [HttpPut(nameof(UpdateCategory))]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 200)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 400)]
        [ProducesResponseType(typeof(IDataResult<CategoryViewModel>), 500)]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequestModel request)
        {
            var response = await _categoryService.UpdateCategory(request);
            return StatusCode(response.Code, response);
        }
    }
}
