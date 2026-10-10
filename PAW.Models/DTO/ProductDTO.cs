using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PAW.Models.DTO;

    public class ProductDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }
        [JsonPropertyName("createdBy")]
        public string? CreatedBy { get; set; }
        [JsonPropertyName("coments")]
        public string Coments { get; set; }
        [JsonPropertyName("createData")]
        public DateTime CreateData { get; set; }
        [JsonPropertyName("modifiedData")]
        public DateTime ModifiedData { get; set; }

    // --- Campos de Referencia para Requisito de Clase 4 / Tarea 3 ---
    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("supplierId")]
    public int? SupplierId { get; set; }

    [JsonPropertyName("supplierName")]
    public string? SupplierName { get; set; }

    [JsonPropertyName("inventoryId")]
    public int? InventoryId { get; set; }

    [JsonPropertyName("inventoryCode")]
    public string? InventoryCode { get; set; }



    public static ProductDTO ConvertFrom(Product product)
        {
            return  new ProductDTO
                {
                Id = Guid.NewGuid(),
                ProductId = product.ProductId,
                Name = product.ProductName!,
                Description = product.Description!,
                Rating = (int) (product.Rating ?? 0),
                ModifiedBy = product.ModifiedBy,
                CreatedBy = product.CreatedBy,
                Coments = string.Empty,
                CreateData = product.LastModified ?? DateTime.Now,
                ModifiedData = product.LastModified ?? DateTime.Now,

                // Mapeo de llaves foráneas desde la entidad Product
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                InventoryId = product.InventoryId
            };

        }
        public static Product Convert(ProductDTO productDTO)
        {
            return new Product
            {
                ProductId = productDTO.ProductId,
                ProductName = productDTO.Name,
                Description = productDTO.Description,
                Rating = productDTO.Rating,
                ModifiedBy = productDTO.ModifiedBy,
                CreatedBy = productDTO.CreatedBy,
                LastModified = DateTime.Now,

                // Asignación de referencias a la entidad
                CategoryId = productDTO.CategoryId,
                SupplierId = productDTO.SupplierId,
                InventoryId = productDTO.InventoryId
            };
        }
}
// Modelo genérico para la respuesta de paginación (máximo 25 registros por página)
public class PagedResult<T>
    {
        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 25;

        [JsonPropertyName("totalPages")]
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 25));

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => PageNumber > 1;

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => PageNumber < TotalPages;
    }

