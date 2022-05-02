namespace Manager.Services.OrderAPI.Messages
{
    /// <summary>
    /// Clase que actualiza el estatus del payment
    /// </summary>
    public class UpdatePaymentResultMessage
    {
        public int OrderId { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }

    }
}
