using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Service;

public interface IUserService
{
    Task<PagedResult<UserDTO>?> GetUsersPagedAsync(int page = 1, int pageSize = 25);
    Task<UserDTO?> GetByIdAsync(string id);
    Task<bool> SaveAsync(UserDTO userDto);
    Task<bool> DeleteAsync(string id);



}


public class UserService(IRestProvider restProvider) : ServiceBase, IUserService
{
    private readonly string _path = "Users";


    public async Task<PagedResult<UserDTO>?> GetUsersPagedAsync(int page = 1, int pageSize = 25)
    {
        var response = await restProvider.GetAsync($"{SetPathUrl(_path)}/paged?page={page}&pageSize={pageSize}", id: null);
        return await JsonProvider.DeserializeAsync<PagedResult<UserDTO>>(response);
    }

    //public async Task<ProductDTO?> GetByIdAsync(string id)
    //{
    //    var response = await restProvider.GetAsync(SetPathUrl(_path), id);
    //    return await JsonProvider.DeserializeAsync<ProductDTO>(response);
    //}

    public async Task<UserDTO?> GetByIdAsync(string id)
    {
        try
        {
            var endpoint = SetPathUrl(_path);
            if (!endpoint.EndsWith("/"))
                endpoint += "/";

            var response = await restProvider.GetAsync(endpoint, id);
            return await JsonProvider.DeserializeAsync<UserDTO>(response);
        }
        catch (Exception ex)
        {
            return null; 
        }
    }




    public async Task<bool> SaveAsync(UserDTO userDTO)
    {
        var endpoint = SetPathUrl(_path);

        // El POST se envía a la colección general 
        var list = new List<UserDTO> { userDTO };
        var jsonContent = JsonProvider.Serialize(list);

        var response = await restProvider.PostAsync(endpoint, jsonContent);
        return !string.IsNullOrEmpty(response);
    }


    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            
            var endpoint = SetPathUrl(_path);

            if (!endpoint.EndsWith("/"))
                endpoint += "/";

            var response = await restProvider.DeleteAsync(endpoint, id);

            return !string.IsNullOrEmpty(response);
        }
        catch (Exception ex)
        {
            return false;
        }
    }

   
}

