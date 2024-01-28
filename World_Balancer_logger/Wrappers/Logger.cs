namespace VRC_Mod_Tool.Wrappers
{
    internal class Logger
    {
        public static void Log(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }
        public static void CrasherLog(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }
        public static void BannedLog(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }
        public static void botLog(object obj)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }
        public static void ListLog(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogDebug(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogImportant(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogWorld(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogSuccess(object obj)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogError(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogWarning(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [World_Balancer_logger] {obj}");
        }

        public static void LogImportantVRChat(object obj)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] [Doughnut Tool] {obj}");
        }
    }
}
