using Microsoft.Extensions.DependencyInjection;

namespace MotimatFrameWork.Razor
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
            return serviceCollection;
        }
    }
}
