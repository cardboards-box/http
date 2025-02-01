# CardboardBox.Http
Extension to HttpClientFactory that exposes common Http Request methods.

Upgrading? Be sure to check the [Breaking Changes](#breaking-changes) section for any changes that might affect your code.

## Installation
You can install the NuGet package within Visual Studio. It targets .net standard 2.1.

```
PM> Install-Package CardboardBox.Http
```

## Dependencies
This library depends on the `IJsonService` provided in `CardboardBox.Json` and will need to be provided in the dependency injection setup.
By default the library will use the `System.Text.Json` library for serialization and deserialization.
This can be changed by providing a custom implementation of `IJsonService` in the dependency injection setup.
There is also a default implementation of `IJsonService` provided in `CardboardBox.Json` that uses `Newtonsoft.Json` for serialization and deserialization.

You will need to use one of the following to register the `IJsonService` in the dependency injection setup:
```csharp
using CardboardBox.Json;
using Microsoft.Extensions.DependencyInjection;
...
//Uses System.Text.Json (You can pass in a custom JsonSerializerOptions instance)
services.AddJson(); 

//Uses Newtonsoft.Json (you'll need to include the ``CardboardBox.Json.Newtonsoft`` package)
services.AddNewtonsoftJson(); 

//Alternatively you can provide your own implementation of IJsonService
services.AddJson<MyCustomJsonService>();
```

## Setup
You can setup the Api Client using the following code:

Where ever you register your services with Dependency Injection, you can add: 
```csharp
using CardboardBox.Http;
using Microsoft.Extensions.DependencyInjection;
...
services.AddCardboardHttp();
```

This will register the IHttpClientFactory as well as all of the other dependencies necessary for handling `CardboardBox.Http`.

## Usage
Once CardboardBox.Http is registered with your service collection you can inject the `IApiService` into any of your services and get access to all of the default methods

```csharp
using CardboardBox.Http;

namespace ExampleHttp;

public class SomeService(IApiService _api)
{
  public Task<SomeModel> GetSomething()
  {
    return _api.Get<SomeModel>("https://example.org");
  }
}

```

## Breaking Changes

### 2.x.x >> 3.x.x
There were some base changes in 3.x.x that might break anyone using 2.x versions:

#### Caching Removed
There were some default caching methods in 2.x that were removed as they were causing issues with some of the new changes and they were hardly used by the people I surveyed. 
If you need caching, you can implement it yourself by extending the `IApiService` interface and adding your own caching logic.

#### Config Callback Changed
There are now various other things you can configure on the `IHttpBuilder` outside of just the `HttpRequestMessage`,
as such, the `config` parameter on all of the request methods in the `IApiService` have changed from `Action<HttpRequestMessage>` to `Action<IHttpBuilderConfig>`.
You can pipe your old configurations to the `IHttpBuilderConfig.Message()` method to get the same behavior. 

#### Newtonsoft.Json Removed
The underlying `CardboardBox.Json` package no longer implements a service for `Newtonsoft.Json` directly, 
so if you're using you will need to include the `CardboardBox.Json.Newtonsoft` package and register the service with it's extension method: `IServiceCollection.AddNewtonsoftJson()`.
This is to reduce the number of dependencies that are included by default and to resolve some virus scanning issues that were reporting false positives on the `Newtonsoft.Json` package.

> You should also consider moving to `System.Text.Json` as it's the new implementation for serialization in .net core and is faster than `Newtonsoft.Json`.

#### Config Options Moved to Extensions
Various configuration methods on `IHttpBuilder` have been moved to extension methods to simplify the builder and reduce the number of methods that need to be implemented when rolling your own `HttpBuilder` implementation.

The only configuration option that's signature has changed is the `IHttpBuilder.With(Action<HttpRequestMessage>)` method, 
which has been changed to `IHttpBuilder.With(Action<IHttpBuilderConfig>)`.
If you want the same behavior as before, you can use the `IHttpBuilder.Message(Action<HttpRequestMessage>)` method instead.

#### Api Methods Moved to Extensions
The API call methods on the `IApiService` have been moved to extension methods to simplify the code and commenting. 
You can still configure the `IApiService.Create()` method to change the default behavior of the `IHttpBuilder` if you need to.
Also, the `GenerateUri` methods have been moved to the `IHttpBuilder` interface as extension methods.

### 1.x.x >> 2.x.x
There were some base changes in 2.0 that might break anyone using 1.x versions:

#### Fail Gracefully
In version 1.x, by default `FailGracefully` was enabled on all requests. 
This would make it so if the request failed with a status code outside of the 200 range, it would just return `null`.
This has been turned off by default in 2.0.0. If you want to turn it back on, you can chain the `.FailGracefully()` builder on any `IHttpBuilder` or override the `Create()` method on the `IApiService`.

#### Fail With Throw
There is now a `FailWithThrow` option on the `IHttpBuilder` that allows for toggling the `FailGracefully` option, this previously didn't exist and was an oversight on my part.

#### Dependencies
The auto-inclusion of the `CardboardBox.Json` services in the dependency injection handler has been removed.
You will need to manually add the CardboardBox.Json package to your dependency injection handler otherwise you will receive an error trying to resolve the `IApiService`.
See the section in the installation instructions for more information.