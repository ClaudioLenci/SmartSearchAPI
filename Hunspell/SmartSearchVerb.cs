using NHunspell;
using System.Collections.Generic;
using Lemmatize;

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
        private MyThes thes;
        private Hunspell hunspell;
        private Lemmatizer lemmatizer;

        public SmartSearchVerb(string verb)
        {
            Synonyms = new List<string>();
            Verb = verb;
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
            lemmatizer = new Lemmatizer();
        }

        public SmartSearchVerb()
        {
            Synonyms = new List<string>();
            Verb = "";
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
            lemmatizer = new Lemmatizer();
        }

        public void GetSynonyms()
        {
            _keyword = lemmatizer.GetLemma(Verb);

            ThesResult tr = thes.Lookup(Verb, hunspell);

            List<string> suggestions = hunspell.Suggest(Verb);
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
            if(!Synonyms.Contains(Verb))
                Synonyms.Insert(0, Verb);
        }
    }
}
