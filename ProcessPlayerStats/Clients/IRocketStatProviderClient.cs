using ProcessPlayerStats.Dtos;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketStatProviderClient
    {
        Task<RTSession> GetRecentRocketLeagueSessionsAsync(string userId, string platform);
    }
}
