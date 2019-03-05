using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace RenderRazorConsole
{
    public class RazorRunner
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly IServiceProvider _serviceProvider;

        public RazorRunner(IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider)
        {
            this._razorViewEngine = razorViewEngine;
            this._serviceProvider = serviceProvider;
        }

        public async Task<string> Render(string viewPath, object model = null)
        {
            var httpContext = new DefaultHttpContext() { RequestServices = _serviceProvider };

            var routeData = new RouteData();
            var actionDescriptor = new ActionDescriptor();
            var modelStateDictionary = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var tempDataProvider = new VirtualTempDataProvider();
            var htmlHelperOptions = new HtmlHelperOptions();

            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor, modelStateDictionary);
            var viewDataDictionary = new ViewDataDictionary(modelMetadataProvider, modelStateDictionary);
            var tempDataDictionary = new TempDataDictionary(httpContext, tempDataProvider);

            viewDataDictionary.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var view = _razorViewEngine.GetView(string.Empty, viewPath, true);
                var viewContext = new ViewContext(actionContext, view.View, viewDataDictionary, tempDataDictionary, stringWriter, htmlHelperOptions);

                await view.View.RenderAsync(viewContext);

                var result = stringWriter.ToString();
                return result;
            }
        }
    }
}
