using System;

namespace ProcessPlayerStats.Dtos.Request
{
    public class MatchRequestDto
    {
        public string GameMode { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
