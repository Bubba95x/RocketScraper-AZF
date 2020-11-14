using ProcessPlayerStats.Models;
using System.Threading.Tasks;

namespace ProcessPlayerStats
{
    public interface IHandler
    {
        Task<string> ProcessEventAsync(PlayerDto player);
    }
}
