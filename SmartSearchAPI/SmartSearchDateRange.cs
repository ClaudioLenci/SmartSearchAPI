using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchDateRange
    {
        public DateTime datemin { get; set; }
        public DateTime datemax { get; set; }
        public bool include { get; set; }

        public SmartSearchDateRange(DateTime datemin, DateTime datemax)
        {
            this.datemin = datemin;
            this.datemax = datemax;
            include = true;
        }

        public SmartSearchDateRange()
        {
            this.datemin = DateTime.MinValue;
            this.datemax = DateTime.MaxValue;
            include = true;
        }

        public override string ToString()
        {
            return datemin.ToString() + " - " + datemax.ToString();
        }
    }
}
