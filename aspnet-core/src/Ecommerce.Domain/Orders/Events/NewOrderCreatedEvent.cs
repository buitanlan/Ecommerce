namespace Ecommerce.Orders.Events;

public class NewOrderCreatedEvent
{
     public string CustomerEmail { get; set; }
     public string Message { get; set; }
}
