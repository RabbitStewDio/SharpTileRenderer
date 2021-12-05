using NUnit.Framework;
using System;

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
            Console.WriteLine(r.AbsolutePath);
        }
    }
}
