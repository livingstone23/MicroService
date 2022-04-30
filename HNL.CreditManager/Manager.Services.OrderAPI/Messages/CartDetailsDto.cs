namespace Manager.Services.OrderAPI.Messages
{

    /// <summary>
    /// Clases provenientes del service bus para deserealizar el mensaje
    /// Vid127
    /// </summary>
    public class CartDetailsDto
    {
        public int CartDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto Product { get; set; }
        public int Count { get; set; }

    }



}
