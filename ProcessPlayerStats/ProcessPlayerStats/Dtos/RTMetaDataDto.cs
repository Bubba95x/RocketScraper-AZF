using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.RocketStats.Dtos
{
    public class RTMetaDataDto
    {
        public string Result { get; set; }
        public string Playlist { get; set; }
        public DateTime DateCollected { get; set; }
    }
}
