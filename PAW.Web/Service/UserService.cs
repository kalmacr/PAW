using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Service;

public interface IUserService
{
 
    //Task<IEnumerable<ProductDTO>> GetProductAsync();
    Task<PagedResult<UserDTO>?> GetUsersPagedAsync(int page = 1, int pageSize = 25);
    Task<UserDTO?> GetByIdAsync(string id);
    Task<bool> SaveAsync(UserDTO userDto);
    Task<bool> DeleteAsync(string id);



}


public class UserService(IRestProvider restProvider) : ServiceBase, IUserService
{
    private readonly string _path = "User";


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
            return null; // Retorna null si no se encuentra o hay error
        }
    }



    //public async Task<bool> SaveAsync(ProductDTO productDto)
    //{
    //    var list = new List<ProductDTO> { productDto };
    //    var jsonContent = JsonProvider.Serialize(list);

    //    var response = await restProvider.PostAsync(
    //        SetPathUrl(_path),
    //        jsonContent);

    //    return !string.IsNullOrEmpty(response);
    //}

    public async Task<bool> SaveAsync(UserDTO userDTO)
    {
        var endpoint = SetPathUrl(_path);

        // El POST se envía a la colección general /product
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
            // Si la API falla, captura el error  500
            return false;
        }
    }

   
}

