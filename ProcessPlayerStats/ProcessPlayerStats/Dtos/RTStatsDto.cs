using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.RocketStats.Dtos
{
    public class RTStatsDto
    {
        public RTMatchStatsDto Saves { get; set; }
        public RTMatchStatsDto Assists { get; set; }
        public RTMatchStatsDto Goals { get; set; }
        public RTMatchStatsDto MatchesPlayed { get; set; }
        public RTMatchStatsDto Mvps { get; set; }
        public RTMatchStatsDto Shots { get; set; }
        public RTMatchStatsDto Wins { get; set; }

    }
}
