using System;
using System.Linq;
using System.Threading.Tasks;
using DewCore.AspNetCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

/// <summary>
/// Middleware DewAspServices class
/// </summary>
public class DewAspServicesMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Action<IServicesContainer> _action;
    private readonly Func<IServicesContainer, Task> _asyncAction;
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Next middleware</param>
    /// <param name="action">Configure service action</param>
    public DewAspServicesMiddleware(RequestDelegate next, Action<IServicesContainer> action)
    {
        _next = next;
        _action = action;
    }
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Next middleware</param>
    /// <param name="action">Configure service action</param>
    public DewAspServicesMiddleware(RequestDelegate next, Func<IServicesContainer, Task> action)
    {
        _next = next;
        _asyncAction = action;
    }
    /// <summary>
    /// Invoke method
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var scontainer = ServicesContainer.GetServices();
        context.Items.Add("DewServiceContainer", scontainer);
        _action?.Invoke(scontainer);
        if (_asyncAction != null)
        {
            await _asyncAction.Invoke(scontainer);
        }
        await _next(context);
    }
}

/// <summary>
/// HTTPContext Extension class
/// </summary>
public static class ServiceContainerHttpContextExtension
{
    /// <summary>
    /// Return service container
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IServicesContainer GetServiceContainer(this HttpContext context)
    {
        var data = context.Items.FirstOrDefault(x => x.Key as string == "DewServiceContainer");
        return data.Value as IServicesContainer;
    }

}
/// <summary>
/// Dew ASP SERVICES pipeline builder extension
/// </summary>
public static class DewAspServicesBuilderExtension
{
    /// <summary>
    /// Builder method
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureServices">Configure service action string</param>
    /// <returns></returns>
    public static IApplicationBuilder UseDewAspServices(
       this IApplicationBuilder builder, Action<IServicesContainer> configureServices)
    {
        return builder.UseMiddleware<DewAspServicesMiddleware>(configureServices);
    }
    /// <summary>
    /// Builder method
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureServices">Configure service action string</param>
    /// <returns></returns>
    public static IApplicationBuilder UseDewAspServices(
       this IApplicationBuilder builder, Func<IServicesContainer, Task> configureServices)
    {
        return builder.UseMiddleware<DewAspServicesMiddleware>(configureServices);
    }
}