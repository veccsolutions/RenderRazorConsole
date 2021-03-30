using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
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
            var hostingEnvironment = new CustomWebHostEnvironment
            {
                ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name
            };

            services.AddSingleton<IWebHostEnvironment>(hostingEnvironment);

            var diagnosticListener = new DiagnosticListener(hostingEnvironment.ApplicationName);
            services.AddSingleton<DiagnosticSource>(diagnosticListener);
            services.AddSingleton<DiagnosticListener>(diagnosticListener);
            services.AddTransient<ObjectPoolProvider, DefaultObjectPoolProvider>();

            services.AddMvcCore()
                .AddRazorViewEngine()
                    .AddRazorRuntimeCompilation(options =>
                    {
                        options.FileProviders.Clear();
                        options.FileProviders.Add(new VirtualFileProvider());
                    });

            return services;
        }
    }
}
