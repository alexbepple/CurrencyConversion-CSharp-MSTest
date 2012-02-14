using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace CurrencyConversion
{
    class CurrencyConverter
    {
        private static DateTime _lastRead;
        private static List<string> _allSymbolsCache;

        public static Decimal ConvertFromTo(string fromCurrency, string toCurrency)
        {
            if (!AllSymbols().Contains(fromCurrency))
                throw new CurrencyDoesNotExistException(fromCurrency);
            if (!AllSymbols().Contains(toCurrency))
                throw new CurrencyDoesNotExistException(toCurrency);
            var url = String.Format(
                "http://www.gocurrency.com/v2/dorate.php?" 
                + "inV=1&from={0}&to={1}&Calculate=Convert",
                fromCurrency, toCurrency);
            var client = new WebClient();
            var result = new WebClient().DownloadString(url);

            var index = result.IndexOf("<div id=\"converter_results\"><ul><li>");
            var theGoodStuff = result.Substring(index);
            var startIndex = theGoodStuff.IndexOf("<b>") + 3;
            var endIndex = theGoodStuff.IndexOf("</b>");
            var importantStuff = theGoodStuff.Substring(startIndex, endIndex - startIndex);
            var parts = importantStuff.Split('=');
            var almostValue = parts[1].Trim().Split(' ')[0];
            return Decimal.Parse(almostValue, new CultureInfo("en"));
        }

        public static List<string> AllSymbols()
        {
            if (_allSymbolsCache != null
                && DateTime.Now.Subtract(_lastRead).TotalMinutes < 5)
                return _allSymbolsCache;

            var client = new WebClient();
            var url = "http://www.jhall.co.uk/currency/by_currency.html";
            var result = client.DownloadString(url);
            _lastRead = DateTime.Now;

            var foundTable = false;
            _allSymbolsCache = new List<string>();
            foreach (var s in result.Split('\r', '\n'))
            {
                if (foundTable)
                    if (Regex.IsMatch(s, "\\s+<td valign=top>[A-Z]{3}</td>"))
                        _allSymbolsCache.Add(
                            new Regex("</td>").Replace(
                                new Regex(".*top>").Replace(s, ""), ""));
                if (s.StartsWith("<h3>Currency Data"))
                    foundTable = true;
            }
            return _allSymbolsCache;
        }
    }
}
