using Manager.Web.Models;

namespace Manager.Web.Services.IServices
{
    public interface IProductService : IBaseService
    {

        //sin token
        //Task<T> GetAllProductsAsync<T>();
        //Task<T> GetProductByIdAsync<T>(int id);
        //Task<T> CreateProductAsync<T>(ProductDto productDto);
        //Task<T> UpdateProductAsync<T>(ProductDto productDto);
        //Task<T> DeleteProductAsync<T>(int id);




        //60.1 Metodos con Token
        Task<T> GetAllProductsAsync<T>(string token);
        Task<T> GetProductByIdAsync<T>(int id, string token);
        Task<T> CreateProductAsync<T>(ProductDto productDto, string token);
        Task<T> UpdateProductAsync<T>(ProductDto productDto, string token);
        Task<T> DeleteProductAsync<T>(int id, string token);


    }
}
