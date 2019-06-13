using System;
using Xunit;
using Cat.Core.Option;

namespace Cat.Test
{
    public class OptionTests
    {
        [Fact]
        public void OptionTests_Reference_IsSome()
        {
            var reference = new TestClass("property");
            var op = reference.AsDynamicOption();
            Assert.True(op.IsSome);
        }

        [Fact]
        public void OptionTests_Value_IsSome()
        {
            int? nullableValue = 34;
            var op = nullableValue.ToOption();
            Assert.True(op.IsSome);
        }

        [Fact]
        public void OptionTests_Reference_IsNone()
        {
            TestClass reference = null;
            var op = reference.AsDynamicOption();
            Assert.False(op.IsSome);
        }

        [Fact]
        public void OptionTests_Value_IsNone()
        {
            int? nullableValue = null;
            var op = nullableValue.ToOption();
            Assert.False(op.IsSome);
        }

        [Fact]
        public void OptionTests_Reference_Then_IsSome()
        {
            var reference = new TestClass("1");
            var op = reference.AsDynamicOption().Select(r => new AnotherTestClass(Int32.Parse(r.Property)));
            Assert.True(op.IsSome);
            Assert.IsType<AnotherTestClass>(op.Unwrap());
            Assert.True(op.Unwrap().Property == 1);
        }

        [Fact]
        public void OptionTests_Value_Then_IsSome()
        {
            int? nullableValue = 1;
            var op = nullableValue.ToOption().Select(_ => DateTime.Now as DateTime?).Select(_ => 45 as int?);
            Assert.True(op.IsSome);
            Assert.True(op.Unwrap() == 45);
        }

        [Fact]
        public void OptionTests_Reference_Then_IsNone()
        {
            TestClass reference = new TestClass("1");
            var op = reference.AsDynamicOption().Select(r => null as AnotherTestClass);
            Assert.False(op.IsSome);
        }

        [Fact]
        public void OptionTests_Value_Then_IsNone()
        {
            int? nullableValue = 1;
            var op = nullableValue.ToOption().Select(_ => DateTime.Now as DateTime?).Select(_ => null as int?);
            Assert.False(op.IsSome);
        }
    }
}
