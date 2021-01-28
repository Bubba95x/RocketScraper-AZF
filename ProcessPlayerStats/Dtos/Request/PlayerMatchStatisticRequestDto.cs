using System;

namespace ProcessPlayerStats.Dtos.Request
{
    public class PlayerMatchStatisticRequestDto
    {
        public Guid PlayerMatchId { get; set; }
        public string StatType { get; set; }
        public int Value { get; set; }
    }
}
