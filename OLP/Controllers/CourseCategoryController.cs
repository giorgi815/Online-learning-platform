using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.CourseCategory;

namespace OLP.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public sealed class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService _categories;

        public CourseCategoryController(ICourseCategoryService categories)
        {
            _categories = categories;
        }


        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GetAll() 
            => Ok(await _categories.GetAllCategoriesAsync());


        [AllowAnonymous, HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id) 
            => Ok(await _categories.GetCategoryByIdAsync(id));


        [Authorize(Roles = "Admin"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseCategoryDto dto)
        {
            var created = await _categories.CreateCategoryAsync(dto);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Admin"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseCategoryDto dto)
            => Ok(await _categories.UpdateCategoryAsync(id, dto));


        [Authorize(Roles = "Admin"), HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categories.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}
