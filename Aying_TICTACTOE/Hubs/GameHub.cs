using Microsoft.AspNetCore.SignalR;

namespace Aying_TICTACTOE.Hubs
{
    public class GameHub : Hub
    {
        // Send a move to all clients
        public async Task SendMove(int row, int col, string player)
        {
            await Clients.All.SendAsync("ReceiveMove", row, col, player);
        }

        // Notify all clients when the game is over
        public async Task SendGameOver(string winner)
        {
            await Clients.All.SendAsync("ReceiveGameOver", winner);
        }
    }
}