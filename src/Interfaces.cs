using System;

namespace DewCore.AspNetCore.Services
{
    /// <summary>
    /// Services interface
    /// </summary>
    public interface IServicesContainer
    {
        /// <summary>
        /// Get a service. Check before singletons, after scoping and if not found it will return instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetService<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new();
        /// <summary>
        /// Get Service istance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetServiceInstance<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new();
        /// <summary>
        /// Get Service scoped
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetServiceScoped<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new();
        /// <summary>
        /// Get singleton
        /// </summary>
        /// <typeparam name="T"></typeparam>
        T GetServiceSingleton<T>(ServiceArgs param = null, bool isRoot = false) where T : class, IService, new();
    }
    /// <summary>
    /// Tag interface for root services
    /// </summary>
    public interface IRootService : IService
    {

    }
    /// <summary>
    /// Service interface
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// Request the dependencies from services object
        /// </summary>
        /// <param name="services"></param>
        void RequestDependencyServices(IServicesContainer services);
        /// <summary>
        /// Init a service
        /// </summary>
        /// <param name="param"></param>
        void InitService(ServiceArgs param = null);
    }
}
