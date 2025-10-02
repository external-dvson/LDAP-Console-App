using System;
using ClientAzureFunction.Models;

namespace ClientAzureFunction.Helpers
{
    public static class DisplayHelper
    {
        public static void DisplayApplicationHeader()
        {
            Console.WriteLine();
            Console.WriteLine("══════════════════════════════════════════════════════════════════");
            Console.WriteLine("🔌 CLIENT CONSOLE APP - Azure Service Bus Consumer");
            Console.WriteLine("══════════════════════════════════════════════════════════════════");
            Console.WriteLine($"📅 Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"🖥️  Environment: {Environment.MachineName}");
            Console.WriteLine($"👤 User: {Environment.UserDomainName}\\{Environment.UserName}");
            Console.WriteLine("══════════════════════════════════════════════════════════════════");
            Console.WriteLine();
        }

        public static void DisplayReceivedGroup(Group group)
        {
            Console.WriteLine();
            Console.WriteLine("┌────────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"│ 📋 GROUP: {group.GroupName.PadRight(50)} │");
            Console.WriteLine("├────────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ 👥 Total Users: {group.Users.Count.ToString().PadRight(44)} │");
            Console.WriteLine("├────────────────────────────────────────────────────────────────┤");

            if (group.Users.Count > 0)
            {
                Console.WriteLine("│ 📋 Users:                                                      │");
                
                int displayCount = Math.Min(group.Users.Count, 10); // Show max 10 users
                for (int i = 0; i < displayCount; i++)
                {
                    var user = group.Users[i];
                    var displayText = $"   {i + 1:D2}. {user.DomainId}";
                    Console.WriteLine($"│ {displayText.PadRight(62)} │");
                }

                if (group.Users.Count > 10)
                {
                    var moreText = $"   ... and {group.Users.Count - 10} more users";
                    Console.WriteLine($"│ {moreText.PadRight(62)} │");
                }
            }
            else
            {
                Console.WriteLine("│ ⚠️  No users found in this group                               │");
            }

            Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
            
            Console.WriteLine(string.Format(AppConstant.Messages.GroupProcessed, group.GroupName, group.Users.Count));
            Console.WriteLine();
        }

        public static void DisplayError(string title, string message)
        {
            Console.WriteLine();
            Console.WriteLine("┌────────────────────────────────────────────────────────────────┐");
            Console.WriteLine($"│ ❌ ERROR: {title.PadRight(52)} │");
            Console.WriteLine("├────────────────────────────────────────────────────────────────┤");
            Console.WriteLine($"│ {message.PadRight(62)} │");
            Console.WriteLine("└────────────────────────────────────────────────────────────────┘");
            Console.WriteLine();
        }

        public static void DisplayInfo(string message)
        {
            Console.WriteLine($"ℹ️  {message}");
        }

        public static void DisplaySuccess(string message)
        {
            Console.WriteLine($"✅ {message}");
        }
    }
}