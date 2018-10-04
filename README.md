<img src="https://raw.githubusercontent.com/pamidur/q-rest/master/logo.png" width="48" align="right"/>Q-Rest Reference
========================

## Project Status
[![Build Status](https://travis-ci.org/pamidur/q-rest.svg?branch=master)](https://travis-ci.org/pamidur/q-rest)

Package | Release | Pre-release
--- | --- | ---
**Core** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Core.svg)](https://www.nuget.org/packages/QRest.Core)
**Semantics.MethodChain** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Semantics.MethodChain.svg)](https://www.nuget.org/packages/QRest.Semantics.MethodChain)
**AspNetCore** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.AspNetCore.svg)](https://www.nuget.org/packages/QRest.AspNetCore)


## HowTo

#### Install
```
dotnet add package QRest.AspNetCore
```

#### Use
```csharp
[HttpGet("{query?}")]
public ActionResult Get(Query query)
{
      var data = collection.AsQueryable();
      var result = query.Apply(data);   
      return Ok(result);
} 
```

#### Execute ##
```
GET http://localhost:5000/api/data/:where(Text-eq(`123`)):take(10)
```

See more on supported method in [wiki](https://github.com/pamidur/q-rest/wiki/Method-Chain-Semantics)

## Outdated

Both cases don't require escaping symbols


:filter(text:eq('qwerty'),text:contains(@a),:or(text:eq('ololo')))

text = quert AND text = a OR text = ololo


:filter(:or(text:eq('qwerty'),text:eq('ololo'):not):not,text:contains(@a))

(text = quert OR text = ololo) AND text = a


Bigger one 
```
:filter(text:eq(qwerty),text:contains(@a)):select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(*,internal:select)):sort(blocks:max(id)):skip(3):take(10),a:let(1)
```
Let me split it for you for better readability
```
:filter(text:eq('qwerty'),text:contains(@a))
:select(id,text,data:select(id,value),blocks:filter(id:eq(1)):select(*,internal:select))
:sort(blocks:max(id))
:skip(3)
:take(10)
,a:let(1)
```

Design notes:
- System is extensible by adding new operations
- The operation is a "static method" with context as a first argument (like c# extension methods)

Ideas:
- oData sucks
- we need to make the whole url to be without escape symbols (less manual mistakes, QA will be happy)
- put hole query into single http query param (so we separate webapi param parsing from actual query parsing)
- the query is a chain of method calls, metheds are chained by colons
- one can call method by using ```:method``` or with params ```:method(param)``` or with multiple params ```:method(param1, param2)```
- ```:select``` == ```:select(*)``` == odata's ```$expand```
- all complex properties are expandable (collapsed by default) despite the database behind
- method call are translated into linq Expressions. System is extensible with custom expressions.
- chain may ends aggregation method such as ```:count``` and ```:max```

- system may have more then one semantics and parser, but after url processing we should and up with object representation of method chain
