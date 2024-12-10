using BattlEye.V7;
using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CrazyDayZ.Shop.Services
{
    public class MonitoringService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationTokenSource _cancellationTokenSource;

        public MonitoringService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _ = Task.Run(async () => await MonitorServersAsync(_cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        private async Task MonitorServersAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UpdateServersInfo();
                await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
            }
        }

        public void UpdateServersInfo()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                List<ServerData> servers = context.Servers.ToList();
                List<Server> serverList = new List<Server>();

                foreach (var serverData in servers)
                {
                    try
                    {
                        var client = BEClient.New(serverData.IP, serverData.Port, serverData.Password);
                        var players = client.GetPlayers();
                        int currentPlayers = players.Count;
                        int maxPlayers = int.Parse(serverData.MaxSlots);
                        int onlinePlayersPercentage = (int)(currentPlayers / (float)maxPlayers * 100);
                        Server server = new Server
                        {
                            Id = serverData.Id,
                            Name = serverData.Name,
                            Connect = $"{serverData.IP}:{serverData.Port}",
                            CurrentPlayers = currentPlayers,
                            MaxPlayers = maxPlayers,
                            OnlinePlayersPercentage = onlinePlayersPercentage
                        };

                        serverList.Add(server);

                        client.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при подключении к серверу {serverData.Name}: {ex.Message}");
                    }
                }
                ServerInfo.Servers = serverList;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}
