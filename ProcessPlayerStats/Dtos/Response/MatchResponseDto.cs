using System;

namespace ProcessPlayerStats.Dtos.Response
{
    public class MatchResponseDto : BaseResponseDto
    {
        public Guid ID { get; set; }
        public string GameMode { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
