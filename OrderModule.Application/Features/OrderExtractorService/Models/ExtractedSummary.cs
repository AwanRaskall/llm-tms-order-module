namespace OrderModule.Application.Features.OrderExtractorService.Models
{
    public class ExtractedSummary
    {
        /// <summary>
        /// Represents structured order data extracted by the LLM processing pipeline
        /// </summary>
        public string Invoice { get; set; } = string.Empty;
        public string DepDate { get; set; } = string.Empty;
        public string DepPoint { get; set; } = string.Empty;
        public string ArrDate { get; set; } = string.Empty;
        public string ArrPoint { get; set; } = string.Empty;
        public string Transport { get; set; } = string.Empty;
        public string Products { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
