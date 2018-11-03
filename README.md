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

A normal service however, can be requested also with arguments.

## Services scoping

You can register three types of service:
- __Singleton:__ _a service that will remain in the memory for ALL the server uptime_
- __Scoped:__ _a service that will remain in the memory for the time of the http request
- __Instance__ _a service that will remain in memory in the scoped call

## Container class

