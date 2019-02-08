<img src="https://raw.githubusercontent.com/pamidur/q-rest/master/logo.png" width="48" align="right"/>Q-Rest Reference
========================

## Project Status
[![Build Status](https://travis-ci.org/pamidur/q-rest.svg?branch=master)](https://travis-ci.org/pamidur/q-rest)

Package | Release | Pre-release
--- | --- | ---
**QRest.Core** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Core.svg)](https://www.nuget.org/packages/QRest.Core)
**QRest.AspNetCore** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.AspNetCore.svg)](https://www.nuget.org/packages/QRest.AspNetCore)
**QRest.Semantics.OData** | `n/a` | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Semantics.OData.svg)](https://www.nuget.org/packages/QRest.Semantics.OData)


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
GET http://localhost:5000/api/data/-where(:Text-eq(`123`))-take(10)
```

See more on supported method in [wiki](https://github.com/pamidur/q-rest/wiki/Method-Chain-Semantics)
