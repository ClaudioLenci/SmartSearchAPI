using Catalyst;

namespace SmartSearchAPI
{
    public class SmartSearchToken
    {
        public List<string> Data { get; set; }
        public List<string> DataTypes { get; set; }
        public string Text
        {
            get
            {
                if (Data.Count > 0)
                {
                    string r = "";
                    foreach (var d in Data)
                        r += d + " ";
                    return r.Substring(0, r.Length - 1);
                }
                return "";
            }
        }
        public int Type { get; set; }
        public SmartSearchVerb Verb { get; set; }
        public SmartSearchKeyword Keyword { get; set; }
        public SmartSearchDateRange DateRange { get; set; }
        private readonly SmartSearchTimeParser parser;

        public SmartSearchToken(List<string> _data, List<string> _dataTypes)
        {
            this.Data = _data;
            this.DataTypes = _dataTypes;
            Verb = new SmartSearchVerb();
            Keyword = new SmartSearchKeyword();
            Type = -1;
            DateRange = new SmartSearchDateRange();
            parser = new SmartSearchTimeParser();
        }

        public SmartSearchToken()
        {
            Data = new List<string>();
            DataTypes = new List<string>();
            Verb = new SmartSearchVerb();
            Keyword = new SmartSearchKeyword();
            Type = -1;
            DateRange = new SmartSearchDateRange();
            parser = new SmartSearchTimeParser();
        }

        //metodo per aggiungere parole (e tag corrispondenti) ai dati del token
        public void AddData(string _data, string _dataType)
        {
            this.Data.Add(_data);
            this.DataTypes.Add(_dataType);
            if (_dataType == "NOUN")
            {
                Keyword.SetSynonyms(true);
                Keyword.Noun = _data;
            }
            if (_dataType == "PROPN")
            {
                Keyword.SetSynonyms(false);
                Keyword.Noun = _data;
            }
            if (_dataType == "VERB")
            {
                Verb.Verb = _data;
            }
        }

        //metodo per classificare il token tramite l'AI Classifier
        public void Classify()
        {
            var modelInput = new Classifier.ModelInput()
            {
                Col0 = Text
            };
            var r = Classifier.Predict(modelInput);
            this.Type = (int)r.PredictedLabel;
        }

        //metodo per ricavare il range temporale tramite SmartSearchTimeParser
        public void GetTime()
        {
            if (Type == 0)
            {
                List<string> txt = new List<string>();
                foreach (var d in Data)
                {
                    txt.Add(d.ToLower());
                }
                DateRange = parser.GetTime(txt.ToArray(), 0);
            }
        }

        //metodo per controllare se il token successivo nell'array tokens
        //può essere unito a quello corrente
        public bool IsMergeableNext(SmartSearchToken token)
        {
            if (token.Type == 0 && this.Type == 0)
            {
                int p = parser.Next(token.Data.ToArray(), -1);
                if (p == -1)
                    p = token.Data.Count - 1;
                return parser.IsPrep(token.Data[p]) || parser.IsYear(token.Data[p]) || parser.IsConj(token.Data[p]) || parser.IsExpression2(token.Data[p]);
            }
            if (token.Type != 0 && this.Type == 0)
            {
                var modelInput = new Classifier.ModelInput()
                {
                    Col0 = this.Text + " " + token.Text
                };
                var r = Classifier.Predict(modelInput);
                return (int)r.PredictedLabel == 0;
            }
            return false;
        }

        //metodo per controllare se il token precedente nell'array tokens
        //può essere unito a quello corrente
        public bool IsMergeablePrev(SmartSearchToken token)
        {
            if (token.Type != 0 && this.Type == 0)
            {
                var modelInput = new Classifier.ModelInput()
                {
                    Col0 = token.Text + " " + this.Text
                };
                var r = Classifier.Predict(modelInput);
                return (int)r.PredictedLabel == 0;
            }
            return false;
        }

        //metodo per unire i dati di due token una volta controllati
        //con i metodi IsMergeableNext e IsMergeablePrev
        public void Merge(SmartSearchToken token)
        {
            this.Data.AddRange(token.Data);
            this.DataTypes.AddRange(token.DataTypes);
            this.Type = 0;
        }
    }
}