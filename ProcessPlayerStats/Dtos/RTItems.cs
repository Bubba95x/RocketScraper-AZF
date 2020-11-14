using API.RocketStats.Dtos;
using System.Collections.Generic;

namespace ProcessPlayerStats.Dtos
{
    public class RTItems
    {
        //public string Metadata { get; set; }
        public List<RTMatchRequestDto> Matches { get; set; }
    }
}
