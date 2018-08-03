# CAT  
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/jkone27/cat/issues)  
C# abstract types - dotnetstandard 1.0  
  
*- Inspired by FP, F# lang and [ROP](https://fsharpforfunandprofit.com/rop/) -*  


![](https://raw.githubusercontent.com/jkone27/cat/master/Pics/Cat01.PNG)

## Option
Simulates a maybe type with generics and interface inheritance. IOption(T) exposes bool property .IsSome .
Some(T) and None(T) both implement IOption(T). I kept the generic constraint on None(T) because 
I think it can anyway help the compiler figure out and not mix up different "nones".

## Result
Simulates Choice/Either type via generics and interface inheritance. 

### Result.Strict
Strict version has more static "strictness", meaning you need to provide more type constraints, but then type inference will work its magic out when possible. IResult(TOk,TError) exposes bool .IsSuccess, TOk .AsSuccess and TError .Error.  
You will get a runtime exception if you try to access .Error on a Success(TOk,TError) and .AsSuccess on a Failure(TOk,TError) instance . 

### Result.Loose
Loose is more dynamic, skipping some typechecks, it's a light-weight version,
but be aware of your types, it is more likely to throw at runtime for invalid casts.  
IResult exposes .IsSuccess and is implemented by Success(T) and Failure(T). you will need to cast or make use of the "as" operator when converting back to the concrete type from IResult, thus providing the generic type constraint.

## Then
Both option and result types were given a .Then extension method, which in a way (for the little i know) emulates the monadic
bind operator, making available the "unwrapped" result to the next computation, or just returning to the end of the "pipeline" if
the option is None, or if the result is Failure.

## ThenLift
Loose results supports .ThenLift method for cases where the next step in the pipeline might want to return an IResult too, it's not wrapped internally, that's why I named it Lift (is going from a type T to a lifted-wrapped type W(T)). If you try to pass an IResult returning function to a .Then method, as a continuation, it will throw you an ArgumentException "use ThenLift instead!".

### Status
[![build status](https://img.shields.io/travis/jkone27/cat.svg)](https://travis-ci.org/jkone27/cat)

#### CatCat
https://www.nuget.org/packages/CatCat/



#### Sample (raw)

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
