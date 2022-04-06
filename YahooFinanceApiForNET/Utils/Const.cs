namespace YahooFinanceApiForNET.Utils
{
    internal static class Const
    {
        //KEYS
        internal const string lang = nameof(lang);
        internal const string region = nameof(region);
        internal const string symbols = nameof(symbols);
        internal const string symbol = nameof(symbol);
        internal const string date = nameof(date);
        internal const string apiKey = "X-API-KEY";
        internal const string quoteResponse = nameof(quoteResponse);
        internal const string optionChain = nameof(optionChain);
        internal const string result = nameof(result);
        internal const string applicationJson = "application/json";
        internal const string underlyingSymbol = nameof(underlyingSymbol);

        //PARTIAL API URLS
        internal const string financeQuote = "/finance/quote";
        internal const string financeOptions = "/finance/options";
        internal const string financeSpark = "/finance/spark";
    }
}
