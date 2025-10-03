namespace ClientAzureFunction.Configuration
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }

    public class AppSettings
    {
        public int MaxRetries { get; set; } = 3;
        public int DelayBetweenRetries { get; set; } = 5000; // milliseconds
        public bool LogReceivedMessages { get; set; } = true;
        public int ProcessingTimeout { get; set; } = 30000; // milliseconds
    }
}