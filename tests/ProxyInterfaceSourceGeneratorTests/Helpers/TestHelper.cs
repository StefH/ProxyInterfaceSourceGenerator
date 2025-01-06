namespace ProxyInterfaceSourceGeneratorTests.Helpers;

internal static class TestHelper
{
    internal static Lazy<string> ProjectRoot = new(() =>
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (currentDirectory != null)
        {
            if (currentDirectory.GetFiles("*.csproj").Length > 0)
            {
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;
        }

        throw new Exception("Project root not found");
    });
}