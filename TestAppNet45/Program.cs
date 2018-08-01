using System;
using Cat.Core.Option;
using Cat.Core.Result.Loose;
using Cat.Core.Result.Strict;

namespace TestAppNet45
{
    class Program
    {
        static void Main(string[] args)
        {
            var x = new SampleClass("test");
            var s1 = x.ToSuccess().Then<SampleClass,int>(n => 0);
            Console.WriteLine(s1.Unwrap<int>());
            var o = x.ToOption().Then(_ => 5 as int?);
            Console.WriteLine(o.UnwrapSome());
            var s2 = x.ToSuccess<SampleClass, string>().Then(z => "hello world");
            Console.WriteLine(s2.AsSuccess);
            Console.ReadLine();

        }
    }
}
