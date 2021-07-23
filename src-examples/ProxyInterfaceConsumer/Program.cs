using System;

namespace SourceGeneratorInterface
{
    public class Program
    {
        public static void Main()
        {
            PersonProxy p = new PersonProxy(new Person());
            p.Name = "test";
            p.Add("x");
            p.Void();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(p));
        }
    }
}
