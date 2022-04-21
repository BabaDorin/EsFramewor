using Application.EventSourcing.EsFramework;
using EsFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EsFramework.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddEventResolver(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<IEventResolver>(new EventResolver(assemblies));

            return services;
        }
    }
}
