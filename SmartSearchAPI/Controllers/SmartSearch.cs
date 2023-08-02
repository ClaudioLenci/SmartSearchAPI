using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLP;

namespace SmartSearchAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmartSearch : ControllerBase
    {
        [HttpGet(Name = "Search")]
        public SmartSearchResult Search(string search)
        {
            SmartSearchNlpProcessor processor = new SmartSearchNlpProcessor();
            SmartSearchResult output = new SmartSearchResult();

            // manda la frase alla AI Catalyst per farla dividere
            // nelle diverse parti del discorso
            var input = processor.ProcessAsync(search).Result;

            // crea e riempe la lista di token accopiando nomi
            // e elementi aggiuntivi(preposizioni, aggettvi, ecc)
            var tokens = new List<SmartSearchToken> { new SmartSearchToken() };
            foreach (var w in input)
            {
                var last = tokens.Count - 1;
                if (w.Item2 == "ADJ" || w.Item2 == "ADP" || w.Item2 == "ADV" || w.Item2 == "CCONJ" || w.Item2 == "NUM" || w.Item2 == "SCONJ" || w.Item2 == "SYM")
                {
                    tokens[last].Data.Add(w.Item1);
                }
                else if (w.Item2 == "NOUN" || w.Item2 == "PROPN")
                {
                    tokens[last].AddData(w.Item1, w.Item2);
                    tokens.Add(new SmartSearchToken());
                }
                else if (w.Item2 == "AUX" || w.Item2 == "INTJ" || w.Item2 == "PUNCT" || w.Item2 == "VERB")
                {
                    tokens[last].Data.Clear();
                }
            }

            // fa analizzare ogni token al classificatore per sapere
            // se riguarda il tempo, le keyword o è inutile
            foreach (var token in tokens)
            {
                token.Classify();
            }

            // analizza i token
            // + se riguarda il tempo cerca di fare il merge con i token adiacenti,
            //    chiama la funzione di conversione e
            //    aggiunge il risultato alla lista di range di date
            // + se riguarda le keyword aggiunge il nome contenuto in esso
            //    alla lista di keyword
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == 0)
                {
                    while (i + 1 < tokens.Count && tokens[i].IsMergeable(tokens[i + 1]))
                    {
                        tokens[i].Merge(tokens[i + 1]);
                        tokens.RemoveAt(i + 1);
                    }
                    tokens[i].GetTime();
                    output.DateRanges.Add(tokens[i].DateRange);
                }
                else if (tokens[i].Type == 1)
                {
                    output.Keywords.Add(tokens[i].Keyword);
                }
            }

            return output;
        }
    }
}
