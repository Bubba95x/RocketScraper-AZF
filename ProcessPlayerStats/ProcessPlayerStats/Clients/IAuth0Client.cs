using System.Threading.Tasks;

namespace ProcessPlayerStats.Clients
{
    public interface IAuth0Client
    {
        Task<string> ObtainAccessTokenAsync();
    }
}
