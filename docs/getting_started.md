<!--
 Copyright 2022 Deveel
 
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 
     http://www.apache.org/licenses/LICENSE-2.0
 
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
-->

# Getting Started

The scope of this library is to provide the support for the persistence of _Users_ and _Roles_ to the [Microsoft Identity Core](https://github.com/dotnet/aspnetcore/tree/main/src/Identity) framework, and as such before you can enable it's functions you must configure it in that context.

## Intall the Library

The library is available through [NuGet](https://nuget.org), and can be installed and restored easily once configured in your projects.

At the moment (_February 2022_) this is developed within the `.NET 5.0` framework, and thus compatible with all the profiles of the .NET framework greater or equal than that (`.NET 6.0`, `.NET 7.0`).

You can install the library through the `dotnet` tool command line

```sh
dotnet add package Deveel.Identity.MongoDb
```

Or by editing your `.csproj` file and adding a `<PackageReference>` entry.

``` xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    ...

  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Deveel.Identity.MongoDb" Version="1.0.1-alpha1" />
    ...
  </ItemGroup>
</Project>
```

This provides to instrumentation to support the MongoDB storage in the scope of your application that uses the Microsoft Identity Framework.

## Configuring the Framework

The configuration of the stroage system is fairly easy and it follows a standard pattern, attached to the builder functions already provided by the framework.

For example, assuming you are working on a traditional _[ASP.NET application model](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-5.0&tabs=windows)_, you can enable these functions like this:

``` csharp

using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Deveel.Security;

namespace Example {
    public class Startup {
        public Startup(IConfiguration config) {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IServiceCollection services) {
            // ... add any other service you need ...

            // this call adds the basic MongoDB storage layer
            // using the default configurations
            services.AddIdentityCore<MongoUser>()
                .AddMongoStores(Configuration, "Mongo:Identity");
        }
    }
}

```

The above sample is all you need to start, and it uses one of the overloads provided that injects the needed configurations and services, accessing the sub/section `Mongo>Identity` from the given `IConfiguration` instance.

You will find out that the library provides many options to configure your instance of the framework: the example above is the simplest one that registers both the _Users_ and _Roles_ stores and services, while more fine-grained calls can let you select what to use.

### Using the Manager

Once the configurations are set, the environment makes available instances of `UserManager<MongoUser>` and `RoleManager<MongoRole>` that can be used for your operations

``` csharp

using System;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc

using Deveel.Security;

namespace Example {
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller {
        public UserController(UserManager<MongoUser> userManager) {
            // initialize a property and use its core functions
        }
    }
}

```
