using System;

namespace ProcessPlayerStats.Dtos.Request
{
    public class PlayerMatchRequestDto
    {
        public Guid PlayerID { get; set; }
        public Guid? MatchID { get; set; }
        public string Victory { get; set; }
        public Guid RocketStatsID { get; set; }
        public string RocketStatsGameMode { get; set; }
        public DateTime RocketStatsMatchDate { get; set; }
    }
}
