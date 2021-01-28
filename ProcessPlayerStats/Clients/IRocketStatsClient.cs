using ProcessPlayerStats.Dtos.RTDtos;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketStatsClient
    {
        Task<RTSession> GetRecentRocketLeagueSessionsAsync(string userId, string platform);
    }
}
