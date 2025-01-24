namespace ProxyInterfaceSourceGeneratorTests.Source
{
    public class ClassDirectAndIndirect
    {
        public string Id { get; set; } = string.Empty;

        public ClassDirectAndIndirect? Value { get; set; }

        public ClassDirectAndIndirect[] Array { get; set; } = [];

        public List<ClassDirectAndIndirect> List { get; set; } = new List<ClassDirectAndIndirect>();

        public int Test(ClassDirectAndIndirect[] array, List<ClassDirectAndIndirect> list, ClassDirectAndIndirect value, ClassDirectAndIndirect? valueNullable)
        {
            return 42;
        }
    }
}