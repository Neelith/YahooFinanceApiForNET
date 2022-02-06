using YahooFinanceApiForNET.Managers;

string apiKey = "pScHdfq2EnDmdGhNRmsS8qvPWZE17TC8ux3OsT11";

Console.WriteLine($"Enter symbols separated by comma and i will show you their current market price:\n");
string symbols = Console.ReadLine();
string[] splittedSymbols = symbols.Split(',');

HttpClient client = new HttpClient();
YahooFinanceApiManager yahooFinanceApiManager = new YahooFinanceApiManager(client, apiKey);

List<Dictionary<string, string>> securities = await yahooFinanceApiManager.GetFinanceQuoteAsync(splittedSymbols);

foreach (var security in securities)
{
    security.TryGetValue("regularMarketPrice", out string regularMarketPrice);
    security.TryGetValue("currency", out string currency);
    security.TryGetValue("symbol", out string symbol);

    Console.WriteLine($"The current market price for {symbol} is: {regularMarketPrice} {currency}");
}

Console.ReadLine();