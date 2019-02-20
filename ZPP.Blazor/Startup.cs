using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZPP.Blazor;

namespace ZPP_Blazor
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Blazor.Extensions.Storage
            // Both SessionStorage and LocalStorage are registered
            services.AddStorage();
        }

        public void Configure(IBlazorApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
