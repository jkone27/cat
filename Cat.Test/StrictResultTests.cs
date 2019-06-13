using Cat.Core.Result.Strict;
using Xunit;

namespace Cat.Test
{
    public class StrictResultTests
    {
        [Fact]
        public void StrictResultTests_Reference_Success()
        {
            var result = new TestClass("test");
            var r = result.ToSuccess<TestClass, string>();
            Assert.True(r.IsSuccess);
            Assert.True(r.AsSuccess == result);
        }

        [Fact]
        public void StrictResultTests_Reference_Failure()
        {
            var errorMessage = "error";
            var r = errorMessage.ToFailure<TestClass, string>();
            Assert.False(r.IsSuccess);
            Assert.True(r.Error == errorMessage);
        }

        [Fact]
        public void StrictResultTests_Reference_Then_Success()
        {
            var result = new TestClass("test");
            var r = result.ToSuccess<TestClass, string>().Select(s => new AnotherTestClass(1));
            Assert.True(r.IsSuccess);
            Assert.True(r.AsSuccess.Property == 1);
        }

        [Fact]
        public void StrictResultTests_Reference_Then_Failure()
        {
            var r = "test".ToFailure<TestClass, string>().Select(s => new AnotherTestClass(1));
            Assert.False(r.IsSuccess);
            Assert.True(r.Error == "test");
        }

        [Fact]
        public void StrictResultTests_Reference_ThenLift_Success()
        {
            var r = new TestClass("test")
                .ToSuccess<TestClass, string>()
                .SelectMany(s => new AnotherTestClass(1)
                    .ToSuccess<AnotherTestClass,string>());
            Assert.True(r.IsSuccess);
            Assert.True(r.AsSuccess.Property == 1);
        }

        [Fact]
        public void StrictResultTests_Reference_ThenLift_Failure()
        {
            var r = new TestClass("test")
                .ToSuccess<TestClass, string>()
                .SelectMany(s => 
                    "error".ToFailure<TestClass,string>());
            Assert.False(r.IsSuccess);
            Assert.True(r.Error == "error");
        }

    }
}