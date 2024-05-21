using Speckle.ProxyGenerator.Models;

namespace Speckle.ProxyGenerator.FileGenerators;

internal interface IFileGenerator
{
    FileData GenerateFile();
}