using Cat.Core.Option;
using Cat.Core.Option.Static;
using Cat.Core.Result.Dynamic;
using Cat.Core.Result.Loose;
using Cat.Core.Result.Static;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Option<int> staticOption = "hello".AsOption()
                .Select(o => (o + " world"))
                .SelectMany(o => ((int?)1).AsOption())
                .Select(o => o + 3);

            if (staticOption.IsSome)
            {
                Console.WriteLine(staticOption.Unwrap());
            }

            IOption<int> dynamicOption = "hello".AsDynamicOption()
                .Select(o => o + "world")
                .Select(o => 1)
                .Select(o => (int?)1);

            if (dynamicOption.IsSome)
            {
                Console.WriteLine(staticOption.Unwrap());
            }

            Result<int, string> staticResult = 1 > 2 ?
                "1 is greater than 2!".AsFailure<int, string>()
                : 2.AsSuccess<int, string>();

            var resultComputations = staticResult
                .Select(r => r > 3 ? 4 : 5, ex => ex.Message)
                .SelectMany(r => "hello".AsSuccess<string, string>())
                .Select(r => r != null ? r : "still-success", ex => ex.Message);

            if (resultComputations.IsSuccess)
            {
                Console.WriteLine(resultComputations.Value);
            }
            else
            {
                Console.WriteLine(resultComputations.Error);
            }

            //the more dynamic so type inference sucks
             IResult<int, string> dynamicResult = 1 > 2 ?
                "1 is greater than 2!".ToFailure<int, string>()
                : 2.ToSuccess<int, string>() as IResult<int, string>; //not needed in static one!

            var dynamicResultComputations = dynamicResult
                .Select(r => r > 3 ? 4 : 5, ex => ex.Message)
                .SelectMany(r => "hello".AsSuccess<string, string>())
                .Select(r => r != null ? r : "still-success", ex => ex.Message);
        }
    }
}
