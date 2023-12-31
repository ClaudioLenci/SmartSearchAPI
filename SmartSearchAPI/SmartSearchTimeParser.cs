﻿using System.Text.RegularExpressions;

namespace SmartSearchAPI
{
    public class SmartSearchTimeParser
    {
        readonly string[] months = { "gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno", "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre" };
        readonly string[] daysofweek = { "lunedì", "martedì", "mercoledì", "giovedì", "venerdì", "sabato", "domenica" };
        readonly string[] expressions = { "l'altroieri", "ieri", "oggi", "domani", "dopodomani" };
        readonly string[] expressions2 = { "precedente", "scorsa", "scorso", "corrente", "prossimo", "prossima", "seguente" };
        readonly string[] names = { "giorno", "settimana", "mese", "anno" };
        readonly string[] preps = { "di", "del", "dello", "della", "dell’", "dei", "degli", "delle", "a", "al", "allo", "alla", "all’", "ai", "agli", "alle", "da", "dal", "dallo", "dalla", "dall’", "dai", "dagli", "dalle", "in", "nel", "nello", "nella", "nell’", "nei", "negli", "nelle", "su", "sul", "sullo", "sulla", "sull’", "sui", "sugli", "sulle" };
        readonly string[] conj = { "e", "o", "anche", "oltre" };
        readonly string[] nconj = { "non", "nemmeno", "tranne", "senza", "eccetto", "ne", "né" };

        public SmartSearchTimeParser(){}

        //funzione ricorsiva che va a controllare in ogni indice se la parola
        //appartiene a uno degli array sopra e a seconda di dove la trova
        //va a svolgere determinate operazioni
        public SmartSearchDateRange GetTime(string[] text, int index)
        {
            if (index >= text.Length || index == -1)
            {
                return new SmartSearchDateRange();
            }
            if (IsPrep(text[index]))
            {
                if (text[index] == "dal" || text[index] == "tra" || text[index] == "da" || text[index] == "dai" || text[index] == "dalla" || text[index] == "dalle")
                {
                    int n = Next(text, index);
                    while (n != -1 && !IsConj(text[n]) && !IsPrep(text[n]))
                    {
                        n = Next(text, n);
                    }
                    if (n == -1)
                    {
                        return new SmartSearchDateRange(GetTime(text, Next(text, index)).DateMin, DateTime.MaxValue);
                    }
                    var r1 = GetTime(text, Next(text, index));
                    var r2 = GetTime(text, n);
                    return new SmartSearchDateRange(r1.DateMin, r2.DateMax);
                }
                if (text[index] == "a" || text[index] == "al" || text[index] == "ai" || text[index] == "alla" || text[index] == "alle")
                {
                    return new SmartSearchDateRange(DateTime.MinValue, GetTime(text, Next(text, index)).DateMax);
                }
                return GetTime(text, Next(text, index));
            }
            if (IsNconj(text[index]))
            {
                SmartSearchDateRange r = GetTime(text, Next(text, index));
                r.Include = false;
                return r;
            }
            if (IsExpression(text[index]))
            {
                return new SmartSearchDateRange(Text2Expression(text[index]), Text2Expression(text[index]).AddHours(24));
            }
            if (IsExpression2(text[index]))
            {
                return Text2Expression2(text, index);
            }
            if (IsYear(text[index]))
            {
                int y = int.Parse(text[index]);
                return new SmartSearchDateRange(new DateTime(y, 1, 1), new DateTime(y + 1, 1, 1));
            }
            if (IsMonth(text[index]))
            {
                return Text2Month(text, index);
            }
            if (IsDay(text[index]))
            {
                return Text2Day(text, index);
            }
            if (IsHour(text[index]))
            {
                return Text2Hour(text, index);
            }
            if (IsDayOfWeek(text[index]))
            {
                return new SmartSearchDateRange(StartOfWeek().AddDays(Text2DayOfWeek(text[index])), StartOfWeek().AddDays(Text2DayOfWeek(text[index]) + 1));
            }
            if (IsDate(text[index]))
            {
                return new SmartSearchDateRange(DateTime.Parse(text[index]), DateTime.Parse(text[index]).AddHours(24));
            }
            return GetTime(text, Next(text, index));
        }

        //funzione per trovare la prima parola appartenente a uno degli
        //array sopra partendo da un dato index
        public int Next(string[] text, int index)
        {
            do
            {
                index++;
            }
            while (index < text.Length && !IsSomething(text[index]));
            return index < text.Length ? index : -1;
        }

        //funzione per controllare se la parola passata come parametro appartiene
        //a uno degli array sopra elencati
        private bool IsSomething(string text)
        {
            return IsConj(text)
                || IsDate(text)
                || IsDay(text)
                || IsDayOfWeek(text)
                || IsExpression(text)
                || IsHour(text)
                || IsMonth(text)
                || IsNconj(text)
                || IsPrep(text)
                || IsYear(text)
                || IsExpression2(text)
                || IsName(text);
        }

        public bool IsNconj(string text)
        {
            return nconj.Contains(text);
        }

        public bool IsConj(string text)
        {
            return conj.Contains(text);
        }

        public bool IsPrep(string text)
        {
            return preps.Contains(text);
        }

