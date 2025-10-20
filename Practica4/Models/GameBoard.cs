namespace Practica4.Models
{
    public class GameBoard
    {
        public const int BoardSize = 10;
        public CellState[,] Grid { get; set; }
        public List<Ship> Ships { get; set; }
        public int TotalShots { get; set; }
        public int Hits { get; set; }
        public int Misses { get; set; }

        public GameBoard()
        {
            Grid = new CellState[BoardSize, BoardSize];
            Ships = new List<Ship>();
            InitializeBoard();
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    Grid[i, j] = CellState.Empty;
                }
            }
        }

        public bool CanPlaceShip(Ship ship, int row, int col)
        {
            if (row < 0 || col < 0) return false;

            int endRow = ship.Orientation == ShipOrientation.Vertical ? row + ship.Size - 1 : row;
            int endCol = ship.Orientation == ShipOrientation.Horizontal ? col + ship.Size - 1 : col;

            // Check bounds
            if (endRow >= BoardSize || endCol >= BoardSize) return false;

            // Check for overlaps with existing ships
            for (int r = row; r <= endRow; r++)
            {
                for (int c = col; c <= endCol; c++)
                {
                    if (Grid[r, c] == CellState.Ship) return false;
                }
            }

            // Check for adjacent ships (ships cannot touch)
            for (int r = Math.Max(0, row - 1); r <= Math.Min(BoardSize - 1, endRow + 1); r++)
            {
                for (int c = Math.Max(0, col - 1); c <= Math.Min(BoardSize - 1, endCol + 1); c++)
                {
                    if (Grid[r, c] == CellState.Ship) return false;
                }
            }

            return true;
        }

        public void PlaceShip(Ship ship, int row, int col)
        {
            ship.Row = row;
            ship.Column = col;
            ship.IsPlaced = true;

            int endRow = ship.Orientation == ShipOrientation.Vertical ? row + ship.Size - 1 : row;
            int endCol = ship.Orientation == ShipOrientation.Horizontal ? col + ship.Size - 1 : col;

            for (int r = row; r <= endRow; r++)
            {
                for (int c = col; c <= endCol; c++)
                {
                    Grid[r, c] = CellState.Ship;
                }
            }

            if (!Ships.Contains(ship))
            {
                Ships.Add(ship);
            }
        }

        public void RemoveShip(Ship ship)
        {
            if (!ship.IsPlaced) return;

            int endRow = ship.Orientation == ShipOrientation.Vertical ? ship.Row + ship.Size - 1 : ship.Row;
            int endCol = ship.Orientation == ShipOrientation.Horizontal ? ship.Column + ship.Size - 1 : ship.Column;

            for (int r = ship.Row; r <= endRow; r++)
            {
                for (int c = ship.Column; c <= endCol; c++)
                {
                    Grid[r, c] = CellState.Empty;
                }
            }

            ship.IsPlaced = false;
            ship.Row = -1;
            ship.Column = -1;
        }

        public bool AllShipsPlaced()
        {
            return Ships.Count == 5 && Ships.All(s => s.IsPlaced);
        }

        public bool AllShipsSunk()
        {
            return Ships.All(s => s.IsSunk);
        }
    }
}
