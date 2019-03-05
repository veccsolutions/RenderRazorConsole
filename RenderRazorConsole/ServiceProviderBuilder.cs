using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using RenderRazorConsole;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderBuilder
    {
        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddLogging((builder) =>
            {
                builder.AddConsole(options =>
                {
                    options.IncludeScopes = true;
                });
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            services.AddRazor();

            services.AddSingleton<RazorRunner>();
            services.AddSingleton<TestInjection>();

            var result = services.BuildServiceProvider();
            return result;
        }

        public static IServiceCollection AddRazor(this IServiceCollection services)
        {
            var hostingEnvironment = new HostingEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name
            };

            services.AddSingleton<IHostingEnvironment>(hostingEnvironment);
            services.AddSingleton<DiagnosticSource>((IServiceProvider serviceProvider) => new DiagnosticListener("DummySource"));
            services.AddTransient<ObjectPoolProvider, DefaultObjectPoolProvider>();

            services.AddMvcCore()
                    .AddRazorViewEngine(options =>
                    {
                        options.AllowRecompilingViewsOnFileChange = false;
                        options.FileProviders.Add(new VirtualFileProvider());
                    });

            return services;
        }
    }
}
