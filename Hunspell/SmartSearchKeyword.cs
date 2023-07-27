using NHunspell;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSearchAPI
{
    public class SmartSearchKeyword
    {
        private string _keyword;
        public string Keyword
        {
            get
            {
                return _keyword;
            }
            set
            {
                _keyword = value;
                if(value != "")
                    GetSynonyms();
            }
        }
        public List<string> Synonyms { get; }

        public SmartSearchKeyword(string keyword)
        {
            Synonyms = new List<string>();
            Keyword = keyword;
        }

        public SmartSearchKeyword()
        {
            Synonyms = new List<string>();
            Keyword = "";
        }

        //https://www.codeproject.com/Articles/43495/Spell-Check-Hyphenation-and-Thesaurus-for-NET-with
        public void GetSynonyms()
        {
            MyThes thes = new MyThes(@"C:\Users\lenci\OneDrive - Istituto Istruzione Superiore Volterra Elia\Iride\SmartSearchAPI\SmartSearchAPI\th_it_IT.dat");
            Hunspell hunspell = new Hunspell(@"C:\Users\lenci\OneDrive - Istituto Istruzione Superiore Volterra Elia\Iride\SmartSearchAPI\SmartSearchAPI\elastic_hunspell_master_dicts_it_IT.aff", @"C:\Users\lenci\OneDrive - Istituto Istruzione Superiore Volterra Elia\Iride\SmartSearchAPI\SmartSearchAPI\elastic_hunspell_master_dicts_it_IT.dic");

            ThesResult tr = thes.Lookup(Keyword, hunspell);

            List<string> suggestions = hunspell.Suggest(Keyword);
            int n = 0;

            while(tr == null && n < suggestions.Count)
            {
                tr = thes.Lookup(suggestions[n]);
                n++;
            }
            if (tr != null)
            {
                if (n > 0)
                {
                    Synonyms.Add(suggestions[n - 1]);
                }
                foreach (var meaning in tr.Meanings)
                {
                    foreach (var synonym in meaning.Synonyms)
                    {
                        Synonyms.Add(synonym);
                    }
                }
            }
        }
    }
}
