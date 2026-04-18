namespace FitnessApi.Models
{
    public class SugarLogDto
    {
        public string? Id { get; set; }
        public DateOnly Date { get; set; }

        public SugarLogslotDto? Fasting { get; set; }
        public SugarLogslotDto? AfterBreakfast { get; set; }
        public SugarLogslotDto? BeforeLunch { get; set; }
        public SugarLogslotDto? AfterLunch2hrs { get; set; }
        public SugarLogslotDto? BeforeDinner { get; set; }
        public SugarLogslotDto? AfterDinner2hrs { get; set; }
        public SugarLogslotDto? Between2am3am { get; set; }

    }

    public class SugarLogslotDto
    {
        public int SugarLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
