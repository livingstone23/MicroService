namespace Manager.Services.OrderAPI.Messages
{
    /// <summary>
    /// Clases provenientes del service bus para deserealizar el mensaje
    /// Vid127
    /// </summary>
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
