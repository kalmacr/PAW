using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Service;

public interface ITaskService
{
    Task<IEnumerable<TaskDTO>?> GetAllAsync();
    Task<TaskDTO?> GetByIdAsync(string id);
    Task<bool> SaveAsync(TaskDTO taskDto);
    Task<bool> DeleteAsync(string id);
}

public class TaskService(IRestProvider restProvider) : ServiceBase, ITaskService
{
    // Ruta al endpoint de la MinimalAPI
    protected string BaseUrl { get; set; } = "https://localhost:7072/";
    private string SetTaskPath(string name) => $"{BaseUrl}{name}";
    public async Task<IEnumerable<TaskDTO>?> GetAllAsync()
    {
        var endpoint = SetTaskPath("task");
        if (!endpoint.EndsWith("/")) endpoint += "/";

        var response = await restProvider.GetAsync(endpoint, string.Empty);
        return await JsonProvider.DeserializeAsync<IEnumerable<TaskDTO>>(response);
    }
    public async Task<TaskDTO?> GetByIdAsync(string id)
    {
        try
        {
            var endpoint = SetTaskPath("task");
            if (!endpoint.EndsWith("/")) endpoint += "/";

            var response = await restProvider.GetAsync(endpoint, id);
            return await JsonProvider.DeserializeAsync<TaskDTO>(response);
        }
        catch
        {
            return null;
        }
    }


    public async Task<bool> SaveAsync(TaskDTO taskDto)
    {
        var endpoint = SetTaskPath("task"); 

        var jsonContent = JsonProvider.Serialize(taskDto);

        var response = await restProvider.PostAsync(endpoint, jsonContent);
        return !string.IsNullOrEmpty(response);
    }
    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            var endpoint = SetTaskPath("task");
            if (!endpoint.EndsWith("/")) endpoint += "/";

            var response = await restProvider.DeleteAsync(endpoint, id);
            return !string.IsNullOrEmpty(response);
        }
        catch
        {
            return false;
        }
    }
}