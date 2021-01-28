using ProcessPlayerStats.Dtos.Request;
using ProcessPlayerStats.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketClient
    {
        Task<MatchResponseDto> AddMatchAsync(MatchRequestDto match);
        Task<MatchResponseDto> GetMatchAsync(Guid ID);
        Task<PlayerMatchResponseDto> GetPlayerMatchAsync(Guid userId, Guid matchId);
        Task<PlayerMatchResponseDto> AddPlayerMatchAsync(PlayerMatchRequestDto matchStatistic);
        Task<List<PlayerResponseDto>> GetAllPlayersAsync();
        Task AddPlayerMatchStatisticAsync(PlayerMatchStatisticRequestDto matchStatistic);
        Task<PlayerMatchResponseDto> GetPlayerMatchByRocketIdAsync(Guid rocketStatsID);
    }
}
