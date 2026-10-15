using System.Text.Json.Serialization;

namespace PAW.Models.DTO;

    public class InventoryDTO
{
    
        [JsonPropertyName("inventoryId")]
        public int InventoryId { get; set; }
        [JsonPropertyName("unitPrice")]
        public decimal? UnitPrice { get; set; }
        [JsonPropertyName("unitsInStock")]
        public int? UnitsInStock { get; set; }
        [JsonPropertyName("lastUpdated")]
        public DateTime? LastUpdated { get; set; }
        [JsonPropertyName("productId")]
        public int? ProductId { get; set; }
        [JsonPropertyName("dateAdded")]
        public DateTime? DateAdded { get; set; }
        [JsonPropertyName("modifiedBy")]
        public string? ModifiedBy { get; set; }


    public static InventoryDTO ConvertFrom(Inventory inventory)
        {
            return  new InventoryDTO
            {
                InventoryId = inventory.InventoryId,
                UnitPrice = inventory.UnitPrice,
                UnitsInStock = inventory.UnitsInStock,
                LastUpdated = inventory.LastUpdated ?? DateTime.Now,
                ProductId = inventory.ProductId,
                DateAdded = inventory.DateAdded,
                ModifiedBy = inventory.ModifiedBy


            };

        }
        public static Inventory Convert(InventoryDTO inventoryDTO)
        {
            return new Inventory
            {
                InventoryId = inventoryDTO.InventoryId,
                UnitPrice = inventoryDTO.UnitPrice,
                UnitsInStock = inventoryDTO.UnitsInStock,
                LastUpdated = inventoryDTO.LastUpdated,
                ProductId =inventoryDTO.ProductId, 
                DateAdded = DateTime.Now,
                ModifiedBy = inventoryDTO.ModifiedBy

            };
        }
}
 