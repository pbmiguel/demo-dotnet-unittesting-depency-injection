using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NUnit.Framework;
using WebApp;

public class MyAppStartupTests
{
    public class EmptyStartup
    {
        public EmptyStartup(IConfiguration _) { }

        public void ConfigureServices(IServiceCollection _) { }

        public void Configure(IApplicationBuilder _) { }
    }
    
    [Test]
    public void Startup_WhenBuildServiceCollection_ShouldDependenciesBeRegistered()
    {
        Startup startup = null;
        IServiceCollection serviceCollection = null;
        WebHost
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Clear();
                config.AddConfiguration(hostingContext.Configuration);
                config.AddJsonFile("appsettings.json");
                startup = new Startup();
            })
            .ConfigureServices(sc =>
            {
                startup.ConfigureServices(sc);
                serviceCollection = sc;
            })
            .UseStartup<EmptyStartup>()
            .Build();

        // Get all classes from Startup assembly
        var classes = GetTypes(typeof(Startup).Assembly)
        .Where(IsClass);

        // Search Constrollers
        var controllers = classes
        .Where(IsSubclassOf<Controller>);

        controllers
        .ToList().ForEach(serviceCollection.TryAddTransient);

        // Search ViewComponents
        classes
        .Where(IsSubclassOf<ViewComponent>)
        .ToList().ForEach(serviceCollection.TryAddTransient);

        // Search Action Controller parameters with [FromServices] attribute.
        var controllerActions = controllers
        .SelectMany(DeclaredInstanceMethods)
        .SelectMany(Parameters)
        .Where(HasAttribute<FromServicesAttribute>)
        .Select(ParameterType);
        
        // Register defaut middleware dependencies;
        serviceCollection.TryAddTransient<RequestDelegate>(sp => context => Task.CompletedTask);

        //Search Middlewares
        var middlewares = classes
        .Where(IsMiddleware);

        middlewares
        .ToList().ForEach(serviceCollection.TryAddTransient);

        // Search Middlewares InvokeAsync parameters 
        var middlewareInvokeAsyncs = middlewares
        .SelectMany(DeclaredInstanceMethods)
        .Where(IsInvokeAsyncMethod)
        .SelectMany(Parameters)
        .Select(ParameterType)
        .Where(IsNotMiddlewareDefaultTypes);
        
        // Validate Types
        controllerActions
        .Union(middlewareInvokeAsyncs)
        .Distinct()
        .Select(DependencyServiceType)
        .ToList().ForEach(serviceCollection.TryAddTransient);

        try
        {
            serviceCollection.BuildServiceProvider(new ServiceProviderOptions()
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    private IEnumerable<Type> GetTypes(Assembly assembly) => assembly.GetTypes();

    private bool IsClass(Type type) => type.IsClass;

    private bool IsSubclassOf<T>(Type type) => type.IsSubclassOf(typeof(T)) && !type.IsAbstract;

    private bool IsMiddleware(Type type) => type.Name.Contains("middleware", StringComparison.InvariantCultureIgnoreCase) || IsSubclassOf<IMiddleware>(type);

    private IEnumerable<MethodInfo> DeclaredInstanceMethods(Type type) =>
        type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

    private bool IsInvokeAsyncMethod(MethodInfo methodInfo) =>
        methodInfo.Name.Equals("InvokeAsync", StringComparison.InvariantCultureIgnoreCase);

    private IEnumerable<ParameterInfo> Parameters(MethodInfo methodInfo) => methodInfo.GetParameters();

    private Type ParameterType(ParameterInfo parameterType) => parameterType.ParameterType;

    private IEnumerable<T> Attributes<T>(ParameterInfo parameterInfo) where T : Attribute =>
        parameterInfo.GetCustomAttributes<T>();

    private bool HasAttribute<T>(ParameterInfo parameterInfo) where T : Attribute =>
        Attributes<T>(parameterInfo).Any();

    private bool IsNotMiddlewareDefaultTypes(Type type) => type != typeof(HttpContext) && type != typeof(RequestDelegate);

    public static Type DependencyServiceType(Type dependencyType) =>
        typeof(DependencyService<>).MakeGenericType(dependencyType);

    class DependencyService<T>
    {
        public DependencyService(T _)
        {
        }
    }
}