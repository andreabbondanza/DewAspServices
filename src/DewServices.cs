using System;
using System.Collections.Generic;

namespace DewCore.AspNetCore.Services
{
    /// <summary>
    /// Service argument class
    /// </summary>
    public class DewServiceArgs
    {
        private Dictionary<string, object> _args = new Dictionary<string, object>();
        public DewServiceArgs Add(string key, object value)
        {
            _args.Add(key, value);
            return this;
        }
        /// <summary>
        /// Throw exceptino for object with same type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DewServiceArgs Add(object value)
        {
            _args.Add(value.GetType().Name, value);
            return this;
        }
        public T GetArgument<T>(string key) where T : class
        {
            return _args[key] as T;
        }
        public object GetArgument(string key)
        {
            return _args[key];
        }
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
    public class DewServices : IDewServices
    {
        private static DewServices _currServices = null;
        public static DewServices GetServices()
        {
            if (_currServices == null)
                _currServices = new DewServices();
            return _currServices;
        }
        private static Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private Dictionary<Type, object> _services = new Dictionary<Type, object>();
        public T GetServiceScoped<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            else
            {
                this.RegisterScoped<T>(param);
                return GetServiceScoped<T>(param);
            }
        }
        private void RegisterScoped<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            if (!_services.ContainsKey(typeof(T)))
            {
                var service = new T();
                if (param == null && service is IRootService)
                    throw new Exception("Rootservice must have injected init params, you must initialize byself before other services");
                service.RequestDependencyServices(this);
                service.InitService(param);
                _services.Add(typeof(T), service);
            }
        }
        private void RegisterSingleton<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            if (!_singletons.ContainsKey(typeof(T)))
            {
                var service = new T();
                if (param == null && service is IRootService)
                    throw new Exception("Rootservice must have injected init params, you must initialize byself before other services");
                service.RequestDependencyServices(this);
                service.InitService(param);
                _singletons.Add(typeof(T), service);
            }
        }
        public T GetServiceInstance<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            var service = new T();
            if (param == null && service is IRootService)
                throw new Exception("Rootservice must have injected init params, you must initialize byself before other services");
            service.RequestDependencyServices(this);
            service.InitService(param);
            return service;

        }
        public T GetServiceSingleton<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            if (_singletons.ContainsKey(typeof(T)))
                return (T)_singletons[typeof(T)];
            else
            {
                this.RegisterSingleton<T>(param);
                return GetServiceSingleton<T>(param);
            }
        }
        /// <summary>
        /// Return a service of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetService<T>(DewServiceArgs param = null) where T : class, IService, new()
        {
            if (_singletons.ContainsKey(typeof(T)))
                return (T)_singletons[typeof(T)];
            if (_services.ContainsKey(typeof(T)))
                return (T)_services[typeof(T)];
            return GetServiceInstance<T>(param);
        }
    }
}
