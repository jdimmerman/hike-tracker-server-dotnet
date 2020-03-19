using System;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace app.Services
{
    public class HikeQueue
    {
        private static IQueueClient _queueClient;
        private static HikeQueue _instance;
        public static HikeQueue Instance { get {
                if (_instance == null) throw new Exception("HikeQueue must be initialized before using");
                return _instance;
        } }

        private HikeQueue(string serviceBusConnectionString, string queueName)
        {
            _queueClient = new QueueClient(serviceBusConnectionString, queueName);
            RegisterOnMessageHandlerAndReceiveMessages();
        }

        public static HikeQueue Initialize(string serviceBusConnectionString, string queueName)
        {
            if (_instance == null)
            {
                _instance = new HikeQueue(serviceBusConnectionString, queueName);
                return _instance;
            }
            throw new Exception("HikeQueue already initialized");
        }

        public async Task EnqueueAsync(string messageBody)
        {
            try
            {
                Message message = new Message(Encoding.UTF8.GetBytes(messageBody));
                await _queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 10,
                AutoComplete = false,
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
            Console.WriteLine("Registered handler");
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