        public bool IsDay(string text)
        {
            try
            {
                int d = int.Parse(text);
                return d >= 0 && d <= 31;
            }
            catch
            {
                return false;
            }
        }

        public bool IsDate(string text)
        {
            try
            {
                DateTime.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsExpression2(string text)
        {
            return expressions2.Contains(text);
        }

        public bool IsName(string text)
        {
            return names.Contains(text);
        }

        public bool IsHour(string text)
        {
            return Regex.Match(text, "^(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$").Success;
        }

        public bool IsExpression(string text)
        {
            return expressions.Contains(text);
        }

        public bool IsDayOfWeek(string text)
        {
            return daysofweek.Contains(text);
        }

        public bool IsMonth(string text)
        {
            return months.Contains(text);
        }

        public bool IsYear(string text)
        {
            try
            {
                int y = int.Parse(text);
                return y > 1800 && y < 2200;
            }
            catch
            {
                return false;
            }
        }

        private SmartSearchDateRange Text2Hour(string[] text, int index)
        {
            string[] time = text[index].Split(':', '.');
            DateTime h = DateTime.Today.AddHours(int.Parse(time[0])).AddMinutes(int.Parse(time[1]));
            int n = Next(text, index);
            while (n != -1 && !IsYear(text[n]))
            {
                n = Next(text, n);
            }
            int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
            n = Next(text, index);
            while (n != -1 && !IsMonth(text[n]))
            {
                n = Next(text, n);
            }
            int m = n != -1 ? months.ToList().IndexOf(text[n]) + 1 : DateTime.Today.Month;
            n = Next(text, index);
            while (n != -1 && !IsDay(text[n]))
            {
                n = Next(text, n);
            }
            int d = n != -1 ? int.Parse(text[n]) : DateTime.Today.Day;
            try
            {
                return new SmartSearchDateRange(new DateTime(y, m, d, h.Hour, h.Minute, 0), new DateTime(y, m, d, h.Hour, h.Minute, 0).AddMinutes(1));
            }
            catch
            {
                return new SmartSearchDateRange(new DateTime(y, m, 1, h.Hour, h.Minute, 0), new DateTime(y, m, 1, h.Hour, h.Minute, 0).AddMinutes(1));
            }
        }

        private SmartSearchDateRange Text2Expression2(string[] text, int index)
        {
            int diff = expressions2.ToList().IndexOf(text[index]) - 3;
            if (diff <= -2)
                diff = -1;
            if (diff >= 2)
                diff = 1;
            int n = 0;
            while (n != -1 && !IsName(text[n]))
            {
                n = Next(text, n);
            }
            if (n == -1)
                return new SmartSearchDateRange();
            int y = DateTime.Now.Year;
            int m = DateTime.Now.Month;
            int d = DateTime.Now.Day;
            switch (text[n])
            {
                case "giorno":
                    return new SmartSearchDateRange(new DateTime(y, m, d).AddDays(diff), new DateTime(y, m, d).AddDays(diff + 1));
                case "settimana":
                    return new SmartSearchDateRange(StartOfWeek().AddDays(7 * diff), StartOfWeek().AddDays(7 * (diff + 1)));
                case "mese":
                    return new SmartSearchDateRange(new DateTime(y, m, 1).AddMonths(diff), new DateTime(y, m, 1).AddMonths(diff + 1));
                case "anno":
                    return new SmartSearchDateRange(new DateTime(y, 1, 1).AddYears(diff), new DateTime(y, 1, 1).AddYears(diff + 1));
                default:
                    return new SmartSearchDateRange();
            }
        }

        private DateTime Text2Expression(string text)
        {
            return DateTime.Today.AddDays(expressions.ToList().IndexOf(text) - 2);
        }

        private int Text2DayOfWeek(string text)
        {
            return daysofweek.ToList().IndexOf(text) + 1;
        }

        private DateTime StartOfWeek()
        {
            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return DateTime.Today.AddDays(-1 * diff);
        }

        private SmartSearchDateRange Text2Month(string[] text, int index)
        {
            int m = months.ToList().IndexOf(text[index]) + 1;
            int n = Next(text, index);
            while (n != -1 && !IsYear(text[n]))
            {
                n = Next(text, n);
            }
            int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
            return new SmartSearchDateRange(new DateTime(y, m, 1), new DateTime(y, m + 1, 1));
        }

        private SmartSearchDateRange Text2Day(string[] text, int index)
        {
            int d = int.Parse(text[index]);
            int n = Next(text, index);
            while (n != -1 && !IsYear(text[n]))
            {
                n = Next(text, n);
            }
            int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
            n = Next(text, index);
            while (n != -1 && !IsMonth(text[n]))
            {
                n = Next(text, n);
            }
            int m = n != -1 ? months.ToList().IndexOf(text[n]) + 1 : DateTime.Today.Month;
            try
            {
                return new SmartSearchDateRange(new DateTime(y, m, d), new DateTime(y, m, d).AddHours(24));
            }
            catch
            {
                return new SmartSearchDateRange(new DateTime(y, m, 1), new DateTime(y, m, 1).AddMonths(1));
            }
        }
    }
}