# Deveel Identity - MongoDB

[![Build](https://github.com/deveel/deveel.identity.mongodb/actions/workflows/cd.yml/badge.svg)](https://github.com/deveel/deveel.identity.mongodb/actions/workflows/cd.yml) [![Code Coverage](https://codecov.io/gh/deveel/deveel.identity.mongodb/branch/master/graph/badge.svg?token=LZQLJ7NVHS)](https://codecov.io/gh/deveel/deveel.identity.mongodb) [![Maintainability](https://api.codeclimate.com/v1/badges/8228ab5c8ef87d708982/maintainability)](https://codeclimate.com/github/deveel/deveel.identity.mongodb/maintainability)

This is an implementation of the storage layer of the [Microsoft Identity](https://github.com/dotnet/aspnetcore/tree/main/src/Identity) framework that supports [MongoDB](https://mongodb.com) as persistent database system.

The model provided by this library supports all the standards requested by the framework for handling of the _User Type_ and the _Role Type_, plus an extension to _multi-tenancy_ scenarios. 

## Motivation

At the time this project was started (_February 2022_), several libraries already existed to support the [MongoDB](https://mongodb.com) database storage layer for the previous version of the framework (_ASP.NET Core Identity Core_), but at certain point Microsoft rebooted the project and forked it (as _Microsoft Extensions Identity_), and despite the functional gap between the two frameworks is small, yet it is important (_Data Protection_, _Authentication Keys_, etc.).

Furthermore, given that the other available libraries are based on a discontinued line, we wanted to provide a version of the storage system anchored to the current efforts and updated revisions of the framework.

## Usage

Starting to use the functions provided by the library is fairly easy and it ony requires invoking one of the overloads of the `IdentityBuilder` utility during the service registration process.

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
            // using the configurations from the sub-section Mongo:Identity
            services.AddIdentityCore<MongoUser>()
                .AddMongoStores(Configuration, "Mongo:Identity");
        }
    }
}

```

The above example is the simplest access point to the functions provided, and if you want to explore more how to tweak your configuration of the layer, visit the [Documentation](docs/README.md) we have provided 

## Contribute

Contributions to open-source projects, like **Deveel Identity - MongoDB**, is generally driven by interest in using the product and services, if they would respect some of the expectations we have to its functions.

The best ways to contribute and improve the quality of this project is by trying it, filing issues, joining in design conversations, and make pull-requests.

Please refer to the [Contributing Guidelines](CONTRIBUTING.md) to receive more details on how you can contribute to this project.

We aim to address most of the questions you might have by providing [documentations](docs/README.md), answering [frequently asked questions](docs/FAQS.md) and following up on issues like bug reports and feature requests.

### Contributors

<a href="https://github.com/deveel/deveel.identity.mongodb/graphs/contributors">
<img src="https://contrib.rocks/image?repo=deveel/deveel.identity.mongodb"/>
</a>

## License Information

This project is released under the [Apache 2 Open-Source Licensing agreement](https://www.apache.org/licenses/LICENSE-2.0).
