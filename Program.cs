using TryConsoleApp.Services;

namespace TryConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting FMC650 Data Publisher...");
            
            using var receivingService = new DataReceivingService("amqp://localhost");
            receivingService.StartListening();
            using var publishingService = new DataPublishingService("vehicle001");
            
            Console.WriteLine("Press 'q' to quit...");
            
            // Keep the application running until user presses 'q'
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                {
                    break;
                }
            }
            
            Console.WriteLine("Stopping publisher...");
        }
    }
}