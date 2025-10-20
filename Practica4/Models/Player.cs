namespace Practica4.Models
{
    public class Player
    {
        public string Name { get; set; }
        public GameBoard OwnBoard { get; set; }
        public GameBoard OpponentBoard { get; set; } // Tracking opponent's board
        public bool IsCurrentTurn { get; set; }

        public Player(string name)
        {
            Name = name;
            OwnBoard = new GameBoard();
            OpponentBoard = new GameBoard();
        }
    }
}