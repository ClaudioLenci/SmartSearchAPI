using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public SmartSearchKeyword Keywords { get; set; }
        public int Type { get; set; }
        public SmartSearchDateRange DateRange { get; set; }
        private readonly SmartSearchTimeParser parser;

        public SmartSearchToken(List<string> _data, List<string> _dataTypes)
        {
            this.Data = _data;
            this.DataTypes = _dataTypes;
            Keywords = new SmartSearchKeyword();
            Type = -1;
            DateRange = new SmartSearchDateRange();
            parser = new SmartSearchTimeParser();
        }

        public SmartSearchToken()
        {
            Data = new List<string>();
            DataTypes = new List<string>();
            Keywords = new SmartSearchKeyword();
            Type = -1;
            DateRange = new SmartSearchDateRange();
            parser = new SmartSearchTimeParser();
        }

        public void AddData(string _data, string _dataType)
        {
            this.Data.Add(_data);
            this.DataTypes.Add(_dataType);
            if (_dataType == "NOUN")
            {
                Keywords.SetSynonyms(true);
                Keywords.Noun = _data;
            }
            if (_dataType == "PROPN")
            {
                Keywords.SetSynonyms(false);
                Keywords.Noun = _data;
            }
        }

        public void Classify()
        {
            var modelInput = new Classifier.ModelInput()
            {
                Col0 = Text
            };
            var r = Classifier.Predict(modelInput);
            Type = (int)r.PredictedLabel;
            GetTime();
        }

        public void GetTime()
        {
            if(Type == 0)
            {
                DateRange = parser.GetTime(Data.ToArray(), 0);
            }
        }

        public bool IsMergeable(SmartSearchToken token)
        {
            if(token.Type == 0 && this.Type == 0)
            {
                int p = parser.Next(token.Data.ToArray(), -1);
                if(p == -1)
                    p = token.Data.Count-1;
                return parser.IsPrep(token.Data[p]) || parser.IsYear(token.Data[p]) || parser.IsConj(token.Data[p]);
            }
            return false;
        }

        public void Merge(SmartSearchToken token)
        {
            if (token.Type == 0 && this.Type == 0)
            {
                this.Data.AddRange(token.Data);
                GetTime();
            }
        }
    }
}
