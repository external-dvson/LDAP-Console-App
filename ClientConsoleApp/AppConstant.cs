namespace ClientConsoleApp
{
    public static class AppConstant
    {
        public static class Messages
        {
            public const string ApplicationStarted = "🚀 Client Console App Started";
            public const string ApplicationStopped = "🛑 Client Console App Stopped";
            public const string ConnectedToServiceBus = "✅ Connected to Service Bus: {0}";
            public const string ConnectionFailed = "❌ Service Bus connection failed: {0}";
            public const string MessageReceived = "📨 Message received from queue: {0}";
            public const string MessageProcessed = "✅ Message processed successfully";
            public const string MessageProcessingFailed = "❌ Message processing failed: {0}";
            public const string GroupProcessed = "📊 Group '{0}' processed with {1} users";
            public const string WaitingForMessages = "⏳ Waiting for messages from LDAP Console App...";
            public const string NoMessagesReceived = "ℹ️ No messages received in the last {0} seconds";
            public const string ErrorDeserializingMessage = "❌ Error deserializing message: {0}";
            public const string UnexpectedError = "💥 Unexpected error: {0}";
            public const string PressKeyToExit = "Press any key to exit...";
        }

        public static class Configuration
        {
            public const string ServiceBusSettingsSection = "ServiceBusSettings";
            public const string AppSettingsSection = "AppSettings";
            public const string AppSettingsFile = "appsettings.json";
        }

        public static class Timeouts
        {
            public const int DefaultProcessingTimeout = 30000; // 30 seconds
            public const int DefaultRetryDelay = 5000; // 5 seconds
            public const int MessageWaitTimeout = 10000; // 10 seconds
        }
    }
}