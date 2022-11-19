using GeekShopping.MessageBus;

namespace GeekShopping.PaymentAPI.Messages;

#nullable disable
public class UpdatePaymentResultMessage : BaseMessage
{
    public long OrderId { get; set; }
    public string Email { get; set; }
    public bool Status { get; set; }
}
