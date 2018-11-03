using System;
using System.Collections.Generic;

namespace DewCore.AspNetCore.Services
{
    /// <summary>
    /// Service argument class
    /// </summary>
    public class ServiceArgs
    {
        private Dictionary<string, object> _args = new Dictionary<string, object>();
        /// <summary>
        /// Add a new service args
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">A key already exists</exception>
        public ServiceArgs Add(string key, object value)
        {
            _args.Add(key, value);
            return this;
        }
        /// <summary>
        /// Throw exceptino for object with same type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ServiceArgs Add(object value)
        {
            _args.Add(value.GetType().Name, value);
            return this;
        }
        /// <summary>
        /// Get an argument by key of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetArgument<T>(string key) where T : class
        {
            return _args[key] as T;
        }
        /// <summary>
        /// Get an argument by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetArgument(string key)
        {
            return _args[key];
        }
        /// <summary>
        /// Get an argument by T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetArgument<T>() where T : class
        {
            var key = typeof(T).Name;
            if (_args.ContainsKey(key))
                return _args[key] as T;
            return null;
        }
    }

    /// <summary>
    /// Current services class
    /// </summary>
    public class ServicesContainer : IServicesContainer
    {
        /// <summary>
        /// Initialize the services object
        /// </summary>
        /// <returns></returns>
        public static ServicesContainer GetServices() => new ServicesContainer();
        private static Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        /// <summary>
        /// Return a service scoped of type T
        /// </summary>
        /// <param name="param">Service args</param>
        /// <param name="isRoot">Must be true if you are calling a RootService</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ServiceNotInitializedException">A rooteservice init without isRoot = true></exception>
        public T GetServiceScoped<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            else
            {
                this.RegisterScoped<T>(param, isRoot);
                return GetServiceScoped<T>(param, isRoot);
            }
        }
        private void RegisterScoped<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                var service = new T();
                if (!isRoot && service is IRootService)
                    throw new ServiceNotInitializedException();
                service.RequestDependencyServices(this);
                service.InitService(param);
                _services.Add(typeof(T), service);
            }
        }
        private void RegisterSingleton<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            if (!_singletons.ContainsKey(typeof(T)))
            {
                var service = new T();
                if (!isRoot && service is IRootService)
                    throw new ServiceNotInitializedException();
                service.RequestDependencyServices(this);
                service.InitService(param);
                _singletons.Add(typeof(T), service);
            }
        }
        /// <summary>
        /// Return a service instance of type T
        /// </summary>
        /// <param name="param">Service args</param>
        /// <param name="isRoot">Must be true if you are calling a RootService</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ServiceNotInitializedException">A rooteservice init without isRoot = true></exception>
        public T GetServiceInstance<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            var service = new T();
            if (!isRoot && service is IRootService)
                throw new ServiceNotInitializedException();
            service.RequestDependencyServices(this);
            service.InitService(param);
            return service;

        }
        /// <summary>
        /// Return a service singleton of type T
        /// </summary>
        /// <param name="param">Service args</param>
        /// <param name="isRoot">Must be true if you are calling a RootService</param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ServiceNotInitializedException">A rooteservice init without isRoot = true></exception>
        public T GetServiceSingleton<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            if (_singletons.ContainsKey(typeof(T)))
                return (T)_singletons[typeof(T)];
            else
            {
                this.RegisterSingleton<T>(param, isRoot);
                return GetServiceSingleton<T>(param, isRoot);
            }
        }
        /// <summary>
        /// Return a service of type T
        /// </summary>
        /// <param name="param">Service args</param>
        /// <param name="isRoot">Must be true if you are calling a RootService</param>
        /// <typeparam name="T"></typeparam>
        public T GetService<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new()
        {
            if (_singletons.ContainsKey(typeof(T)))
                return (T)_singletons[typeof(T)];
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            return GetServiceInstance<T>(param);
        }

        private ServicesContainer() { }
    }
}
