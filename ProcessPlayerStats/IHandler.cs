using System.Threading.Tasks;

namespace ProcessPlayerStats
{
    public interface IHandler
    {
        Task<string> ProcessEventAsync();
    }
}
