using static Manager.Web.SD;

namespace Manager.Web.Models
{
    /// <summary>
    /// Class for sending a generic heading implementation
    /// </summary>
    public class ApiRequest
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }
    }
}
