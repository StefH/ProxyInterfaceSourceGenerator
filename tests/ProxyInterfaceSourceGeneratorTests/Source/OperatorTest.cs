namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class OperatorTest
    {
        public string Name { get; set; }

        // Operator : implicit
        public static implicit operator OperatorTest(string name) => new()
        {
            Name = name
        };

        // Operator : explicit
        public static explicit operator string(OperatorTest test) => test.Name;
    }
}