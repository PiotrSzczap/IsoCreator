using System;
using System.IO;
using System.Threading.Tasks;
using DiscUtils.Streams;

namespace IsoCreator.Domain
{
    public static class CDBuilderExtensions
    {
        public static async Task BuildAsync(this StreamBuilder streamBuilder, string outputFile, Action<long, long> notifyProgress = null)
        {
            using (FileStream output = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                using (Stream stream = streamBuilder.Build())
                {
                    var streamLength = stream.Length;
                    int currentLength = 0;
                    byte[] buffer = new byte[65536];
                    for (int count = await stream.ReadAsync(buffer, 0, buffer.Length);
                        count != 0;
                        count = await stream.ReadAsync(buffer, 0, buffer.Length))
                    {
                        notifyProgress?.Invoke(currentLength, streamLength);
                        currentLength += buffer.Length;
                        await output.WriteAsync(buffer, 0, count);
                    }
                }
            }
        }
    }
}
