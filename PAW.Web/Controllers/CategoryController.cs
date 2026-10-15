using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Service;
 using System.Diagnostics;

namespace PAW.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        //public async Task<IActionResult> Index()
        //{
        //    var result = await _productService.GetProductAsync();
        //    return View(result);
        //}

        //public IActionResult Create()
        //{
        //    return View();
        //}



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
        // Vista principal con soporte para número de página
        public async Task<IActionResult> Index(int page = 1)
        {
            var pagedCategories = await _categoryService.GetCategoriesPagedAsync(page, 25) ?? new PagedResult<CategoryDTO>();
            return View(pagedCategories);
        }

        // Partial View para Ver Detalles
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return PartialView("DetailsPartial", category);
        }

        // Partial View para Crear
        [HttpGet]
        public IActionResult CreatePartial()
        {
            return PartialView("FormPartial", new CategoryDTO());
        }

        // Partial View para Editar
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return PartialView("FormPartial", category);
        }

        // Endpoint AJAX para guardar (Crear / Editar)
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CategoryDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _categoryService.SaveAsync(model);
            if (!success) return StatusCode(500, "Error al procesar la solicitud.");

            return Ok(new { success = true });
        }


        // Endpoint AJAX para eliminar
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return StatusCode(500, "Error al eliminar la categoria.");

            return Ok(new { success = true });
        }


    }
}
