using System;

namespace API.RocketStats.Dtos.RTDtos
{
    public class RTMatchRequestDto
    {
        public Guid ID { get; set; }
        public RTMetaDataDto Metadata { get; set; }
        public RTStatsDto Stats { get; set; }
    }
}
