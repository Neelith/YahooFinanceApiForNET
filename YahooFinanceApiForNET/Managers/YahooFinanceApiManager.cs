using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using YahooFinanceApiForNET.Utils;

namespace YahooFinanceApiForNET.Managers
{
    public class YahooFinanceApiManager
    {
        #region Fields and Props

        private readonly string apiKey = string.Empty;
        private readonly string baseUrl = string.Empty;

        private readonly HttpClient httpClient;

        #endregion

        #region Ctor
        public YahooFinanceApiManager(HttpClient httpClient, string apiKey, string baseUrl = "https://yfapi.net")
        {
            this.apiKey = apiKey.IsNullOrWhiteSpace()
                ? throw new ArgumentException($"'{nameof(apiKey)}' cannot be null or whitespace.", nameof(apiKey))
                : apiKey;
            this.baseUrl = baseUrl.IsNullOrWhiteSpace()
                ? throw new ArgumentException($"'{nameof(baseUrl)}' cannot be null or whitespace.", nameof(baseUrl))
                : baseUrl;
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        #endregion

        #region /Finance/Quote API

        /// <summary>
        /// This method wraps the api https://yfapi.net/v6/finance/quote. 
        /// This api returns the real time quote data for stocks, ETFs, mutuals funds, and so on.
        /// Version, lang and region are set by default but you can change them.
        /// </summary>
        /// <param name="symbols"> The lists of symbols you want to get real time quote data.</param>
        /// <param name="version"> The api version, default is set to v6.</param>
        /// <param name="lang"> The language, default is set to EN.</param>
        /// <param name="region"> The region, default is set to us.</param>
        /// <returns cref="HttpResponseMessage"></returns>
        public async Task<HttpResponseMessage> GetFinanceQuoteRawAsync(IEnumerable<string> symbols, int version = 6, string lang = "EN", string region = "us")
        {
            if (symbols is null || !symbols.Any()) throw new ArgumentNullException(nameof(symbols));

            Dictionary<string, string> queryParams = new Dictionary<string, string>();
            queryParams.Add(Const.region, region);
            queryParams.Add(Const.lang, lang);
            string parsedSymbols = ParseSymbolsIntoString(symbols);
            queryParams.Add(Const.symbols, parsedSymbols);

            Uri uri = BuildUri(queryParams, version, Const.financeQuote);
            HttpRequestMessage request = BuildHttpGetRequest(uri);

            return await httpClient.SendAsync(request);
        }

        /// <summary>
        /// This method wraps the api https://yfapi.net/v6/finance/quote. 
        /// This api returns the real time quote data for stocks, ETFs, mutuals funds, and so on.
        /// Version, lang and region are set by default but you can change them.
        /// </summary>
        /// <param name="symbols"> The lists of symbols you want to get real time quote data.</param>
        /// <param name="version"> The api version, default is set to v6.</param>
        /// <param name="lang"> The language, default is set to EN.</param>
        /// <param name="region"> The region, default is set to us.</param>
        public async Task<IEnumerable<Dictionary<string, string>>> GetFinanceQuoteAsync(IEnumerable<string> symbols, int version = 6, string lang = "EN", string region = "us")
        {
            HttpResponseMessage response = await GetFinanceQuoteRawAsync(symbols, version, lang, region);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(content);
            JEnumerable<JObject> rawSecurities = data[Const.quoteResponse][Const.result].Children<JObject>();
            List<Dictionary<string, string>> securities = ParseSecurities(rawSecurities).ToList();

            return securities;
        }

        #endregion

        #region /Finance/Options API

        /// <summary>
        /// This method wraps the api https://yfapi.net/v6/finance/options. 
        /// This api returns the option chain for a particular symbol.
        /// Version and date are set by default but you can change them.
        /// </summary>
        /// <param name="symbol"> The symbol you want to get real time options data.</param>
        /// <param name="version"> The api version, default is set to v7.</param>
        /// <param name="date"></param>
        /// <returns cref="HttpResponseMessage"></returns>
        public async Task<HttpResponseMessage> GetFinanceOptionsRawAsync(string symbol, string dateTime = null, int version = 7)
        {
            if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException($"'{nameof(symbol)}' cannot be null or whitespace.", nameof(symbol));

            Dictionary<string, string> queryParams = new Dictionary<string, string>();

            if (!dateTime.IsNullOrWhiteSpace())
            {
                queryParams.Add(Const.date, dateTime);
            }

            string apiEndpoint = $"{Const.financeOptions}/{symbol}";
            Uri uri = BuildUri(queryParams, version, apiEndpoint);
            HttpRequestMessage request = BuildHttpGetRequest(uri);

            return await httpClient.SendAsync(request);
        }

        /// <summary>
        /// This method wraps the api https://yfapi.net/v6/finance/options. 
        /// This api returns the option chain for a particular symbol.
        /// Version and date are set by default but you can change them.
        /// </summary>
        /// <param name="symbol"> The symbol you want to get real time options data.</param>
        /// <param name="version"> The api version, default is set to v7.</param>
        /// <param name="dateTime"></param>
        public async Task<Dictionary<string, IEnumerable<Dictionary<string, string>>>> GetFinanceOptionsAsync(string symbol, string dateTime = null, int version = 7)
        {
            HttpResponseMessage response = await GetFinanceOptionsRawAsync(symbol, dateTime, version);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(content);
            JEnumerable<JObject> rawSecurities = data[Const.optionChain][Const.result].Children<JObject>();

            List<Dictionary<string, string>> securities = ParseSecurities(rawSecurities).ToList();

            var symbolSecurities = new Dictionary<string, IEnumerable<Dictionary<string, string>>>();
            symbolSecurities.Add(symbol, securities);

            return symbolSecurities;
        }

        #endregion

        #region Private utility methods

        private IEnumerable<Dictionary<string, string>> ParseSecurities(JEnumerable<JObject>? rawSecurities)
        {
            if (rawSecurities is null || !rawSecurities.Value.Any()) throw new ArgumentNullException(nameof(rawSecurities));

            List<Dictionary<string, string>> securities = new();
            foreach (JObject rawSecurity in rawSecurities)
            {
                var security = new Dictionary<string, string>();

                foreach (var property in rawSecurity.Properties())
                {
                    security.Add(property.Name, property.Value.ToString());
                }

                securities.Add(security);
            }

            return securities;
        }

        private string ParseSymbolsIntoString(IEnumerable<string> symbols)
        {
            var sb = new StringBuilder();

            foreach (string symbol in symbols)
            {
                sb.Append(symbol);
                sb.Append(',');
            }

            return sb.ToString();
        }

        private Uri BuildUri(Dictionary<string,string> queryParams, int apiVersion, string apiEndpoint)
        {
            string apiUrl = $"{baseUrl}/v{apiVersion}{apiEndpoint}";
            var builder = new UriBuilder(apiUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);

            foreach (KeyValuePair<string, string> param in queryParams)
            {
                query[param.Key] = param.Value;
            }
            
            builder.Query = query.ToString();

            return builder.Uri;
        }

        private HttpRequestMessage BuildHttpGetRequest(Uri uri)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = uri;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Const.applicationJson));
            request.Headers.Add(Const.apiKey, apiKey);

            return request;
        }

        #endregion

    }
}
