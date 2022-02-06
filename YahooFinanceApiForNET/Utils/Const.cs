namespace YahooFinanceApiForNET.Utils
{
    internal static class Const
    {
        //KEYS
        internal const string lang = nameof(lang);
        internal const string region = nameof(region);
        internal const string symbols = nameof(symbols);
        internal const string ApiKey = "X-API-KEY";
        internal const string quoteResponse = nameof(quoteResponse);
        internal const string result = nameof(result);
        internal const string ApplicationJson = "application/json";

        //PARTIAL API URLS
        internal const string financeQuote = "/finance/quote";
    }
}
