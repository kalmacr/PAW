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
    
    public class InventoryController(ILogger<InventoryController> logger, IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpGet("paged", Name = "GetInventoriesPaged")]
        public async Task<ActionResult<PagedResult<InventoryDTO>>> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 25)
        {
            var inventories = await inventoryRepository.ReadAsync() ?? [];
            var totalCount = inventories.Count();

            var pagedItems = inventories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(InventoryDTO.ConvertFrom);

            return Ok(new PagedResult<InventoryDTO>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        [HttpGet(Name = "GetInventories")]
        public async Task<IEnumerable<InventoryDTO>> GetAll()
        {
            var categories = await inventoryRepository.ReadAsync() ?? [];
            return categories.Select(InventoryDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetInventoryById")]
        public async Task<ActionResult<InventoryDTO>> GetById(int id)
        {
            var category = await inventoryRepository.FindAsync(id);
            if (category == null) return NotFound();

            return InventoryDTO.ConvertFrom(category);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<InventoryDTO> categoryDts)
        {
            foreach (var dto in categoryDts)
            {
                var entity = InventoryDTO.Convert(dto);

                if (entity.InventoryId == 0)
                {
                    await inventoryRepository.CreateAsync(entity);
                }
                else
                {
                    await inventoryRepository.UpdateAsync(entity);
                }
            }
            return true;
        }





        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            var category = await inventoryRepository.FindAsync(id);
            if (category == null) return false;

            return await inventoryRepository.DeleteAsync(category);
        }
    }
}
