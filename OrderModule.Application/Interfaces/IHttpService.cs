using System.Threading.Tasks;

namespace OrderModule.Application.Interfaces
{
    /// <summary>
    /// Common contract for HTTP services
    /// </summary>
    public interface IHttpService
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(string url, TRequest data);
        Task<TResponse> GetAsync<TResponse>(string url);
    }

    /// <summary>
    /// Marker interface for HTTP communication with local Ollama service.
    /// </summary>
    public interface IHttpServiceOllama : IHttpService { }

    /// <summary>
    /// Marker interface for HTTP communication with OpenRouter cloud API.
    /// </summary>
    public interface IHttpServiceOpenRouter : IHttpService { }
}