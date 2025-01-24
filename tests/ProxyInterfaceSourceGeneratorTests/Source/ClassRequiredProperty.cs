namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class ClassRequiredProperty
    {
        public required string Test { get; set; }

        public required string Test2 { get; init; }

        public required ClassRequiredProperty? X { get; init; }
    }
}