using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalyst;
using Mosaik.Core;

namespace NLP
{
    public class NLP_processor
    {
        public async Task<Tuple<string, string>[]> ProcessAsync(string input)
        {
            Catalyst.Models.Italian.Register();
            Storage.Current = new DiskStorage("catalyst-models");
            var nlp = await Pipeline.ForAsync(Language.Italian);
            var doc = new Document(input, Language.Italian);
            nlp.ProcessSingle(doc);
            List<Tuple<string, string>> r = new List<Tuple<string, string>>();
            foreach(var data in doc.TokensData[0])
            {
                int start = data.Bounds[0];
                int end = data.Bounds[1];
                r.Add(new Tuple<string, string>(input.Substring(start, end - start + 1), data.Tag.ToString()));
            }
            return r.ToArray();
        }
    }
}
