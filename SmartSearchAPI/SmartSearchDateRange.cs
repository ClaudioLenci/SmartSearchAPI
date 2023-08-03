namespace SmartSearchAPI
{
    public class SmartSearchDateRange
    {
        public DateTime DateMin { get; set; }
        public DateTime DateMax { get; set; }
        public bool Include { get; set; }

        public SmartSearchDateRange(DateTime _DateMin, DateTime _DateMax)
        {
            this.DateMin = _DateMin;
            this.DateMax = _DateMax;
            Include = true;
        }

        public SmartSearchDateRange()
        {
            this.DateMin = DateTime.MinValue;
            this.DateMax = DateTime.MaxValue;
            Include = true;
        }

        public override string ToString()
        {
            return DateMin.ToString() + " - " + DateMax.ToString();
        }
    }
}
