using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchResult
    {
        public List<SmartSearchKeyword> keywords { get; set; }
        public List<SmartSearchDateRange> dateRanges { get; set; }

        public SmartSearchResult() 
        {
            keywords = new List<SmartSearchKeyword>();
            dateRanges = new List<SmartSearchDateRange>();
        }
    }
}
