namespace FitnessApi.DataLayer.Entities
{
    public class SugarLog
    {
        public int Id { get; set; }                // PK for EF
        public string? UserId { get; set; }        // Owner of the sugar log
        public DateTime Date { get; set; }         // EF supports DateTime

        // Owned types for slots
        public SugarLogSlot? Fasting { get; set; }
        public SugarLogSlot? AfterBreakfast { get; set; }
        public SugarLogSlot? BeforeLunch { get; set; }
        public SugarLogSlot? AfterLunch2hrs { get; set; }
        public SugarLogSlot? BeforeDinner { get; set; }
        public SugarLogSlot? AfterDinner2hrs { get; set; }
        public SugarLogSlot? Between2am3am { get; set; }
    }

    // Owned type — no PK required
    public class SugarLogSlot
    {
        public int SugarLevel { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
