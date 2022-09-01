using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

namespace SharpTileRenderer.Tests
{
    public class UriTest
    {
        [Test]
        public void TestURI()
        {
            var b = new Uri("content:///fileName", UriKind.Absolute);
            var x = new Uri("../test", UriKind.Relative);

            var r = new Uri(b, x);
            r.AbsolutePath.Should().Be("/test");
            try
            {
                // ReSharper disable once ObjectCreationAsStatement
                new UriBuilder(x);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                
            }
        }

        [Test]
        public void ResolveTest()
        {
            Console.WriteLine(Path.Combine("test.xml", ".."));
            Console.WriteLine(Path.GetRelativePath(".", Path.Combine("test.xml", "../test")));
        }
    }
}
