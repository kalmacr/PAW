using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Service;

public interface IInventoryService
{
 
    //Task<IEnumerable<ProductDTO>> GetProductAsync();
    Task<PagedResult<InventoryDTO>?> GetInventoriesPagedAsync(int page = 1, int pageSize = 25);
    Task<InventoryDTO?> GetByIdAsync(string id);
    Task<bool> SaveAsync(InventoryDTO inventoryDto);
    Task<bool> DeleteAsync(string id);



}


public class InventoryService(IRestProvider restProvider) : ServiceBase, IInventoryService
{
    private readonly string _path = "inventory";


    public async Task<PagedResult<InventoryDTO>?> GetInventoriesPagedAsync(int page = 1, int pageSize = 25)
    {
        var response = await restProvider.GetAsync($"{SetPathUrl(_path)}/paged?page={page}&pageSize={pageSize}", id: null);
        return await JsonProvider.DeserializeAsync<PagedResult<InventoryDTO>>(response);
    }

    //public async Task<ProductDTO?> GetByIdAsync(string id)
    //{
    //    var response = await restProvider.GetAsync(SetPathUrl(_path), id);
    //    return await JsonProvider.DeserializeAsync<ProductDTO>(response);
    //}

    public async Task<InventoryDTO?> GetByIdAsync(string id)
    {
        try
        {
            var endpoint = SetPathUrl(_path);
            if (!endpoint.EndsWith("/"))
                endpoint += "/";

            var response = await restProvider.GetAsync(endpoint, id);
            return await JsonProvider.DeserializeAsync<InventoryDTO>(response);
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

    public async Task<bool> SaveAsync(InventoryDTO inventoryDTO)
    {
        var endpoint = SetPathUrl(_path);

        // El POST se envía a la colección general /product
        var list = new List<InventoryDTO> { inventoryDTO };
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

