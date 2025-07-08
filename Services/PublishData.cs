using TryConsoleApp.Models;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace TryConsoleApp.Services
{
    public class DataPublishingService : IDisposable
    {
        private readonly RabbitMQPublisher _publisher;
        private readonly FMC650Generator _generator;
        private readonly string _vehicleId;
        private readonly Timer _timer;

        public DataPublishingService(string vehicleId = "vehicle001", string connectionString = "amqp://localhost")
        {
            _publisher = new RabbitMQPublisher(connectionString);
            _generator = new FMC650Generator();
            _vehicleId = vehicleId;

            // Create timer that fires every 10 seconds
            _timer = new Timer(PublishData, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void PublishData(object? state)
        {
            var data = _generator.GenerateData();
            _publisher.PublishFMC650Data(data, _vehicleId);
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _publisher?.Dispose();
        }
    }

    public class RabbitMQPublisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "fmc650.exchange";

        public RabbitMQPublisher(string connectionString = "amqp://localhost")
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare topic exchange
            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );
        }

        public void PublishFMC650Data(FMC650Data data, string vehicleId = "default")
        {
            var routingKey = $"fmc650.{vehicleId}.telemetry";
            var message = JsonSerializer.Serialize(data);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.ContentType = "application/json";

            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}