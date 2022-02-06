using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp.SourceGenerators.Extensions.Models;
using Moq;
using ProxyInterfaceSourceGenerator;
using ProxyInterfaceSourceGeneratorTests.Source;
using ProxyInterfaceSourceGeneratorTests.Source.PnP;
using Xunit;

namespace ProxyInterfaceSourceGeneratorTests
{
    public class PnPTests
    {
        public PnPTests()
        {
           
        }

        [Fact]
        public void X()
        {
            var webMock = new Mock<IWeb>();


            var ccMock = new Mock<IClientContext>();
          //  ccMock.SetupGet(cc => cc.Web).Returns(webMock.Object);

        }
    }
}
