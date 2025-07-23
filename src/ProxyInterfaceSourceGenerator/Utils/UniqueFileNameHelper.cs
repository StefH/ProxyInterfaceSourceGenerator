namespace ProxyInterfaceSourceGenerator.Utils;

internal class UniqueFileNameHelper
{
    private const string Suffix = ".g.cs";
    private readonly int _length = Suffix.Length;

    // Tracks usage count per base name (case-insensitive)
    private readonly Dictionary<string, int> _fileNameCounters = new(StringComparer.OrdinalIgnoreCase);

    internal string GetUniqueFileName(string fileName)
    {
        var baseName = fileName.Substring(0, fileName.Length - _length);

        if (!_fileNameCounters.TryGetValue(baseName, out var count))
        {
            // First time: return original name
            _fileNameCounters[baseName] = 0;
            return fileName;
        }

        // Increment count and return suffixed name
        count++;
        _fileNameCounters[baseName] = count;

        var newFileName = $"{baseName}_{count}{Suffix}";
        return newFileName;
    }
}