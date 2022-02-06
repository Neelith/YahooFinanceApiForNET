# YahooFinanceApiForNET
.NET wrapper for Yahoo finance apis from https://www.yahoofinanceapi.com/

# Summary
This library is a simple wrapper for https://www.yahoofinanceapi.com/ apis written in .NET.
This library exposes the class YahooFinanceApiManager and its methods:
  - GetFinanceQuote
  - GetFinanceOptions
  - GetFinanceSpark
  - GetFinanceQuoteSummary
  - GetFinanceChart
  - GetFinanceRecommendationsBySymbol
  - GetFinanceScreener
  - GetFinanceInsights
  - GetFinanceAutocomplete
  - GetFinanceQuoteMarketSummary
  - GetFinanceTrending

Each method listed above has also a Raw counterpart (ex. GetFinanceQuoteRaw) from which you can retrieve the raw HttpResponse.

# How to use this library
All you need to do is register at https://www.yahoofinanceapi.com/ , grab an api key and initialize you YahooFinanceApiManager with that key.
After that you can call your methods and... that's all. You just got your market data.

If you have some bug or improvements to suggest let me know and if this has come in handy feel free to support me.
