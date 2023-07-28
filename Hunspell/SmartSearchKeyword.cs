using NHunspell;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartSearchAPI
{
    public class SmartSearchKeyword
    {
        private string _keyword;
        private bool isNoun;
        public string Noun
        {
            get
            {
                return _keyword;
            }
            set
            {
                _keyword = value;
                if(value != "" && isNoun)
                    GetSynonyms();
            }
        }
        public List<string> Synonyms { get; }
        MyThes thes;
        Hunspell hunspell;

        public SmartSearchKeyword(string keyword)
        {
            Synonyms = new List<string>();
            isNoun = true;
            Noun = keyword;
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
        }

        public void SetSynonyms(bool _set)
        {
            isNoun = _set;
        }

        public SmartSearchKeyword()
        {
            Synonyms = new List<string>();
            isNoun = true;
            Noun = "";
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
        }

        //https://www.codeproject.com/Articles/43495/Spell-Check-Hyphenation-and-Thesaurus-for-NET-with
        public void GetSynonyms()
        {
            
            ThesResult tr = thes.Lookup(Noun, hunspell);

            List<string> suggestions = hunspell.Suggest(Noun);
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
