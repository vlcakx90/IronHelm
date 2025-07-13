using System.Runtime.Serialization.Json;


namespace Castle.Helpers
{
    public static class Extensions
    {
        //////////////////////////// Mine
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


        //////////////////////////// SharpC2
        //public static byte[] Serialize<T>(this T obj) // Serializer here comes from ProtoBuf
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        Serializer.Serialize(ms, obj);
        //        return ms.ToArray();
        //    }
        //}

        //public static T Deserialize<T>(this byte[] data)
        //{
        //    using (var ms = new MemoryStream(data))
        //    {
        //        return Serializer.Deserialize<T>(ms);
        //    }
        //}

    }
}
