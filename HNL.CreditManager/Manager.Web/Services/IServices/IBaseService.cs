using Manager.Web.Models;

namespace Manager.Web.Services.IServices
{

    public interface IBaseService : IDisposable
    {
        ResponseDto responseModel { get; set; }
        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
