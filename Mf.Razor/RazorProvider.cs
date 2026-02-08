using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mf.Razor.Entity.Interfaces;
using Mf.Razor.Services;

namespace Mf.Razor
{
    public static class RazorProvider
    {
        public static IServiceCollection AddBootstrap5(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }

        public static IServiceCollection AddLogger(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IFrontLogger, LogService>();
            serviceCollection.AddSingleton<LogService>();
            return serviceCollection;
        }
    }
}
