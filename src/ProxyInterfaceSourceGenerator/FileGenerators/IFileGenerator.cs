using ProxyInterfaceSourceGenerator.Models;

namespace ProxyInterfaceSourceGenerator.FileGenerators;

internal interface IFileGenerator
{
    FileData GenerateFile();
}