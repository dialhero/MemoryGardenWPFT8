namespace MemoryGardenWPF.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Emoji { get; set; } = "";
        public bool IsMatched { get; set; }
    }
}
