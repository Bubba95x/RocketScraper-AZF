using API.RocketStats.Dtos;
using ProcessPlayerStats.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketClient
    {
        Task<List<PlayerDto>> GetAllPlayersAsync();
        Task PostRocketStatsMatchAsync(Guid userId, RTMatchRequestDto requestDto);
    }
}
