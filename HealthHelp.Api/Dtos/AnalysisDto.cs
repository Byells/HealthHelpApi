namespace HealthHelp.Api.Dtos
{
    public class AnalysisDto
    {
        public double Score { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<string> Tips { get; set; } = new List<string>();
        public DateOnly AnalysisDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public Dictionary<string, double> DomainScores { get; set; } = new Dictionary<string, double>();
    }
}