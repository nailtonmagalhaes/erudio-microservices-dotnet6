namespace GeekShopping.OrderAPI.Messages;

#nullable disable
public class UpdatePaymentResultVO
{
    public long OrderId { get; set; }
    public string Email { get; set; }
    public bool Status { get; set; }
}
