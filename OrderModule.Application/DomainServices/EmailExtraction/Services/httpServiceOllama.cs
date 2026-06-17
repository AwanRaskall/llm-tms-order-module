using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderModule.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace OrderModule.Application.DomainServices.EmailExtraction.Services
{
    /// <summary>
    /// HTTP client for local Ollama instance.
    /// Handles connection setup, retry logic and request/response lifecycle
    /// BaseAddress is read from configuration: Ollama:BaseUrl
    /// </summary>
    public class HttpServiceOllama : IHttpServiceOllama
    {
        private readonly ILogger<HttpServiceOllama> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _maxRetries;

        private HttpClient? _client;
        private readonly object _lock = new();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpServiceOllama(
            ILogger<HttpServiceOllama> logger,
            IConfiguration configuration,
            int maxRetries = 3)
        {
            _logger = logger;
            _configuration = configuration;
            _maxRetries = maxRetries;
        }

        public HttpClient GetClient()
        {
            if (_client != null)
                return _client;

            lock (_lock)
            {
                if (_client == null)
                {
                    // http://localhost:11434 - classic link for local Ollama
                    var baseUrl = _configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";

                    _client = new HttpClient
                    {
                        BaseAddress = new Uri(baseUrl.TrimEnd('/') + '/'),
                        Timeout = TimeSpan.FromMinutes(5)
                    };
                }
            }

            return _client;
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(string url, TRequest parameters)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogWarning("URL cannot be empty");
            }

            string json;
            try
            {
                json = JsonSerializer.Serialize(parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize data for POST {Url}", url);
                throw;
            }

            HttpResponseMessage? response = null;

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                // Create content and request inside a loop - HttpRequestMessage cannot be reused once sent
                // Creating HTTP content with JSON and UTF-8 encoding
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                // Implement HTTP-request SendAsync
                using var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                try
                {
                    _logger.LogInformation("Ollama POST {Url} attempt {Attempt}", url, attempt);

                    response = await GetClient().SendAsync(request).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Ollama POST {Url} returned {StatusCode} on attempt {Attempt}", url, response.StatusCode, attempt);

                    }

                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var result = JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions);
                    _logger.LogInformation("Ollama POST {Url} successful on attempt {Attempt}", url, attempt);

                    return result!;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "HTTP error on Ollama POST {Url} (attempt {Attempt})", url, attempt);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "Ollama POST {Url} timed out (attempt {Attempt})", url, attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ollama POST {Url} failed", url);
                    throw;
                }
                finally
                {
                    response?.Dispose();
                }
            }

            _logger.LogWarning("Ollama POST {Url} failed after {MaxRetries} attempts", url, _maxRetries);
            return default!;
        }

        public async Task<TResponse> GetAsync<TResponse>(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be empty.", nameof(url));

            HttpResponseMessage? response = null;

            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation("Ollama GET {Url} attempt {Attempt}", url, attempt);

                    response = await GetClient().GetAsync(url).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        _logger.LogWarning("Ollama GET {Url} failed on attempt {Attempt}. Status: {StatusCode}", url, attempt, (int)response.StatusCode);

                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    return JsonSerializer.Deserialize<TResponse>(responseBody, JsonOptions)!;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning(ex, "Ollama GET {Url} failed on attempt {Attempt}", url, attempt);
                }
                catch (TaskCanceledException ex)
                {
                    _logger.LogWarning(ex, "Ollama GET {Url} timeout on attempt {Attempt}", url, attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ollama GET {Url} failed", url);
                }
                finally
                {
                    response?.Dispose();
                }
            }

            _logger.LogWarning("Ollama GET {Url} failed after {MaxRetries} attempts", url, _maxRetries);
            return default!;
        }
    }
}