using NHunspell;
using System.Collections.Generic;

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
                if (value != "" && isNoun)
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

        public SmartSearchKeyword()
        {
            Synonyms = new List<string>();
            isNoun = true;
            Noun = "";
            thes = new MyThes(@".\th_it_IT.dat");
            hunspell = new Hunspell(@".\elastic_hunspell_master_dicts_it_IT.aff", @".\elastic_hunspell_master_dicts_it_IT.dic");
        }

        //funzione per settare la variabile bool che consente di creare i sinonimi
        public void SetSynonyms(bool _set)
        {
            isNoun = _set;
        }

        //funzione per ricavare i sinonimi della keyword
        // + prima si cerca la keyword nel dizionario
        // + poi per ogni significato della keyword si aggiungono tutti i sinonimi alla lista
        public void GetSynonyms()
        {
            ThesResult tr = thes.Lookup(Noun, hunspell);

            List<string> suggestions = hunspell.Suggest(Noun);
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
