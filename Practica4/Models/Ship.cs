namespace Practica4.Models
{
    public class Ship
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public ShipType Type { get; set; }
        public int Size => (int)Type;
        public ShipOrientation Orientation { get; set; } = ShipOrientation.Horizontal;
        public int Row { get; set; } = -1;
        public int Column { get; set; } = -1;
        public bool IsPlaced { get; set; }
        public int Hits { get; set; }
        public bool IsSunk => Hits >= Size;
        
        public string Name => Type switch
        {
            ShipType.Carrier => "Carrier",
            ShipType.Battleship => "Battleship",
            ShipType.Submarine => "Submarine",
            ShipType.Cruiser => "Cruiser",
            ShipType.Destroyer => "Destroyer",
            _ => "Unknown"
        };

        public string Icon => Type switch
        {
            ShipType.Carrier => "🚢",
            ShipType.Battleship => "⛴️",
            ShipType.Submarine => "🚤",
            ShipType.Cruiser => "🛥️",
            ShipType.Destroyer => "⛵",
            _ => "🚢"
        };
    }
}