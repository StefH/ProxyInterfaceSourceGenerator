using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace ProxyInterfaceSourceGenerator.Tool;

internal static class PortableExecutableReferenceUtils
{
    private static readonly string[] TargetFrameworks = ["net8.0", "net7.0", "net6.0", "net5.0", "netstandard2.1", "netstandard2.0"];

    internal static HashSet<PortableExecutableReference> GetAllReferences(string dllPath, string? nugetPackagesFolder = null)
    {
        if (!File.Exists(dllPath))
        {
            throw new FileNotFoundException("Target DLL not found.", dllPath);
        }

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var references = new HashSet<PortableExecutableReference>();

        // Build full resolver set
        nugetPackagesFolder ??= GetDefaultNuGetPackagesPath();
        var frameworkAssemblies = GetFrameworkAssemblyPaths().ToList();
        var allCandidateDlls = new HashSet<string>(frameworkAssemblies, StringComparer.OrdinalIgnoreCase);

        // Start with local folder
        var baseDir = Path.GetDirectoryName(dllPath)!;
        foreach (var dll in Directory.GetFiles(baseDir, "*.dll"))
        {
            allCandidateDlls.Add(dll);
        }

        // Scan NuGet folder for dependencies
        var nugetGraph = ScanNuGetPackages(nugetPackagesFolder);
        foreach (var dll in nugetGraph)
        {
            allCandidateDlls.Add(dll);
        }

        var resolver = new PathAssemblyResolver(allCandidateDlls);
        using var metadataLoadContext = new MetadataLoadContext(resolver);

        var rootAssembly = metadataLoadContext.LoadFromAssemblyPath(dllPath);
        TraverseWithMetadataLoadContext(rootAssembly, metadataLoadContext, visited, references);

        return references;
    }

    private static void TraverseWithMetadataLoadContext(
        Assembly assembly,
        MetadataLoadContext mlc,
        HashSet<string> visited,
        HashSet<PortableExecutableReference> references)
    {
        if (string.IsNullOrEmpty(assembly.Location) || !visited.Add(assembly.Location))
        {
            return;
        }

        try
        {
            references.Add(MetadataReference.CreateFromFile(assembly.Location));
        }
        catch
        {
            // Skip invalid ones
        }

        foreach (var refName in assembly.GetReferencedAssemblies())
        {
            try
            {
                var refAssembly = mlc.LoadFromAssemblyName(refName);
                TraverseWithMetadataLoadContext(refAssembly, mlc, visited, references);
            }
            catch
            {
                // Skip if unresolved
            }
        }
    }

    private static IEnumerable<string> GetFrameworkAssemblyPaths()
    {
        var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
        var common = new[]
        {
            "System.Private.CoreLib.dll",
            "System.Runtime.dll",
            "netstandard.dll",
            "mscorlib.dll"
        };

        foreach (var name in common)
        {
            var path = Path.Combine(runtimeDir, name);
            if (File.Exists(path))
            {
                yield return path;
            }
        }
    }

    private static string GetDefaultNuGetPackagesPath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".nuget", "packages");
    }

    private static IEnumerable<string> ScanNuGetPackages(string nugetRoot)
    {
        var dlls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(nugetRoot))
        {
            return dlls;
        }



        foreach (var idDir in Directory.EnumerateDirectories(nugetRoot))
        {
            foreach (var versionDir in Directory.EnumerateDirectories(idDir))
            {
                var libDir = Path.Combine(versionDir, "lib");
                if (!Directory.Exists(libDir))
                {
                    continue;
                }

                foreach (var tfm in TargetFrameworks)
                {
                    var tfmDir = Path.Combine(libDir, tfm);
                    if (Directory.Exists(tfmDir))
                    {
                        foreach (var dll in Directory.EnumerateFiles(tfmDir, "*.dll"))
                        {
                            dlls.Add(dll);
                        }

                        break;
                    }
                }
            }
        }

        return dlls.OrderBy(d => d);
    }
}