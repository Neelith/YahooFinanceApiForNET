using System.Net.Http.Headers;

namespace YahooFinanceApiForNET.Services
{
    public class HttpRequestBuilder : IHttpRequestBuilder
    {
        private HttpRequestMessage httpRequestMessage;

        public HttpRequestBuilder Create()
        {
            this.httpRequestMessage = new HttpRequestMessage();
            return this;
        }

        public HttpRequestMessage Build()
        {
            return this.httpRequestMessage;
        }

        public HttpRequestBuilder WithHttpMethod(HttpMethod httpMethod)
        {
            this.httpRequestMessage.Method = httpMethod;
            return this;
        }

        public HttpRequestBuilder WithRequestUri(Uri uri)
        {
            this.httpRequestMessage.RequestUri = uri;
            return this;
        }

        public HttpRequestBuilder WithHeaders(string name, IEnumerable<string> values)
        {
            this.httpRequestMessage.Headers.Add(name, values);
            return this;
        }

        public HttpRequestBuilder WithHeaders(string name, string value)
        {
            this.httpRequestMessage.Headers.Add(name, value);
            return this;
        }

        public HttpRequestBuilder WithHeadersAccept(MediaTypeWithQualityHeaderValue mediaType)
        {
            this.httpRequestMessage.Headers.Accept.Add(mediaType);
            return this;
        }
    }
}
