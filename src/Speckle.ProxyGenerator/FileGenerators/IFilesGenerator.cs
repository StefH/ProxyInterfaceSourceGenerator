using Speckle.ProxyGenerator.Models;

namespace Speckle.ProxyGenerator.FileGenerators;

internal interface IFilesGenerator
{
    IEnumerable<FileData> GenerateFiles();
}