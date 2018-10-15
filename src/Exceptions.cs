using System;
using DewCore.AspNetCore.Services;

namespace DewCore.AspNetCore.Services
{
    public class ServiceNotInitializedException : Exception
    {
        public ServiceNotInitializedException(): base("Service not initialized"){}
    }

    public class UnauthorizedRootServiceCallException:Exception{
        public UnauthorizedRootServiceCallException() : base("Unable to initialize the root service"){}
    }
}