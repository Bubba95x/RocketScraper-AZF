using ProcessPlayerStats.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IRocketClient
    {
        Task<List<PlayerModel>> GetAllPlayersAsync();
    }
}
