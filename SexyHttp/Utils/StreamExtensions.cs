using System.IO;
using System.Threading.Tasks;

namespace SexyHttp.Utils
{
    public static class StreamExtensions
    {
        public static async Task<byte[]> ReadToEndAsync(this Stream stream, long bufferSize = 1024 * 10)
        {
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static byte[] ReadToEnd(this Stream stream, long bufferSize = 1024 * 10)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
