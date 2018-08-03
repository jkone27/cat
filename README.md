# CAT  
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/jkone27/cat/issues)  
C# abstract types - dotnetstandard 1.0  
  
*- Inspired by FP, F# lang and [ROP](https://fsharpforfunandprofit.com/rop/) -*  


![](https://raw.githubusercontent.com/jkone27/cat/master/Pics/Cat01.PNG)

## Why Cat?

This small library with no dependencies at all (except dotnetstandard) was born from the interest of having maybe/option and
result/choice types in C#, without having to reference bigger library, and in a sort of "simplified" way. So it's just some interfaces, implementations and a few handy extension methods, you are welcome for contributions and filing/spotting issues!

## Option
Simulates a **Maybe/Option** type with generics and interface inheritance. `IOption<T>` exposes bool property `.IsSome` .
`Some<T>` and `None<T>` both implement `IOption<T>`. I kept the generic constraint on `None<T>` because 
I think it can anyway help the compiler figure out and not mix up different "nones".

## Result
Simulates **Choice/Either** type via generics and interface inheritance. 

### Result.Strict
Strict version has more static "strictness", meaning you need to provide more type constraints, but then type inference will work its magic out when possible. `IResult<TOk,TError>` exposes `bool .IsSuccess`, `TOk .AsSuccess` and `TError .Error`.  
You will get a runtime exception if you try to access `.Error` on a `Success<TOk,TError>` and `.AsSuccess` on a `Failure<TOk,TError>` instance . 

### Result.Loose
Loose is more dynamic, skipping some typechecks, it's a light-weight version,
but be aware of your types, it is more likely to throw at runtime for invalid casts.  
`IResult` exposes `bool .IsSuccess` and is implemented by `Success<T>` and `Failure<T>`. You will need to cast or make use of the "as" operator when converting back to the concrete type from `IResult`, thus providing the generic type constraint.

## Then
Both option and result types were given a `.Then` extension method, which in a way (for the little I know) emulates the monadic
**bind** operator, making available the "unwrapped" result to the next computation, or just returning to the end of the "pipeline" if
the option is `None`, or if the result is `Failure`.

## ThenLift
Results types supports `.ThenLift` method for cases where the next step in the pipeline might want to return an `IResult/IResult<TOk,TError>` too, it's not wrapped internally, that's why I named it Lift: is going from a type `T` to a lifted-wrapped type `W<T>`. If you try to pass an `IResult` returning function to a `.Then` method for the Loose type, as a continuation, it will throw you an `ArgumentException` "use ThenLift instead!".

### Status
[![build status](https://img.shields.io/travis/jkone27/cat.svg)](https://travis-ci.org/jkone27/cat)

#### CatCat
https://www.nuget.org/packages/CatCat/


## Usage

### Option

Here is some basic usage of the `IOption<T>` type. Please note that `null` references, as well as `Nullable<T>` where `T` is a value type and `null` strings, are all converted into `None<T>`.
  
```cs
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

```

let's now consider a service which might fail with an exception during invokation, 
it will come handy when considering the use of the Result types.

```cs
public class ServiceWhichMayFail
{
    public string CallWhichMayFail(bool fail)
    {
        if (fail)
            throw new Exception("fail");

        else return "ok";
    }
}
```

### Loose Result

Here is a wrapped version of that same service first:

```cs
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
```
and we can then proceed to use the `IResult` and `.Then/ThenLift` extensions as follow:
```cs
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
```

### Strict Result
and now let's see the different behaviour of strict result, once we wrap again our "could-be" failing service,
wrapped with a strict `IResult`, which has more static type checking, thus providing better type inference in method chains:  
```cs
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

```
and now the usage, you can see that is no more necessary to declare the type constraints in `.Then` calls (type inference works):  

```cs
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
```
Let me know any improvements/issues, feel free to pr.
