using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchToken
    {
        public List<string> data { get; }
        public string text 
        {
            get
            {
                if (data.Count > 0)
                {
                    string r = "";
                    foreach (var d in data)
                        r += d + " ";
                    return r.Substring(0, r.Length - 1);
                }
                return "";
            }
        }
        public SmartSearchKeyword Keywords { get; set; }
        public int type { get; set; }
        public SmartSearchDateRange range { get; set; }

        public SmartSearchToken(List<string> data)
        {
            this.data = data;
            Keywords = new SmartSearchKeyword();
            type = -1;
            range = new SmartSearchDateRange();
        }

        public SmartSearchToken()
        {
            data = new List<string>();
            Keywords = new SmartSearchKeyword();
            type = -1;
            range = new SmartSearchDateRange();
        }

        public void Classify()
        {
            var modelInput = new Classifier.ModelInput()
            {
                Col0 = text
            };
            var r = Classifier.Predict(modelInput);
            type = (int)r.PredictedLabel;
            GetTime();
        }

        public void GetTime()
        {
            if(type == 0)
            {
                SmartSearchTimeParser parser = new SmartSearchTimeParser();
                range = parser.GetTime(data.ToArray(), 0);
            }
        }

        public bool IsMergeable(SmartSearchToken token)
        {
            if(token.type == 0 && this.type == 0)
            {
                SmartSearchTimeParser parser = new();
                int p = parser.Next(token.data.ToArray(), -1);
                if(p == -1)
                    p = token.data.Count-1;
                return parser.IsPrep(token.data[p]) || parser.IsYear(token.data[p]);
            }
            return false;
        }

        public void Merge(SmartSearchToken token)
        {
            if (token.type == 0 && this.type == 0)
            {
                this.data.AddRange(token.data);
                GetTime();
            }
        }
    }
}
