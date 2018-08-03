# CAT  
C# abstract types - dotnetstandard 1.0  
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/jkone27/cat/issues)


![](https://raw.githubusercontent.com/jkone27/cat/master/Pics/Cat01.PNG)

## Option
simulates maybe type with inheritance

## Result
tries to implement Choice/Either type, strict has more static "strictness",  
wheras Loose is more dynamic (using interface inheritance polymorphism)

### Status
[![build status](https://img.shields.io/travis/jkone27/cat.svg)](https://travis-ci.org/jkone27/cat)

#### CatCat
https://www.nuget.org/packages/CatCat/

#### Sample

```cs

//program.cs -- tested with .NET 4.5 and donetcore 2.0

using System;
using Cat.Core.Option;
using Cat.Core.Result.Loose;
using Cat.Core.Result.Strict;

namespace ConsoleApp1
{

    //a service which might fail trowing some exceptions
    public class ServiceWhichMayFail
    {
        public string CallWhichMayFail(bool fail)
        {
            if (fail)
                throw new Exception("fail");

            else return "ok";
        }
    }
    
    
    //main program
    class Program
    {
        static void Main(string[] args)
        {
            Options();
            LooseResults();
            StrictResult();
            Console.ReadLine();
        }

        //sample tests for options (null, Nullable<T>, null string and null references are handled towards None)
        private static void Options()
        {
            Console.WriteLine(nameof(Options)+":\r\n");
            var r = "not null".ToOption()
                .Then(z => "hello")
                .Then(x => "1")
                .Then(x => Int32.Parse(x) as int?)
                .Then(x => DateTime.Now)
                .Then(x => null as string);

            if (!r.IsSome)
            {
                Console.WriteLine("r is None");
            }
            else
            {
                Console.WriteLine($"r is Some!: {r.UnwrapSome()}");
            }
            
        }
        
       

        //wrapped with Cat.Core.Loose.IResult (more dynamic type checking, watch out!)
        public class WrappedService
        {
            private readonly ServiceWhichMayFail service;

            public WrappedService(ServiceWhichMayFail service)
            {
                this.service = service;
            }
            public IResult CallWhichMightFail(bool fail)
            {
                try
                {
                    return service.CallWhichMayFail(fail).ToSuccess();
                }
                catch (Exception ex)
                {
                    return ex.ToFailure();
                }
            }
        }

        //Cat.Core.Loose.IResult
        private static void LooseResults()
        {
            Console.WriteLine(nameof(LooseResults)+":\r\n");
            var x = new WrappedService(new ServiceWhichMayFail());

            var result = x.CallWhichMightFail(false)
                .Then<string, int>(s => 3)
                .ThenLift<int>(z => x.CallWhichMightFail(false))
                .ThenLift<string>(z => x.CallWhichMightFail(true))
                .ThenLift<string>(z => x.CallWhichMightFail(false));

            if (result.IsSuccess)
            {
                var r = ((Success<string>) result).Data;
                Console.WriteLine($"success: {r}");
            }
            else
            {
                var ex = ((Failure<Exception>) result).Error;
                Console.WriteLine($"failure: {ex}");
            }
        }

        

        //wrapped with Cat.Core.Strict.IResult (more static type checking, les need of explicit type declaration)
        public class WrappedServiceStrict
        {
            private readonly ServiceWhichMayFail service;

            public WrappedServiceStrict(ServiceWhichMayFail service)
            {
                this.service = service;
            }
            public IResult<string,Exception> CallWhichMightFail(bool fail)
            {
                try
                {
                    return service.CallWhichMayFail(fail).ToSuccess<string,Exception>();
                }
                catch (Exception ex)
                {
                    return ex.ToFailure<string,Exception>();
                }
            }
        }

        //Cat.Core.Strict.IResult tests
        private static void StrictResult()
        {
            Console.WriteLine(nameof(StrictResult)+":\r\n");
            var x = new WrappedServiceStrict(new ServiceWhichMayFail());

            var result = x.CallWhichMightFail(false)
                .Then(s => 3)
                .ThenLift(z => x.CallWhichMightFail(false))
                .ThenLift(z => x.CallWhichMightFail(false))
                .ThenLift(z => x.CallWhichMightFail(false))
                .Then(_ => 43);

            if (result.IsSuccess)
            {
                var r = result.AsSuccess;
                Console.WriteLine($"success: {r}");
            }
            else
            {
                var ex = result.Error;
                Console.WriteLine($"failure: {ex}");
            }
        }

       
    }
}




```
