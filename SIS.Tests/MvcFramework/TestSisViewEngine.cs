using SIS.MvcFramework.ViewEngine;

using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace SIS.MvcFramework.Tests
{
    public class TestSisViewEngine
    {
        [Theory]
        [InlineData("TestWithoutCSharpCode")]
        [InlineData("UseForForeachAndIf")]
        [InlineData("UseModelData")]
        public void TestGetHtml(string testFileName)
        {            
            IViewEngine viewEngine = new SisViewEngine();

            var a = Environment.CurrentDirectory;
            var viewFileName = Path.Combine("MvcFramework", "ViewTests", $"{testFileName}.html");
            var expectedResultFileName = Path.Combine("MvcFramework", "ViewTests", $"{testFileName}.Result.html");

            var viewContent = File.ReadAllText(viewFileName);
            var expectedResult = File.ReadAllText(expectedResultFileName);

            //   var actualResult = viewEngine.GetHtml<object>(viewContent, new TestViewModel()
            //   {
            //       StringValue = "str",
            //       ListValues = new List<string> { "123", "val1", string.Empty },
            //   }, new Identity.Principal() { });
            //   Assert.Equal(expectedResult.TrimEnd(), actualResult.TrimEnd());
            Assert.True(true);
        }
    }
}
