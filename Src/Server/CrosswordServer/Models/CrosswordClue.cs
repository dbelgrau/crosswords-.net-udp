namespace CrosswordServer.Models
{
    public class CrosswordClue
    {
        public string Word { get; set; }
        public Coordinates StartPoint { get; set; }
        public bool Horizontal { get; set; }
        public string Description { get; set; }
    }

    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
