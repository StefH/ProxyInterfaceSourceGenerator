using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal interface IFilesGenerator
{
    IEnumerable<FileData> GenerateFiles();
}