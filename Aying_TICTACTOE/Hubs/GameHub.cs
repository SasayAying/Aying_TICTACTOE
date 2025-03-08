using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task MakeMove(int cellIndex, string player)
    {
        await Clients.All.SendAsync("ReceiveMove", cellIndex, player);
    }
}