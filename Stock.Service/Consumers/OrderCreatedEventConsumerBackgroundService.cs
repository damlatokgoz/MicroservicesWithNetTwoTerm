using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumerBackgroundService(IBus bus) :BackgroundService
    {
        private IModel? Channel { get; set; }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var channel = bus.GetChannel();

            channel.QueueDeclare(queue: BusConstants.StockCreatedEventQueue,
                durable: true,
                exclusive: true,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(BusConstants.StockCreatedEventQueue, BusConstants.OrderCreatedEventExchange,"",null);

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var consumer = new EventingBasicConsumer(Channel);
            Channel.BasicConsume(BusConstants.StockCreatedEventQueue, true, consumer);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message);


                Console.WriteLine($"Gelen Event:{orderCreatedEvent.orderId}");




                Channel!.BasicAck(ea.DeliveryTag, false);
            };

            return Task.CompletedTask;

        }

        private void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
