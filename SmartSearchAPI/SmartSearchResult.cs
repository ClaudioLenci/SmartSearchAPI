namespace SmartSearchAPI
{
    public class SmartSearchResult
    {
        public List<SmartSearchVerb> Verbs { get; set; }
        public List<SmartSearchKeyword> Keywords { get; set; }
        public List<SmartSearchDateRange> DateRanges { get; set; }

        public SmartSearchResult()
        {
            Verbs = new List<SmartSearchVerb>();
            Keywords = new List<SmartSearchKeyword>();
            DateRanges = new List<SmartSearchDateRange>();
        }
    }
}
