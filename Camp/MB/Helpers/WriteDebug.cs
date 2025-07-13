using System;

namespace MB1.Helpers
{
    public static class WriteDebug
    {
        private const string nornmal = "[*] ";
        private const string good = "[+] ";
        private const string error = "[!] ERROR: ";
        private const string dummy = "[^] ";

        public static void Good(string msg)
        {
            Console.WriteLine(good + msg);
        }
        public static void Info(string msg)
        {
            Console.WriteLine(nornmal + msg);
        }
        public static void Error(string msg)
        {
            Console.WriteLine(error + msg);
        }

        public static void Dummy(string msg)
        {
            Console.WriteLine(dummy + msg);
        }

    }
}
