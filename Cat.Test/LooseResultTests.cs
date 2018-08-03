using System;
using Cat.Core.Result.Loose;
using Xunit;

namespace Cat.Test
{
    public class LooseResultTests
    {
        [Fact]
        public void LooseResultTests_Reference_Success()
        {
            var result = new TestClass("test");
            var r = new Success<TestClass>(result);
            Assert.True(r.IsSuccess);
            Assert.True(r.Data == result);
        }

        [Fact]
        public void LooseResultTests_Reference_Failure()
        {
            var errorMessage = "error";
            var r = errorMessage.ToFailure();
            Assert.False(r.IsSuccess);
            Assert.True(r.Error == errorMessage);
        }

        [Fact]
        public void LooseResultTests_Reference_Then_Success()
        {
            var result = new TestClass("test");
            var r = result.ToSuccess().Then<TestClass, AnotherTestClass>(s => new AnotherTestClass(1));
            Assert.True(r.IsSuccess);
            Assert.True(r.Unwrap<AnotherTestClass>().Property == 1);
        }

        [Fact]
        public void LooseResultTests_Reference_Then_Failure()
        {
            var r = "test".ToFailure().Then<TestClass,AnotherTestClass>(s => new AnotherTestClass(1));
            Assert.False(r.IsSuccess);
            Assert.True(((Failure<string>) r).Error == "test");
        }

        [Fact]
        public void LooseResultTests_Reference_Then_ArgumentExceptionIfIResult()
        {
            Action a = () => "test".ToFailure()
                .Then<string,IResult>(s => new AnotherTestClass(1).ToSuccess());
            Assert.ThrowsAny<ArgumentException>(() => a());
        }

         [Fact]
        public void LooseResultTests_Reference_ThenResult_Success()
        {
            var r = new TestClass("test").ToSuccess().ThenLift<TestClass>(s => new AnotherTestClass(1).ToSuccess());
            Assert.True(r.IsSuccess);
            Assert.True(r.Unwrap<AnotherTestClass>().Property == 1);
        }

        [Fact]
        public void LooseResultTests_Reference_ThenResult_Failure()
        {
            var r = new TestClass("test").ToSuccess().ThenLift<TestClass>(s => "error".ToFailure());
            Assert.False(r.IsSuccess);
            Assert.True(((Failure<string>) r).Error == "error");
        }

    }
}