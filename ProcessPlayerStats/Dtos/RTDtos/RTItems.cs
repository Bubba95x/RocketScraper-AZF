using API.RocketStats.Dtos.RTDtos;
using System.Collections.Generic;

namespace ProcessPlayerStats.Dtos.RTDtos
{
    public class RTItems
    {
        //public string Metadata { get; set; }
        public List<RTMatchRequestDto> Matches { get; set; }
    }
}
