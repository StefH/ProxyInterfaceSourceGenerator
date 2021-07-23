using System.Collections.Generic;

namespace ProxyInterfaceSourceGenerator.FileGenerators
{
    internal interface IFilesGenerator
    {
        IEnumerable<Data> GenerateFiles();
    }
}