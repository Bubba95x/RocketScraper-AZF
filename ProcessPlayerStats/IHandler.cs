using ProcessPlayerStats.Dtos.Response;
using System.Threading.Tasks;

namespace ProcessPlayerStats
{
    public interface IHandler
    {
        Task<string> ProcessEventAsync(PlayerResponseDto player);
    }
}
