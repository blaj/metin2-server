using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Metin2Server.Shared.Extensions;

public static class MediatRRegistrationExtensions
{
    public static IServiceCollection AddSlicesMediatR(this IServiceCollection services, Assembly assembly)
    {
        var sliceAssemblies = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null)
            .Select(t => t.Assembly)
            .Distinct()
            .ToArray();

        if (sliceAssemblies.Length != 0)
        {
            services.AddMediatR(sliceAssemblies);
        }

        return services;
    }
}