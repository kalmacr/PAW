// Usamos un alias para evitar el choque de nombres con la clase Task de C#
using TaskEntity = PAW.Models.Task;

namespace PAW.MinimalAPI.Services;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllAsync();
   Task<TaskEntity?> GetByIdAsync(int id);
    Task<bool> SaveAsync(TaskEntity task);
   Task<bool> DeleteAsync(int id);
}

public class InMemoryTaskRepository : ITaskRepository
{
    private static readonly List<TaskEntity> _tasks = new()
    {
        new TaskEntity { Id = 1, Name = "Diseñar Base de Datos", Description = "Crear esquemas en SQL Server", Status = "Pendiente", CreatedAt = DateTime.Now, ModifiedBy = "Admin" },
        new TaskEntity { Id = 2, Name = "Configurar Minimal API", Description = "Endpoints REST sin controladores", Status = "En Progreso", CreatedAt = DateTime.Now, ModifiedBy = "Admin" }
    };

    public Task<IEnumerable<TaskEntity>> GetAllAsync()
        => Task.FromResult<IEnumerable<TaskEntity>>(_tasks);

    public Task<TaskEntity?> GetByIdAsync(int id)
        => Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

    public Task<bool> SaveAsync(TaskEntity task)
    {
        if (task.Id == 0)
        {
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            task.CreatedAt = DateTime.Now;
            _tasks.Add(task);
        }
        else
        {
            var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
            if (existing != null)
            {
                existing.Name = task.Name;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.DueDate = task.DueDate;
                existing.LastModified = DateTime.Now;
                existing.ModifiedBy = task.ModifiedBy;
            }
        }
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        if (task != null)
        {
            _tasks.Remove(task);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}