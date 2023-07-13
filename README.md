# CardboardBox.Http
Extension to HttpClientFactory that exposes common Http Request methods

## Breaking Chnages in 2.0.0
There were some base changes in 2.0 that might break anyone using 1.x versions:

### Fail Gracefully
In version 1.x, by default `FailGracefully` was enabled on all requests. 
This would make it so if the request failed with a status code outside of the 200 range, it would just return `null`.
This has been turned off by default in 2.0.0. If you want to turn it back on, you can chain the `.FailGracefully()` builder on any `IHttpBuilder` or override the `Create()` method on the `IApiService`.

### Fail With Throw
There is now a `FailWithThrow` option on the `IHttpBuilder` that allows for toggling the `FailGracefully` option, this previously didn't exist and was an oversight on my part.

### Dependencies
The auto-inclusion of the `CardboardBox.Json` services in the dependency injection handler has been removed.
You will need to manually add the CardboardBox.Json package to your dependency injection handler otherwise you will receive an error trying to resolve the `IApiService`.
See the section in the installation instructions for more information.

## Installation
You can install the nuget package within Visual Studio. It targets .net standard 2.1.

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

//Uses Newtonsoft.Json
services.AddJson<NewtonsoftJsonService>(); 

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

namespace ExampleHttp
{
  public class SomeService 
  {
	private readonly IApiService _api;

	public SomeService(IApiService api)
	{
	  _api = api;
	}

	public Task<SomeModel> GetSomething()
	{
	  return _api.Get<SomeModel>("https://example.org");
	}
  }
}
```

