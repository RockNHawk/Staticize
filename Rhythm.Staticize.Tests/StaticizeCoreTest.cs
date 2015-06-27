using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Rhythm.Staticize;

namespace Rhythm.Staticize
{
    [TestClass]
    public class StaticizeCoreTest
    {
        [TestMethod]
        public void UrlExtensionsTest()
        {
            var uri = new Uri("http://localhost/a/b/c/1.html");
            string parentDirectory = UrlExtensions.GetFileDirectory(uri);
            Assert.AreEqual(parentDirectory, "/a/b/c/");

            var parentAddress = UrlExtensions.GetParent(uri);
            Assert.AreEqual(parentAddress, "http://localhost/a/b/c/");
        }

    }
}
