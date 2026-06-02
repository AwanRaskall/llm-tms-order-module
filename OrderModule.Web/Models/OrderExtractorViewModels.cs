namespace OrderModule.Web.Models
{
    /// <summary>
    /// ViewModel for Index (OrderExtractor) page.
    /// Contains 8 fields extracted from email file by LLM.
    /// User can edit fields before saving as draft.
    /// </summary>
    public class ViewModelSummary
    {
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