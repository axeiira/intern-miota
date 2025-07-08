using TryConsoleApp.Models;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TryConsoleApp.Services
{
    public class DataReceivingService : IDisposable
    {
        private readonly RabbitMQConsumer _consumer;

        public DataReceivingService(string connectionString = "amqp://localhost")
        {
            _consumer = new RabbitMQConsumer(connectionString);
        }

        public void StartListening()
        {
            _consumer.StartConsuming();
            Console.WriteLine("Started listening for FMC650 data...");
            Console.WriteLine("Press [CTRL+C] to stop listening");
        }

        public void StopListening()
        {
            _consumer.StopConsuming();
        }

        public void Dispose()
        {
            _consumer?.Dispose();
        }
    }

    public class RabbitMQConsumer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "fmc650.exchange";
        private const string QueueName = "fmc650.telemetry.queue";

        public RabbitMQConsumer(string connectionString)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(connectionString) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the same exchange as publisher
            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false
            );

            // Declare queue
            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            // Bind queue to exchange with routing pattern for all vehicles
            _channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: "fmc650.*.telemetry"
            );
        }

        public void StartConsuming()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    // Extract vehicle ID from routing key (fmc650.vehicleId.telemetry)
                    var routingParts = routingKey.Split('.');
                    var vehicleId = routingParts.Length >= 2 ? routingParts[1] : "unknown";

                    // Deserialize the FMC650Data
                    var fmc650Data = JsonSerializer.Deserialize<FMC650Data>(message);
                    
                    Console.WriteLine("=====================================");
                    Console.WriteLine($"Received data at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"Vehicle ID: {vehicleId}");
                    Console.WriteLine($"Routing Key: {routingKey}");
                    Console.WriteLine("-------------------------------------");
                    
                    if (fmc650Data != null)
                    {
                        Console.WriteLine($"Device ID: {fmc650Data.DeviceId}");
                        Console.WriteLine($"Timestamp: {fmc650Data.Timestamp}");
                        Console.WriteLine($"Latitude: {fmc650Data.Latitude}");
                        Console.WriteLine($"Longitude: {fmc650Data.Longitude}");
                        Console.WriteLine($"Speed: {fmc650Data.Speed}");
                        Console.WriteLine($"Heading: {fmc650Data.Heading}");
                        Console.WriteLine($"Fuel: {fmc650Data.Fuel}");
                        Console.WriteLine($"Engine Hours: {fmc650Data.EngineHours}");
                        Console.WriteLine($"Ignition Status: {fmc650Data.IgnitionStatus}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to deserialize FMC650Data");
                        Console.WriteLine($"Raw message: {message}");
                    }
                    
                    Console.WriteLine("=====================================");
                    Console.WriteLine();

                    // Acknowledge the message
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    // Reject message and don't requeue on error
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public void StopConsuming()
        {
            _channel?.Close();
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}