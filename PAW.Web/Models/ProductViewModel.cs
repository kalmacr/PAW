using PAW.Models.DTO;
using PAW.Web.Services;

namespace PAW.Web.Models;

public class ProductViewModel
{
    public IEnumerable<ProductDTO> Products { get; set; }
    public TipoCambioResponse TipoCambio { get; set; } 
}
