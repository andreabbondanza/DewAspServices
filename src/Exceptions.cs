using System;
using DewCore.AspNetCore.Services;

namespace DewCore.AspNetCore.Services
{
    /// <summary>
    /// Root Service invalid request exception
    /// </summary>
    public class ServiceNotInitializedException : Exception
    {
        /// <summary>
        /// Root Service invalid request exception
        /// </summary>
        public ServiceNotInitializedException(): base("Rootservice must be initialized with isRoot to true to avoid unexpected exception for dependencies missing"){}
    }

}