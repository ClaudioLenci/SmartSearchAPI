using IronPython.Hosting;
using IronPython.Runtime;
using NHunspell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchVerb
    {
        private string _keyword;
        public string Verb
        {
            get
            {
                return _keyword;
            }
            set
            {
                _keyword = value;
                if (value != "")
                    GetSynonyms();
            }
        }
        public List<string> Synonyms { get; }
        MyThes thes;
        Hunspell hunspell;

        public SmartSearchVerb(string verb)
        {
            Synonyms = new List<string>();
            Verb = verb;
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
        }

        public SmartSearchVerb()
        {
            Synonyms = new List<string>();
            Verb = "";
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
        }

        public void GetSynonyms()
        {
            var stems = hunspell.Stem(Verb);
            if(!stems.Contains(Verb))
                stems.Add(Verb);
            foreach (var v in stems)
            {

                ThesResult tr = thes.Lookup(Verb, hunspell);

                List<string> suggestions = hunspell.Suggest(v);
                int n = 0;

                while (tr == null && n < suggestions.Count)
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
}
