using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Service;

namespace PAW.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        
        public async Task<IActionResult> Index(int page = 1)
        {
            var pagedUsers = await _userService.GetUsersPagedAsync(page, 25) ?? new PagedResult<UserDTO>();
            return View(pagedUsers);
        }

        // Partial View para Ver Detalles
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return PartialView("DetailsPartial", user);
        }

        // Partial View para Crear
        [HttpGet]
        public IActionResult CreatePartial()
        {
            return PartialView("FormPartial", new UserDTO());
        }

        // Partial View para Editar
        [HttpGet]
        public async Task<IActionResult> EditPartial(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return PartialView("FormPartial", user);
        }

        // Endpoint AJAX para guardar (Crear / Editar)
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] UserDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _userService.SaveAsync(model);
            if (!success) return StatusCode(500, "Error al procesar la solicitud.");

            return Ok(new { success = true });
        }


        // Endpoint AJAX para eliminar
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteAsync(id);
            if (!success) return StatusCode(500, "Error al eliminar el usuario.");

            return Ok(new { success = true });
        }


    }
}
