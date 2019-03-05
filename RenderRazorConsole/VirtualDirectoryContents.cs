using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.FileProviders;

namespace RenderRazorConsole
{
    public class VirtualDirectoryContents : IDirectoryContents
    {
        public bool Exists => true;

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            yield return TestFile.Value;
            yield return ModelFile.Value;
            yield return InjectionFile.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Lazy<IFileInfo> TestFile { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("custom:\\testapp\\test.cshtml",
                                                          "test.cshtml",
                                                          DateTimeOffset.Now,
                                                          false,
                                                          (info) => Encoding.Default.GetBytes("@(System.DateTime.Now)")));

        public static Lazy<IFileInfo> ModelFile { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("custom:\\testapp\\model.cshtml",
                                                          "model.cshtml",
                                                          DateTimeOffset.Now,
                                                          false,
                                                          (info) => Encoding.Default.GetBytes(@"@model RenderRazorConsole.TestModel
@foreach (var item in Model.Values)
{
<TEXT>@item
</TEXT>
}
")));

        public static Lazy<IFileInfo> InjectionFile { get; } =
            new Lazy<IFileInfo>(() => new VirtualFileInfo("custom:\\testapp\\injection.cshtml",
                                                          "injection.cshtml",
                                                          DateTimeOffset.Now,
                                                          false,
                                                          (info) => Encoding.Default.GetBytes(@"@using RenderRazorConsole
@model TestModel
@inject TestInjection _testInjection;

Foreach:
@foreach (var item in Model.Values)
{
<TEXT>@item
</TEXT>
}

Injected:
@(_testInjection.Value)

Partial:
@Html.Partial(""test.cshtml"")

Partial With Model
@Html.Partial(""model.cshtml"", Model)
")));
    }
}
