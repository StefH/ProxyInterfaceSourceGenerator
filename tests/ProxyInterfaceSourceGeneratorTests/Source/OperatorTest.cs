namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class OperatorTest
    {
        public string Name { get; set; } = null!;

        public int? Id { get; set; }

        // Operator : implicit
        public static implicit operator OperatorTest(string name)
        {
            return new()
            {
                Name = name
            };
        }
        
        public static implicit operator OperatorTest(int? id)
        {
            return new()
            {
                Id = id
            };
        }

        // Operator : explicit
        public static explicit operator string(OperatorTest test)
        {
            return test.Name;
        }

        public static explicit operator int?(OperatorTest test)
        {
            return test.Id;
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