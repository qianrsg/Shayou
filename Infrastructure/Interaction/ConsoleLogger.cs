using System;

namespace Shayou.Infrastructure.Interaction
{
    public enum LogChannel
    {
        Backend,
        Frontend
    }

    public static class ConsoleLogger
    {
        public static void Log(LogChannel channel, string message)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = channel switch
            {
                LogChannel.Backend => ConsoleColor.Gray,
                LogChannel.Frontend => ConsoleColor.Blue,
                _ => originalColor
            };

            Console.WriteLine($"[{channel}] {message}");
            Console.ForegroundColor = originalColor;
        }
    }
}
