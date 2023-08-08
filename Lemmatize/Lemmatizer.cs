using LemmaSharp;
using System.IO;

namespace Lemmatize
{
    public class Lemmatizer
    {
        private readonly ITrainableLemmatizer l;

        public Lemmatizer()
        {
            l = new LemmatizerPrebuiltFull(LanguagePrebuilt.Italian);
            StreamReader sr = new StreamReader(@"..\Lemmatize\Lemma_train.txt");
            while(!sr.EndOfStream)
            {
                var s = sr.ReadLine().Split('\t');
                l.AddExample(s[0], s[1]);
            }
        }

        public string GetLemma(string word)
        {
            return l.Lemmatize(word).ToLower();
        }
    }
}
