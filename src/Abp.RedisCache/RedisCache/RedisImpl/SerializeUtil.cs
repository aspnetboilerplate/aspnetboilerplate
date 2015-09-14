using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RedisCache.RedisImpl
{
   public class SerializeUtil
    {

        
        public static byte[] Serialize(object data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream();
            formatter.Serialize(rems, data);
            return rems.GetBuffer();
        }

        
        public static object Deserialize(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream(data);
            data = null;
            return formatter.Deserialize(rems);
        }

        private static MemoryStream Compress(Stream stream)
        {
            var compressedStream = new MemoryStream();
            using (var compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                stream.CopyTo(compressionStream);
                return compressedStream;
            }
        }

        private static Stream Decompress(Stream stream)
        {
            var decompressedStream = new MemoryStream();
            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(decompressedStream);
                decompressedStream.Seek(0, SeekOrigin.Begin);
                return decompressedStream;
            }

        }
    }
}
