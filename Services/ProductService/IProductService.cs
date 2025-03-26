using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;

namespace Ecommerce.Services.ProductService;

public interface IProductService
{
    Task<(HttpError?, List<Product>)> GetProductsAsync();
    Task<(HttpError?, Product)> GetProductByIdAsync(int id);
    Task<(HttpError?, Product)> PostProductAsync(AddProductDto request);
    Task<(HttpError?, Product)> PatchProductAsync(int id, Dictionary<string, object> updates);
    Task<HttpError?> DeleteProductAsync(int id);
}
