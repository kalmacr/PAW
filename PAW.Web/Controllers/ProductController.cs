using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAW.Models.DTO;
using PAW.Web.Models;
using PAW.Web.Service;
 using System.Diagnostics;

namespace PAW.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
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
            var pagedProducts = await _productService.GetProductsPagedAsync(page, 25) ?? new PagedResult<ProductDTO>();
            return View(pagedProducts);
        }

        // Partial View para Ver Detalles
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return PartialView("DetailsPartial", product);
        }

        // Partial View para Crear
        [HttpGet]
        public IActionResult CreatePartial()
        {
            return PartialView("FormPartial", new ProductDTO());
        }

        // Partial View para Editar
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return PartialView("FormPartial", product);
        }

        // Endpoint AJAX para guardar (Crear / Editar)
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ProductDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _productService.SaveAsync(model);
            if (!success) return StatusCode(500, "Error al procesar la solicitud.");

            return Ok(new { success = true });
        }


        // Endpoint AJAX para eliminar
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _productService.DeleteAsync(id);
            if (!success) return StatusCode(500, "Error al eliminar el producto.");

            return Ok(new { success = true });
        }


    }
}
