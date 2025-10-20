namespace Practica4.Models
{
    public class GameSession
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player CurrentPlayer => Player1.IsCurrentTurn ? Player1 : Player2;
        public Player OpponentPlayer => Player1.IsCurrentTurn ? Player2 : Player1;
        public GameState State { get; set; } = GameState.Setup;
        public string Winner { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public void SwitchTurns()
        {
            Player1.IsCurrentTurn = !Player1.IsCurrentTurn;
            Player2.IsCurrentTurn = !Player2.IsCurrentTurn;
        }
    }
}