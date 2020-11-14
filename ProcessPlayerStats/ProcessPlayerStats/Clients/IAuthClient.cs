using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IAuthClient
    {
        Task<string> ObtainAccessTokenAsync();
    }
}
