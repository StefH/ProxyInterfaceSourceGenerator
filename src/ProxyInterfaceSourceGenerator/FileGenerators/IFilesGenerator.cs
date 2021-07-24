using System.Collections.Generic;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal interface IFilesGenerator
    {
        IEnumerable<FileData> GenerateFiles();
    }
}