using System;

namespace CurrencyConversion
{
    public class CurrencyDoesNotExistException : Exception
    {
        public CurrencyDoesNotExistException(string currency): base(currency)
        {}
    }
}