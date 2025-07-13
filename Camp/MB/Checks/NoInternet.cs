using System.Net;
using System.Text;

namespace MB1.Checks
{
    public class NoInternet : Check
    {
        public override string Name => nameof(NoInternet);

        public override bool Execute()
        {
            bool result = false;

            try
            {
                // https://ifconfig.me/
                string SwgYE = "htt"; string yQjQg = "ps:"; string CowPw = "//i"; string YPKbK = "fco"; string BylYt = "nfi"; string mgdKt = "g.m"; string QOYkd = "e/";
                string ipMe = SwgYE + yQjQg + CowPw + YPKbK + BylYt + mgdKt + QOYkd;

                WebClient client = new WebClient();
                var ipAddr = client.DownloadData(ipMe);

                if (IPAddress.TryParse(Encoding.UTF8.GetString(ipAddr), out IPAddress ip))
                {
                    //WriteDebug.Info($"IpAddr: {ip}");
                    result = false;
                }
            }
            catch
            {
                result = true;
            }

            return result;
        }
    }
}
