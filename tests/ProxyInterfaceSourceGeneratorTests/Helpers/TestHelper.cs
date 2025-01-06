namespace ProxyInterfaceSourceGeneratorTests.Helpers;

internal static class TestHelper
{
    internal static Lazy<string> TestProjectRoot = new(() =>
    {
        var currentDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (currentDirectory != null)
        {
            if (currentDirectory.GetFiles("ProxyInterfaceSourceGeneratorTests.csproj").Length > 0)
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new Exception("Project root not found");
    });
}