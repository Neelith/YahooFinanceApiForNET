using System.Net.Http.Headers;

namespace YahooFinanceApiForNET.Services
{
    public interface IHttpRequestBuilder
    {
        public HttpRequestBuilder Create();

        public HttpRequestMessage Build();

        public HttpRequestBuilder WithHttpMethod(HttpMethod httpMethod);

        public HttpRequestBuilder WithRequestUri(Uri uri);

        public HttpRequestBuilder WithHeaders(string name, IEnumerable<string> values);

        public HttpRequestBuilder WithHeaders(string name, string value);

        public HttpRequestBuilder WithHeadersAccept(MediaTypeWithQualityHeaderValue mediaType);
    }
}