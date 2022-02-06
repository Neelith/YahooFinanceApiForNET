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
        /// <returns>an http response</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<HttpResponseMessage> GetFinanceQuoteRawAsync(IEnumerable<string> symbols, int version = 6, string lang = "EN", string region = "us")
        {
            if (symbols is null || !symbols.Any()) throw new ArgumentNullException(nameof(symbols));

            Uri uri = BuildFinanceQuoteApiUri(symbols, version, lang, region);
            HttpRequestMessage request = BuildFinanceQuoteApiRequest(uri);

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
        /// <returns>an IEnumerable of dictionaries. One dictionary per security.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<List<Dictionary<string, string>>> GetFinanceQuoteAsync(IEnumerable<string> symbols, int version = 6, string lang = "EN", string region = "us")
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

        private HttpRequestMessage BuildFinanceQuoteApiRequest(Uri uri)
        {
            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = uri;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(Const.ApplicationJson));
            request.Headers.Add(Const.ApiKey, apiKey);

            return request;
        }

        private Uri BuildFinanceQuoteApiUri(IEnumerable<string> symbols, int version, string lang, string region)
        {
            string apiUrl = $"{baseUrl}/v{version}{Const.financeQuote}";
            var builder = new UriBuilder(apiUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            query[Const.region] = region;
            query[Const.lang] = lang;
            query[Const.symbols] = ParseSymbolsIntoString(symbols);
            builder.Query = query.ToString();

            return builder.Uri;
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

        #endregion

    }
}
