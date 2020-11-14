using System;

namespace ProcessPlayerStats.Models
{
    public class PlayerModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PlatformName { get; set; }
        public string AvatarUrl { get; set; }
        public string RocketStatsID { get; set; }
    }
}
