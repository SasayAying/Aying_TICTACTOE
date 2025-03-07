namespace Aying_TICTACTOE.Models
{
    public class GameResult
    {
        public int Id { get; set; }
        public required string PlayerName { get; set; }
        public required string Result { get; set; } // Win, Lose, Draw
    }
}