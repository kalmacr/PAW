using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Service;
 using System.Diagnostics;

namespace PAW.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        
        public async Task<IActionResult> Index(int page = 1)
        {
            var pagedCategories = await _inventoryService.GetInventoriesPagedAsync(page, 25) ?? new PagedResult<InventoryDTO>();
            return View(pagedCategories);
        }

        // Partial View para Ver Detalles
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            var inventory = await _inventoryService.GetByIdAsync(id);
            if (inventory == null) return NotFound();
            return PartialView("DetailsPartial", inventory);
        }

        // Partial View para Crear
        [HttpGet]
        public IActionResult CreatePartial()
        {
            return PartialView("FormPartial", new InventoryDTO());
        }

        // Partial View para Editar
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            var category = await _inventoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return PartialView("FormPartial", category);
        }

        // Endpoint AJAX para guardar (Crear / Editar)
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] InventoryDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _inventoryService.SaveAsync(model);
            if (!success) return StatusCode(500, "Error al procesar la solicitud.");

            return Ok(new { success = true });
        }


        // Endpoint AJAX para eliminar
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _inventoryService.DeleteAsync(id);
            if (!success) return StatusCode(500, "Error al eliminar la categoria.");

            return Ok(new { success = true });
        }


    }
}
