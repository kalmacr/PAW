using Microsoft.AspNetCore.Mvc;
using PAW.Models.DTO;
using PAW.Web.Service;

namespace PAW.Web.Controllers
{
    public class TaskController(ITaskService taskService) : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Endpoint para alimentar la tabla vía AJAX
        [HttpGet]
        public async Task<IActionResult> GetTaskList()
        {
            var tasks = await taskService.GetAllAsync() ?? Enumerable.Empty<TaskDTO>();
            return PartialView("_TablePartial", tasks); // O renderizar con JS
        }

        [HttpGet]
        public async Task<IActionResult> CreatePartial()
        {
            return PartialView("_FormPartial", new TaskDTO());
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var task = await taskService.GetByIdAsync(id.ToString());
            if (task == null) return NotFound();

            return PartialView("_FormPartial", task);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] TaskDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await taskService.SaveAsync(model);
            if (!success) return StatusCode(500, "Error al procesar la solicitud.");

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await taskService.DeleteAsync(id.ToString());
            return Ok(new { success });
        }
    }
}