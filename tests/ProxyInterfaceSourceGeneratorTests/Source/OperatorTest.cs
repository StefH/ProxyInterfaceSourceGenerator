namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class OperatorTest
    {
        public string Name { get; set; } = null!;

        // Operator : implicit
        public static implicit operator OperatorTest(string name)
        {
            return new()
            {
                Name = name
            };
        }

        // Operator : explicit
        public static explicit operator string(OperatorTest test)
        {
            return test.Name;
        }
    }

    public class X
    {
        public X()
        {
            OperatorTest operatorTest = "stef";
            var s = (string)operatorTest;
        }
    }
}