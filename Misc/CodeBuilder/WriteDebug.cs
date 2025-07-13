
namespace CodeBuilder
{
    public static class WriteDebug
    {
        private const string nornmal = "[*] ";
        private const string good = "[+] ";
        private const string error = "[!] ERROR: ";
        private const string dummy = "[^] ";
        public static void Info(string msg)
        {
            Console.WriteLine(nornmal + msg);
        }
        public static void Good(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(good + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Error(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error + msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Dummy(string msg)
        {
            Console.WriteLine(dummy + msg);
        }
    }
}
