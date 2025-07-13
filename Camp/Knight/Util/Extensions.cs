using System.IO;
using System.Runtime.Serialization.Json;

namespace Knight
{
    public static class Extensions
    {
        public static byte[] Serialise<T>(this T data)
        {
            var serialiser = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serialiser.WriteObject(ms, data);
                return ms.ToArray();
            }
        }

        public static T Deserialise<T>(this byte[] data)  // THE PROBLEM 
        {
            var serialiser = new DataContractJsonSerializer(typeof(T));

            using (var ms = new MemoryStream(data))
            {
                return (T)serialiser.ReadObject(ms); // Errors if File attribute is not null
            }
        }
    }
}