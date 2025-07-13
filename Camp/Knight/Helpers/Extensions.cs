using SharedArsenal.Native;
using System.IO;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO.Pipes;


namespace Knight.Helpers
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

        #region TCP
        public static bool DataAvailable(this TcpClient client)
        {
            var stream = client.GetStream();
            return stream.DataAvailable;
        }

        public static async Task<byte[]> ReadStream(this Stream stream)
        {
            // read length
            var lengthBuf = new byte[4];
            var read = await stream.ReadAsync(lengthBuf, 0, 4);

            if (read != 4)
                throw new Exception("Failed to read length");

            var length = BitConverter.ToInt32(lengthBuf, 0);

            // read rest of data
            using (var ms = new MemoryStream())
            {
                var totalRead = 0;

                do
                {
                    var buf = new byte[1024];
                    read = await stream.ReadAsync(buf, 0, buf.Length);

                    await ms.WriteAsync(buf, 0, read);
                    totalRead += read;
                }
                while (totalRead < length);

                return ms.ToArray();
            }
        }

        public static async Task WriteStream(this Stream stream, byte[] data)
        {
            // format data as [length][value]
            var lengthBuf = BitConverter.GetBytes(data.Length);
            var lv = new byte[lengthBuf.Length + data.Length];

            Buffer.BlockCopy(lengthBuf, 0, lv, 0, lengthBuf.Length);
            Buffer.BlockCopy(data, 0, lv, lengthBuf.Length, data.Length);

            using (var ms = new MemoryStream(lv))
            {
                // write in chunks
                var bytesRemaining = lv.Length;
                do
                {
                    var lengthToSend = bytesRemaining < 1024 ? bytesRemaining : 1024;
                    var buf = new byte[lengthToSend];

                    var read = await ms.ReadAsync(buf, 0, lengthToSend);

                    if (read != lengthToSend)
                        throw new Exception("Could not read data from stream");

                    await stream.WriteAsync(buf, 0, buf.Length);

                    bytesRemaining -= lengthToSend;
                }
                while (bytesRemaining > 0);
            }
        }
        #endregion

        #region SMB
        public static bool DataAvailable(this PipeStream pipe)
        {
            var hPipe = pipe.SafePipeHandle.DangerousGetHandle();
            var status = Kernel32.PeekNamedPipe(hPipe, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            return status;
        }

        //public static bool DataAvailable(this PipeStream pipe)    // SharpC2 using DInvoke
        //{
        //    var hPipe = pipe.SafePipeHandle.DangerousGetHandle();
        //    return Interop.Methods.PeekNamedPipe(hPipe);
        //}

        #endregion
    }
}
