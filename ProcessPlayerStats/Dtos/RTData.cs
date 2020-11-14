using System;
using System.Collections.Generic;

namespace ProcessPlayerStats.Dtos
{
    public class RTData
    {
        public DateTime ExpiryDate { get; set; }
        public List<RTItems> Items { get; set; }
    }
}
