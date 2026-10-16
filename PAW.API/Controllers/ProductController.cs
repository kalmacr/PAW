using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAW.DataAccess.Repositories;
using PAW.DataAccess.MSSQL;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class ProductController(ILogger<ProductController> logger,
        IProductRepository productRepository) : ControllerBase
    {


    //    [HttpGet(Name = "GetProducts")]
    //    public async Task<IEnumerable<ProductDTO>> GetAll()
    //    {
    //        var products = await productRepository.ReadAsync() ?? [];
    //        return products.Select(ProductDTO.ConvertFrom);
    //    }

    //    [HttpGet("{id:int}", Name = "GetProductById")]
    //    public async Task<ActionResult<ProductDTO>> GetById(int id)
    //    {
    //        var product = await productRepository.FindAsync(id);
            
    //        return ProductDTO.ConvertFrom(product);
    //    }

    //    [HttpPost]
    //    public async Task<bool> Save([FromBody] IEnumerable<Product> product)
    //    {
    //        foreach (var p in product)
    //        {   if (p.ProductId > 0)             
    //                await productRepository.CreateAsync(p);
                
    //            else
    //                await productRepository.UpdateAsync(p);
                
    //        }
    //        return true;
    //    }
    //    [HttpDelete]
    //    public async Task<bool> Delete(Product product)
    //    {
            
    //        return await productRepository.DeleteAsync(product);
    //    }
    //}
    // Paginado de productos (máximo 25 registros por página)
        [HttpGet("paged", Name = "GetProductsPaged")]
        public async Task<ActionResult<PagedResult<ProductDTO>>> GetPaged(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 25)
        {
            var products = await productRepository.ReadAsync() ?? [];
            var totalCount = products.Count();

            var pagedItems = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(ProductDTO.ConvertFrom);

            return Ok(new PagedResult<ProductDTO>
            {
                Items = pagedItems,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            });
        }

        [HttpGet(Name = "GetProducts")]
        public async Task<IEnumerable<ProductDTO>> GetAll()
        {
            var products = await productRepository.ReadAsync() ?? [];
            return products.Select(ProductDTO.ConvertFrom);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<ActionResult<ProductDTO>> GetById(int id)
        {
            var product = await productRepository.FindAsync(id);
            if (product == null) return NotFound();

            return ProductDTO.ConvertFrom(product);
        }

        [HttpPost]
        public async Task<bool> Save([FromBody] IEnumerable<ProductDTO> productDtos)
        {
            foreach (var dto in productDtos)
            {
                var entity = ProductDTO.Convert(dto);

                if (entity.ProductId == 0)
                {
                    await productRepository.CreateAsync(entity);
                }
                else
                {
                    await productRepository.UpdateAsync(entity);
                }
            }
            return true;
        }





        [HttpDelete("{id:int}")]
        public async Task<bool> Delete(int id)
        {
            var product = await productRepository.FindAsync(id);
            if (product == null) return false;

            return await productRepository.DeleteAsync(product);
        }
    }
}
