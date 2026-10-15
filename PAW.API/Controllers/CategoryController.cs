using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAW.DataAccess.Repositories;
using PAW.DataAccess.MSSQL;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository) : ControllerBase
    {
        [HttpGet("paged", Name = "GetCategoriesPaged")]
        public async Task<ActionResult<PagedResult<CategoryDTO>>> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 25)
        {
            var categories = await categoryRepository.ReadAsync() ?? [];
            var totalCount = categories.Count();

            var pagedItems = categories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(CategoryDTO.ConvertFrom);

            return Ok(new PagedResult<CategoryDTO>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<IEnumerable<CategoryDTO>> GetAll()
        {
            var categories = await categoryRepository.ReadAsync() ?? [];
            return categories.Select(CategoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryDTO>> GetById(int id)
        {
            var category = await categoryRepository.FindAsync(id);
            if (category == null) return NotFound();

            return CategoryDTO.ConvertFrom(category);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<CategoryDTO> categoryDts)
        {
            foreach (var dto in categoryDts)
            {
                var entity = CategoryDTO.Convert(dto);

                if (entity.CategoryId == 0)
                {
                    await categoryRepository.CreateAsync(entity);
                }
                else
                {
                    await categoryRepository.UpdateAsync(entity);
                }
            }
            return true;
        }





        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            var category = await categoryRepository.FindAsync(id);
            if (category == null) return false;

            return await categoryRepository.DeleteAsync(category);
        }
    }
}
