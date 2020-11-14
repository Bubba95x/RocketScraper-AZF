using System;

namespace API.RocketStats.Dtos
{
    public class RTMatchRequestDto
    {
        public Guid ID { get; set; }
        public RTMetaDataDto Metadata { get; set; }
        public RTStatsDto Stats { get; set; }
    }
}
