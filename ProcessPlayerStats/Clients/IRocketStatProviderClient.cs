using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketStatProviderClient
    {
        Task GetRecentRocketLeagueSessionsAsync(string userId, string platform);
    }
}
