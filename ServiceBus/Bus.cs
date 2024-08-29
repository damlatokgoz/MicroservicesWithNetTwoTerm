using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceBus
{
    public class Bus(IOptions<BusOptions> busOptions) : IBus
    {
        public Task Send<T>(T message, string exchangeName) where T : class
        {
            //using var channel = GetChannel();
            //channel.ConfirmSelect();

            //channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

            //var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));


            //channel.BasicPublish(exchange:exchangeName,
            //    routingKey:"",
            //    basicProperties:null,
            //    body:body);

            //channel.WaitForConfirms(timeout:TimeSpan.FromMinutes(1)); //rabbitmq mesajı başarılı olarak kaydedene kadar bekler.
            return Task.CompletedTask;

        }

        public IModel GetChannel()
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(busOptions.Value.Url)
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            return channel;
        }

    }
}