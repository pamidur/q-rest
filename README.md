<img src="https://raw.githubusercontent.com/pamidur/q-rest/master/logo.png" width="48" align="right"/>Q-Rest Reference
========================

## Project Status
[![Build Status](https://travis-ci.org/pamidur/q-rest.svg?branch=master)](https://travis-ci.org/pamidur/q-rest)

Package | Stable | Latest
--- | --- | ---
**QRest.Core** | [![Nuget](https://img.shields.io/nuget/v/QRest.Core?label=stable)](https://www.nuget.org/packages/QRest.Core) | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Core?label=latest)](https://www.nuget.org/packages/QRest.Core)
**QRest.AspNetCore** | [![NuGet](https://img.shields.io/nuget/v/QRest.AspNetCore?label=stable)](https://www.nuget.org/packages/QRest.AspNetCore) | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.AspNetCore?label=latest)](https://www.nuget.org/packages/QRest.AspNetCore)
**QRest.Semantics.OData** | [![NuGet](https://img.shields.io/nuget/vpre/QRest.Semantics.OData?label=stable)](https://www.nuget.org/packages/QRest.Semantics.OData) | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/QRest.Semantics.OData?label=latest)](https://www.nuget.org/packages/QRest.Semantics.OData)


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
