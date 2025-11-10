namespace Practica4.Models
{
    public enum CellState
    {
        Empty,
        Ship,
        Miss,
        Hit
    }

    public enum ShipType
    {
        Carrier = 5,      // 5 cells
        Battleship = 4,   // 4 cells
        Submarine = 3,    // 3 cells
        Cruiser = 1,      // 3 cells (another 3-cell ship)
        Destroyer = 2     // 2 cells
    }

    public enum ShipOrientation
    {
        Horizontal,
        Vertical
    }

    public enum GameState
    {
        Setup,
        Player1Placing,
        Player2Placing,
        Playing,
        GameOver
    }
}