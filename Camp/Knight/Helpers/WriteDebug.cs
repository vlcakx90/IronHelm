using Knight.Models.Raven;
using System;


namespace Knight.Helpers
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


        public static void PrintRaven(Raven raven)
        {
            Console.WriteLine();
            Console.WriteLine("### Raven");
            Console.WriteLine($"SoldierId: {raven.SoldierId}");
            Console.WriteLine($"Type:      {raven.Type}");
            Console.WriteLine($"Message:   {raven.Message}");
            Console.WriteLine();
        }
    }
}
