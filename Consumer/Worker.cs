
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer
{
    public class Worker(ILogger<Worker> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
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
                    queue: "test",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    noWait: true //async method to create queue (tanpa menunggu response success/fail)
                );

                //Bind Queue to Exchange
                await channel.QueueBindAsync(
                    queue: "test",
                    exchange: "exchange",
                    routingKey: "test",
                    noWait: true //async method to bind queue
                );

                await channel.BasicQosAsync(
                    prefetchSize: 0, //Besaran Pesan Yang di Terima Consumen
                    prefetchCount: 1, //Besaran Pesan Yang Masuk ke Consumen
                    global: false); //Jika true, maka semua consumen akan menerima jumlah pesan yang sama

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (ch, ea) =>
                {
                    var messageRequest = Encoding.UTF8.GetString(ea.Body.ToArray());

                    await Task.Delay(5 * 60 * 1000);

                    _logger.LogInformation($"Received message: {messageRequest}");

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                };

                await channel.BasicConsumeAsync("test", false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Received message: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}
