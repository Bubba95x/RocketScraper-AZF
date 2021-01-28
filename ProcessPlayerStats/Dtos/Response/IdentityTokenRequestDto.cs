namespace ProcessPlayerStats.Dtos.Response
{
    public class IdentityTokenRequestDto
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string scope { get; set; }

    }
}
