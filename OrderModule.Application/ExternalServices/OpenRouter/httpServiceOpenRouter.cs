using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderModule.Application.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderModule.Application.ExternalServices.OpenRouter
{
    /// <summary>
    /// HTTP client for OpenRouter API.
    /// Handles connection, authentication, retry logic and request/response lifecycle.
    /// </summary>
    public class HttpServiceOpenRouter : IHttpServiceOpenRouter
    {
        private readonly ILogger<HttpServiceOpenRouter> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _maxRetries;

        private HttpClient? _client;
        private readonly object _lock = new object();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        public HttpServiceOpenRouter(
            ILogger<HttpServiceOpenRouter> logger,
            IConfiguration configuration,
            int maxRetries = 3)
        {
            _logger        = logger;
            _configuration = configuration;
            _maxRetries    = maxRetries;
        }

        public HttpClient GetClient()
        {
            if (_client != null)
                return _client;

            lock (_lock)
            {
                if (_client == null)
                {
                    var apiKey = _configuration["OpenRouter:ApiKey"];
                    if (string.IsNullOrWhiteSpace(apiKey))
                        throw new InvalidOperationException(
                            "OpenRouter:ApiKey is missing in configuration.");

                    _client = new HttpClient
                    {
                        BaseAddress = new Uri("https://openrouter.ai/api/v1/"),
                        Timeout = TimeSpan.FromMinutes(5)
                    };

                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                    if (_client.DefaultRequestHeaders.Authorization == null)
                    {
                        Console.WriteLine("Authorization header is NOT set");
                    }
                    else
                    {
                        Console.WriteLine("Authorization header IS set");
                        Console.WriteLine("Scheme: " + _client.DefaultRequestHeaders.Authorization.Scheme);
                        Console.WriteLine("Has parameter: " +
                            (!string.IsNullOrEmpty(_client.DefaultRequestHeaders.Authorization.Parameter)));
                    }

                    // Optional attribution
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("HTTP-Referer", "http://localhost");
                    _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Title", "LogiFlow Order Extractor");
                }
            }

            return _client;
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(
            string url,
            TRequest parameters)
        {
            if (string.IsNullOrWhiteSpace(url))
                _logger.LogWarning("URL cannot be empty");

            string json;
            try
            {
                json = JsonSerializer.Serialize(parameters, JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize request for POST {Url}", url);
                throw;
            }

            HttpResponseMessage? response = null;

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                // Сreate a new request for each attempt
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                request.Headers.Authorization = GetClient().DefaultRequestHeaders.Authorization;

                try
                {
                    _logger.LogInformation("OpenRouter POST {Url} attempt {Attempt}", url, attempt);

                    response = await GetClient().SendAsync(request).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        _logger.LogWarning(
                            "OpenRouter POST {Url} returned {StatusCode} on attempt {Attempt}: {Error}",
                            url, 
                            response.StatusCode, 
                            attempt, 
                            errorBody);
                    }

                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // Console.WriteLine("-- OpenRouter raw response --");
                    // Console.WriteLine(responseBody);
                    // Console.WriteLine("-- End raw response --");
                                        
                    return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions)!;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "HTTP error on OpenRouter POST {Url} (attempt {Attempt})", url, attempt);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "OpenRouter POST {Url} timed out (attempt {Attempt})", url, attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OpenRouter POST {Url} failed with exception", url);
                    throw;
                }
                finally
                {
                    response?.Dispose();
                }
            }

            _logger.LogWarning("OpenRouter POST {Url} failed after {MaxRetries} attempts", url, _maxRetries);
            return default!;
        }

        public Task<TResponse> GetAsync<TResponse>(string url)
        {
            // GET is not required for OpenRouter
            throw new NotImplementedException("GET is not required for OpenRouter");
        }
    }
}