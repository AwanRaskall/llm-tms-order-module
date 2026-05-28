

namespace OrderModule.Application.Interfaces
{
    /// <summary>
    /// Common contract for raw responses from LLM APIs.
    /// Allows uniform content extraction regardless of provider format.
    /// </summary>
    public interface IModelResponse
    {
        string GetContent();
    }
}