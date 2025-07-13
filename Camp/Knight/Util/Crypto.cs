// From my project: SharpConnect

using Knight.Helpers;
using System;
using System.Text;

namespace Knight.Utils
{
    public static class Crypto
    {

        public static string Encode<T>(T obj)
        {
            try
            {
                var json = obj.Serialise();
                string temp = Convert.ToBase64String(json);

                return temp;
            }
            catch
            {
                throw new FormatException();
            }
        }
        public static T Decode<T>(string enc)
        {
            try
            {
                //string temp = Encoding.ASCII.GetString(data);
                var temp = Convert.FromBase64String(enc);

                return temp.Deserialise<T>();
            }
            catch
            {
                throw new FormatException();
            }
        }
    }
}