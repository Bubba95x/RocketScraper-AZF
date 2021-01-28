using System;

namespace ProcessPlayerStats.Dtos.Response
{
    public class PlayerResponseDto : BaseResponseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PlatformName { get; set; }
        public string AvatarUrl { get; set; }
        public string RocketStatsID { get; set; }
    }
}
