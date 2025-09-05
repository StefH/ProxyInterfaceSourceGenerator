using System.IO.Compression;
using System.Text;

namespace ProxyInterfaceSourceGenerator.Tool;

public static class StringCompressor
{
    public static byte[] Compress(string text)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress))
        using (var writer = new StreamWriter(gzip, leaveOpen: true))
        {
            writer.Write(text);
        }

        return output.ToArray();
    }

    public static async Task<string> DecompressAsync(byte[] compressedBytes)
    {
        using var input = new MemoryStream(compressedBytes);
        await using var gzip = new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);

        return await reader.ReadToEndAsync();
    }
}