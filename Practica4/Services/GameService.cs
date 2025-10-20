using Practica4.Models;
using Microsoft.Maui.Storage;

namespace Practica4.Services
{
    public class GameService
    {
        private GameSession _currentSession;
        private readonly ISecureStorage _secureStorage;

        public GameSession CurrentSession => _currentSession;

        public event EventHandler<GameStateChangedEventArgs> GameStateChanged;
        public event EventHandler<ShotFiredEventArgs> ShotFired;

        public GameService()
        {
            _secureStorage = SecureStorage.Default;
        }

        public async Task StartNewGame(string player1Name, string player2Name)
        {
            // Store player names
            await _secureStorage.SetAsync("player1", player1Name);
            await _secureStorage.SetAsync("player2", player2Name);

            _currentSession = new GameSession
            {
                Player1 = new Player(player1Name) { IsCurrentTurn = true },
                Player2 = new Player(player2Name),
                State = GameState.Player1Placing,
                StartTime = DateTime.Now
            };

            InitializePlayerShips(_currentSession.Player1);
            InitializePlayerShips(_currentSession.Player2);

            OnGameStateChanged(GameState.Player1Placing);
        }

        private void InitializePlayerShips(Player player)
        {
            // Create all ships for the player
            var ships = new List<Ship>
            {
                new Ship { Type = ShipType.Carrier },
                new Ship { Type = ShipType.Battleship },
                new Ship { Type = ShipType.Submarine },
                new Ship { Type = ShipType.Cruiser },
                new Ship { Type = ShipType.Destroyer }
            };

            player.OwnBoard.Ships = ships;
        }

        public async Task<(string player1, string player2)> GetStoredPlayers()
        {
            var player1 = await _secureStorage.GetAsync("player1");
            var player2 = await _secureStorage.GetAsync("player2");
            return (player1, player2);
        }

        public bool PlaceShip(Ship ship, int row, int col, Player player)
        {
            var board = player.OwnBoard;
            
            // Remove ship from current position if already placed
            if (ship.IsPlaced)
            {
                board.RemoveShip(ship);
            }

            // Try to place at new position
            if (board.CanPlaceShip(ship, row, col))
            {
                board.PlaceShip(ship, row, col);
                return true;
            }

            return false;
        }

        public void RotateShip(Ship ship, Player player)
        {
            if (ship.IsPlaced)
            {
                var board = player.OwnBoard;
                var oldOrientation = ship.Orientation;
                var row = ship.Row;
                var col = ship.Column;
                
                // Remove ship
                board.RemoveShip(ship);
                
                // Change orientation
                ship.Orientation = ship.Orientation == ShipOrientation.Horizontal 
                    ? ShipOrientation.Vertical 
                    : ShipOrientation.Horizontal;
                
                // Try to place with new orientation
                if (!board.CanPlaceShip(ship, row, col))
                {
                    // Revert orientation if can't place
                    ship.Orientation = oldOrientation;
                    board.PlaceShip(ship, row, col);
                }
                else
                {
                    board.PlaceShip(ship, row, col);
                }
            }
            else
            {
                // Just toggle orientation for unplaced ships
                ship.Orientation = ship.Orientation == ShipOrientation.Horizontal 
                    ? ShipOrientation.Vertical 
                    : ShipOrientation.Horizontal;
            }
        }

        public void ConfirmShipPlacement()
        {
            if (_currentSession.State == GameState.Player1Placing)
            {
                if (_currentSession.Player1.OwnBoard.AllShipsPlaced())
                {
                    _currentSession.State = GameState.Player2Placing;
                    OnGameStateChanged(GameState.Player2Placing);
                }
            }
            else if (_currentSession.State == GameState.Player2Placing)
            {
                if (_currentSession.Player2.OwnBoard.AllShipsPlaced())
                {
                    _currentSession.State = GameState.Playing;
                    _currentSession.Player1.IsCurrentTurn = true;
                    _currentSession.Player2.IsCurrentTurn = false;
                    OnGameStateChanged(GameState.Playing);
                }
            }
        }

        public ShotResult FireShot(int row, int col)
        {
            if (_currentSession.State != GameState.Playing)
                return new ShotResult { Success = false };

            var attacker = _currentSession.CurrentPlayer;
            var defender = _currentSession.OpponentPlayer;
            
            // Check if cell was already hit
            if (attacker.OpponentBoard.Grid[row, col] != CellState.Empty)
                return new ShotResult { Success = false, Message = "Already fired at this location!" };

            var result = new ShotResult
            {
                Success = true,
                Row = row,
                Column = col,
                Attacker = attacker.Name,
                Defender = defender.Name
            };

            // Check actual board for hit/miss
            if (defender.OwnBoard.Grid[row, col] == CellState.Ship)
            {
                result.IsHit = true;
                attacker.OpponentBoard.Grid[row, col] = CellState.Hit;
                defender.OwnBoard.Grid[row, col] = CellState.Hit;
                attacker.OpponentBoard.Hits++;

                // Find which ship was hit
                foreach (var ship in defender.OwnBoard.Ships)
                {
                    if (IsShipHit(ship, row, col))
                    {
                        ship.Hits++;
                        result.ShipHit = ship.Name;
                        if (ship.IsSunk)
                        {
                            result.ShipSunk = true;
                            result.Message = $"{ship.Name} sunk!";
                        }
                        break;
                    }
                }

                // Check for win
                if (defender.OwnBoard.AllShipsSunk())
                {
                    _currentSession.Winner = attacker.Name;
                    _currentSession.State = GameState.GameOver;
                    _currentSession.EndTime = DateTime.Now;
                    result.GameOver = true;
                    result.Winner = attacker.Name;
                    OnGameStateChanged(GameState.GameOver);
                }
            }
            else
            {
                result.IsHit = false;
                attacker.OpponentBoard.Grid[row, col] = CellState.Miss;
                defender.OwnBoard.Grid[row, col] = CellState.Miss;
                attacker.OpponentBoard.Misses++;
            }

            attacker.OpponentBoard.TotalShots++;

            // Switch turns if not game over and it was a miss
            if (!result.GameOver && !result.IsHit)
            {
                _currentSession.SwitchTurns();
            }

            OnShotFired(result);
            return result;
        }

        private bool IsShipHit(Ship ship, int row, int col)
        {
            if (!ship.IsPlaced) return false;

            int endRow = ship.Orientation == ShipOrientation.Vertical 
                ? ship.Row + ship.Size - 1 : ship.Row;
            int endCol = ship.Orientation == ShipOrientation.Horizontal 
                ? ship.Column + ship.Size - 1 : ship.Column;

            return row >= ship.Row && row <= endRow && col >= ship.Column && col <= endCol;
        }

        protected virtual void OnGameStateChanged(GameState newState)
        {
            GameStateChanged?.Invoke(this, new GameStateChangedEventArgs { NewState = newState });
        }

        protected virtual void OnShotFired(ShotResult result)
        {
            ShotFired?.Invoke(this, new ShotFiredEventArgs { Result = result });
        }
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public GameState NewState { get; set; }
    }

    public class ShotFiredEventArgs : EventArgs
    {
        public ShotResult Result { get; set; }
    }

    public class ShotResult
    {
        public bool Success { get; set; }
        public bool IsHit { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string ShipHit { get; set; }
        public bool ShipSunk { get; set; }
        public bool GameOver { get; set; }
        public string Winner { get; set; }
        public string Message { get; set; }
        public string Attacker { get; set; }
        public string Defender { get; set; }
    }
}