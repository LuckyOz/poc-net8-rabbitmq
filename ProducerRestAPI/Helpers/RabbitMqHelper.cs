
using RabbitMQ.Client;

namespace ProducerRestAPI.Helpers
{
    public class RabbitMqHelper
    {
        public async Task<bool> Send(string data, string queue)
        {
            //Setup Connection
            ConnectionFactory factory = new()
            {
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                HostName = "localhost"
            };

            IConnection conn = await factory.CreateConnectionAsync();
            IChannel channel = await conn.CreateChannelAsync();

            //Setup Exchange
            await channel.ExchangeDeclareAsync(
                exchange: "exchange",
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: false,
                noWait: true //async method to create exchange (tanpa menunggu response success/fail)
            );

            //Setup Queue
            await channel.QueueDeclareAsync(
                queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                noWait: true //async method to create queue (tanpa menunggu response success/fail)
            );

            //Bind Queue to Exchange
            await channel.QueueBindAsync(
                queue: queue,
                exchange: "exchange",
                routingKey: queue,
                noWait: true //async method to bind queue
            );

            //Publish Message
            byte[] messageBodyBytes = System.Text.Encoding.UTF8.GetBytes("Hello, world!");

            BasicProperties props = new()
            {
                ContentType = "text/plain",
                DeliveryMode = DeliveryModes.Persistent,
                Expiration = "36000000"
            };

            await channel.BasicPublishAsync(
                exchange: "exchange",
                routingKey: queue,
                mandatory: true,
                basicProperties: props,
                body: messageBodyBytes
            );

            await channel.CloseAsync(); //Close Connection
            await conn.CloseAsync(); //Close Connection
            await channel.DisposeAsync(); //Clear Chache or Temporary
            await conn.DisposeAsync(); //Clear Chache or Temporary

            return true;
        }
    }
}
