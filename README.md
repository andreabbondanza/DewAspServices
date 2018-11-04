# DewAspServices

An alternative way to use services on Asp Net Core.

## Logic

There are two type of services:
- __Services__ 
- __RootServices__.

__RootServices__  are services that must be registred with arguments. The main difference from normal services is that when you initialize a __RootServices__ you usually do this from Startup passing it options, arguments, etc. in that scoping.

__Services__ are usually arguments free.

Both implements __IService__ interface

### Reason

When you request a service, the system if doesn't find it, it will be instantiate. However this can be a problem for a __RootService__ if it is declared as dependency of another service that request it. In that case it will throw an __ServiceNotInitializedException__.

This because, when you create the service from startup with the requested arguments, you must pass __isRoot__ argument to true, that isn't passed when a __RootService__ is created into the request.

This way to be has been choseed because is better to get a __ServiceNotInitializedException__ and understand the problem that get a generic Exception (nullreference, keynotfound, etc.) and get mad.

A normal service however, can be requested also with arguments, but you have to be careful about exceptions if your service is another service dependency.

## Services scoping

You can register three types of service:
- __Singleton:__ _a service that will remain in the memory for ALL the server uptime_
- __Scoped:__ _a service that will remain in the memory for the time of the http request
- __Instance__ _a service that will remain in memory in the scoped call

## Container class

The container class has the task to distribuite the services, it has:

- __GetServiceSingleton\<T\>__: a method that return the instance of T (if exists) or create one of it, it works via singleton patterns, so it will be the same for all the serverup time
- __GetServiceScoped\<T\>__: a method that return the instance of T (if exists) or create one of it, it works for all ther time of the request
- __GetServiceInstance\<T\>__: a method that return the instance of T (if exists) or create one of it, it works for instance (like a new)
- __GetService\<T\>__: a method that return the instance of T (if exists) or create one of it. It call in order:
- - 1) GetServiceSingleton 
- - 2) GetServiceScoped 
- - 3) GetServiceInstance

## How it works

To get the current service container you just need to call:
```csharp
HttpContext.GetServiceContainer();
```
In the controller.

### Startup class

Probably you want to configure services before the controller action starts, for this there is a __Middleware__ that works like the _ConfigureServices_ method in the Startup Class.

Here an example:

```csharp
 app.UseDewAspServices(serviceContainer =>
{
    var cfService = serviceContainer.GetServiceSingleton<ConfigService>(); // initialize service
    cfService.DoStuff(); // if you need do stuff
});
```
With async:
```csharp
 app.UseDewAspServices(async (serviceContainer) =>
{
    serviceContainer.GetServiceSingleton<FileService>(new ServiceArgs().Add(env.ContentRootFileProvider).Add(env.ContentRootPath), true); // example with args
    var confService = services.GetServiceSingleton<ConfigService>();
    await confService.ReloadService();
});
```

Note the __true__ second argument for _FileService_ service. It indicates that the FileService is a __ROOT__ service.

### Passing arguments to a Service

As you can see from the examples above, to pass arguments to a service initialization you must use the __ServiceArgs__ collection type.

To pass an argument you need just to do:

```csharp
serviceContainer.GetServiceSingleton<FileService>(new ServiceArgs().Add(env.ContentRootFileProvider).Add(env.ContentRootPath), true); // example with args
// OR 
serviceContainer.GetServiceSingleton<FileService>(new ServiceArgs().Add("FileProvider",env.ContentRootFileProvider).Add("RootPath",env.ContentRootPath), true); // example with args
```

In this example we are passing the _ContentRootFileProvider_ and the _ContentRoot_ path to our FileService.

Let's see how we can read them:

```csharp
public void InitService(ServiceArgs param)
{
    _providerByType = param.GetArgument<PhysicalFileProvider>(); // get file provider by its type
    _pathByTypeKey = param.GetArgument("String") as string; // get root path by its type used as Key
    _pathByKey = param.GetArgument("RootPath"); // get root path by its key
}
```
