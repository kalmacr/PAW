using APW.Architecture;
using PAW.Architecture.Providers;
using PAW.Models.DTO;

namespace PAW.Web.Service;

public interface ICategoryService
{
 
    //Task<IEnumerable<ProductDTO>> GetProductAsync();
    Task<PagedResult<CategoryDTO>?> GetCategoriesPagedAsync(int page = 1, int pageSize = 25);
    Task<CategoryDTO?> GetByIdAsync(string id);
    Task<bool> SaveAsync(CategoryDTO categoryDto);
    Task<bool> DeleteAsync(string id);



}

//public class ProductService : ServiceBase, IProductService
//{
//    private readonly string _path = "product";
//    private readonly IRestProvider _restProvider;
//    public ProductService(IRestProvider restProvider)
//    {
//        _restProvider = restProvider;

//    }

//    public async Task<IEnumerable<ProductDTO>> GetProductAsync()
//    {
//        var response = await _restProvider.GetAsync(SetPathUrl(_path), id: null);
//        var products = await JsonProvider.DeserializeAsync<IEnumerable<ProductDTO>>(response);
//        return products;
//    }

//    public async Task<string?> GetByIdAsync(String id)
//    {
//        var response = await _restProvider.GetAsync(SetPathUrl(_path), id);
//        var product = await JsonProvider.DeserializeAsync<ProductDTO>(response);
//        return product?.Name;

//    }
//}
public class CategoryService(IRestProvider restProvider) : ServiceBase, ICategoryService
{
    private readonly string _path = "category";


    public async Task<PagedResult<CategoryDTO>?> GetCategoriesPagedAsync(int page = 1, int pageSize = 25)
    {
        var response = await restProvider.GetAsync($"{SetPathUrl(_path)}/paged?page={page}&pageSize={pageSize}", id: null);
        return await JsonProvider.DeserializeAsync<PagedResult<CategoryDTO>>(response);
    }

    //public async Task<ProductDTO?> GetByIdAsync(string id)
    //{
    //    var response = await restProvider.GetAsync(SetPathUrl(_path), id);
    //    return await JsonProvider.DeserializeAsync<ProductDTO>(response);
    //}

    public async Task<CategoryDTO?> GetByIdAsync(string id)
    {
        try
        {
            var endpoint = SetPathUrl(_path);
            if (!endpoint.EndsWith("/"))
                endpoint += "/";

            var response = await restProvider.GetAsync(endpoint, id);
            return await JsonProvider.DeserializeAsync<CategoryDTO>(response);
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

    public async Task<bool> SaveAsync(CategoryDTO categoryDto)
    {
        var endpoint = SetPathUrl(_path);

        // El POST se envía a la colección general /product
        var list = new List<CategoryDTO> { categoryDto };
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

