using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace RenderRazorConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var sp = ServiceProviderBuilder.BuildServiceProvider();
            var viewEngine = sp.GetRequiredService<IRazorViewEngine>();

            try
            {
                var razorRunner = sp.GetRequiredService<RazorRunner>();
                while (true)
                {
                    var rendered = await razorRunner.Render("custom:\\testapp\\test.cshtml");
                    Console.WriteLine(rendered);

                    rendered = await razorRunner.Render("custom:\\testapp\\model.cshtml", new TestModel { Values = new[] { "test", "model", "array", "stuff" } });
                    Console.WriteLine(rendered);

                    rendered = await razorRunner.Render("custom:\\testapp\\injected.cshtml", new TestModel { Values = new[] { "test-injected", "model-injected", "array-injected", "stuff-injected" } });
                    Console.WriteLine(rendered);

                    Console.ReadLine();
                }
             }
            catch (Exception exception)
            {
                Console.WriteLine("ERROR: " + exception);
            }
            Console.ReadLine();
        }
    }
}
