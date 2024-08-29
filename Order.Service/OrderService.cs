using MassTransit;
using ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Service
{
    public class OrderService(ServiceBus.IBus bus, IPublishEndpoint publishEndpoint) : IOrderService
    {
        public async Task Create()
        {
            var orderCreatedEvent = new OrderCreatedEvent(10, new Dictionary<int, int>()
            {
                { 1, 5 },{2,5 }
            });

            // await bus.Send(orderCreatedEvent, BusConstants.OrderCreatedEventExchange);

            CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.CancelAfter(delay: TimeSpan.FromSeconds(60));

            await publishEndpoint.Publish(orderCreatedEvent, pipeline =>
            {
               pipeline.SetAwaitAck(true);
               pipeline.Durable = true;
            }, cancellationTokenSource.Token);
        }
    }
}
