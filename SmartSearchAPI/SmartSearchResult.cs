﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchResult
    {
        public List<SmartSearchKeyword> Keywords { get; set; }
        public List<SmartSearchDateRange> DateRanges { get; set; }

        public SmartSearchResult() 
        {
            Keywords = new List<SmartSearchKeyword>();
            DateRanges = new List<SmartSearchDateRange>();
        }
    }
}
