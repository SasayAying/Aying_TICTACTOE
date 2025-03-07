using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Aying_TICTACTOE.Hubs;

namespace Aying_TICTACTOE.Pages
{
    public class IndexModel : PageModel
    {
        public string?[,] Board { get; set; } = new string[3, 3]; // 3x3 game board (nullable)
        public string CurrentPlayer { get; set; } = "X"; // Current player (X or O)
        public bool GameOver { get; set; } = false; // Indicates if the game is over

        public void OnGet()
        {
            // Initialize the game board when the page loads
            Board = new string[3, 3];
            CurrentPlayer = "X";
            GameOver = false;
        }

        public IActionResult OnPostMove(int row, int col)
        {
            if (!GameOver && string.IsNullOrEmpty(Board[row, col]))
            {
                // Update the board with the current player's move
                Board[row, col] = CurrentPlayer;

                // Notify all clients of the move
                var hubContext = HttpContext.RequestServices.GetService<IHubContext<GameHub>>();
                hubContext?.Clients.All.SendAsync("ReceiveMove", row, col, CurrentPlayer);

                // Check for a win
                if (CheckForWin(Board))
                {
                    GameOver = true;
                    hubContext?.Clients.All.SendAsync("ReceiveGameOver", CurrentPlayer);
                }
                else if (!IsBoardFull(Board))
                {
                    // AI's turn (if playing against AI)
                    var bestMove = GetBestMove(Board).Split(',');
                    int aiRow = int.Parse(bestMove[0]);
                    int aiCol = int.Parse(bestMove[1]);
                    Board[aiRow, aiCol] = "O";

                    // Notify all clients of the AI's move
                    hubContext?.Clients.All.SendAsync("ReceiveMove", aiRow, aiCol, "O");

                    // Check if the AI won
                    if (CheckForWin(Board))
                    {
                        GameOver = true;
                        hubContext?.Clients.All.SendAsync("ReceiveGameOver", "O");
                    }
                    else
                    {
                        CurrentPlayer = "X"; // Switch back to the human player
                    }
                }
            }

            // Refresh the page to update the UI
            return Page();
        }

        private bool CheckForWin(string?[,] board)
        {
            // Check rows for a win
            for (int i = 0; i < 3; i++)
            {
                if (!string.IsNullOrEmpty(board[i, 0]) &&
                    board[i, 0] == board[i, 1] &&
                    board[i, 1] == board[i, 2])
                {
                    return true; // Row win
                }
            }

            // Check columns for a win
            for (int j = 0; j < 3; j++)
            {
                if (!string.IsNullOrEmpty(board[0, j]) &&
                    board[0, j] == board[1, j] &&
                    board[1, j] == board[2, j])
                {
                    return true; // Column win
                }
            }

            // Check diagonals for a win
            if (!string.IsNullOrEmpty(board[0, 0]) &&
                board[0, 0] == board[1, 1] &&
                board[1, 1] == board[2, 2])
            {
                return true; // Diagonal win
            }
            if (!string.IsNullOrEmpty(board[0, 2]) &&
                board[0, 2] == board[1, 1] &&
                board[1, 1] == board[2, 0])
            {
                return true; // Diagonal win
            }

            // No winner
            return false;
        }

        private bool IsBoardFull(string?[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (string.IsNullOrEmpty(board[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private string GetBestMove(string?[,] board)
        {
            int bestScore = int.MinValue;
            int[] bestMove = { -1, -1 };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (string.IsNullOrEmpty(board[i, j]))
                    {
                        board[i, j] = "O"; // AI is "O"
                        int score = Minimax(board, 0, false);
                        board[i, j] = null;

                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = new int[] { i, j };
                        }
                    }
                }
            }

            return $"{bestMove[0]},{bestMove[1]}";
        }

        private int Minimax(string?[,] board, int depth, bool isMaximizing)
        {
            if (CheckForWin(board))
            {
                return isMaximizing ? -1 : 1;
            }
            else if (IsBoardFull(board))
            {
                return 0;
            }

            if (isMaximizing)
            {
                int bestScore = int.MinValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (string.IsNullOrEmpty(board[i, j]))
                        {
                            board[i, j] = "O";
                            int score = Minimax(board, depth + 1, false);
                            board[i, j] = null;
                            bestScore = Math.Max(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
            else
            {
                int bestScore = int.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (string.IsNullOrEmpty(board[i, j]))
                        {
                            board[i, j] = "X";
                            int score = Minimax(board, depth + 1, true);
                            board[i, j] = null;
                            bestScore = Math.Min(score, bestScore);
                        }
                    }
                }
                return bestScore;
            }
        }
    }
}