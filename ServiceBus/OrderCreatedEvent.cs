namespace ServiceBus
{
    public record OrderCreatedEvent(int orderId, Dictionary<int, int> stockInfo);


}
