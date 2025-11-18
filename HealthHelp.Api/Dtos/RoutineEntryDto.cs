namespace HealthHelp.Api.Dtos
{
    public class RoutineEntryDto
    {
        public int Id { get; set; }
        public string Category { get; set; } 
        public string? Description { get; set; } 
        public decimal Hours { get; set; } 
        public DateOnly EntryDate { get; set; } 
        
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}