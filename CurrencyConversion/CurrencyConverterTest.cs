using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeMock.ArrangeActAssert;

namespace CurrencyConversion
{
    [TestClass]
    public class CurrencyConverterTest
    {

//        [TestMethod, Isolated]
        public void ReturnsReasonableRatesWithTypemock()
        {
            Isolate.WhenCalled(() => CurrencyConverter.AllSymbols())
                .WillReturn(new List<string>{"USD", "EUR"});
            var stubWebClient = Isolate.Fake.Instance<WebClient>();
            Isolate.Swap.NextInstance<WebClient>().With(stubWebClient);
            Isolate.WhenCalled(() => stubWebClient.DownloadString(string.Empty))
                .WillReturn("<div id=\"converter_results\"><ul><li><b>1 a = 42 b</b>");
            Assert.AreEqual(42, CurrencyConverter.ConvertFromTo("EUR", "USD"));
        }

        [TestMethod]
        public void ReturnsReasonableRates()
        {
            var rate = CurrencyConverter.ConvertFromTo("EUR", "USD");
            Assert.IsTrue(1 < rate && rate < 2);
        }
    }
}
