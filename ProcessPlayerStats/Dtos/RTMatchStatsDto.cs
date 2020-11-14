using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.RocketStats.Dtos
{
    public class RTMatchStatsDto
    {
        public string Rank { get; set; }
        public double? Percentile { get; set; }
        public string DisplayName { get; set; }
        public string DisplayCategory { get; set; }
        public string Category { get; set; }
        //public List<object> MetaData { get; set; }
        public int Value { get; set; }
        public string DisplayValue { get; set; }
        public string DisplayType { get; set; }
    }
}
