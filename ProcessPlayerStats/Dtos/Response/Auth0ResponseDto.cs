namespace ProcessPlayerStats.Dtos.Response
{
    public class Auth0ResponseDto
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }
}
