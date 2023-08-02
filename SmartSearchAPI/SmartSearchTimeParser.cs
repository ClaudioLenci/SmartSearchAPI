using Microsoft.Recognizers.Definitions.Arabic;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmartSearchAPI
{
    public class SmartSearchTimeParser
    {
        readonly string[] months = { "gennaio", "febbraio", "marzo", "aprile", "maggio", "giugno", "luglio", "agosto", "settembre", "ottobre", "novembre", "dicembre" };
        readonly string[] daysofweek = { "lunedì", "martedì", "mercoledì", "giovedì", "venerdì", "sabato", "domenica" };
        readonly string[] expressions = { "l'altroieri", "ieri", "oggi", "domani", "dopodomani" };
        readonly string[] expressions2 = { "scorso", "corrente", "prossimo" };
        readonly string[] names = { "giorno", "settimana", "mese", "anno" };
        readonly string[] preps = { "di", "del", "dello", "della", "dell’", "dei", "degli", "delle", "a", "al", "allo", "alla", "all’", "ai", "agli", "alle", "da", "dal", "dallo", "dalla", "dall’", "dai", "dagli", "dalle", "in", "nel", "nello", "nella", "nell’", "nei", "negli", "nelle", "su", "sul", "sullo", "sulla", "sull’", "sui", "sugli", "sulle" };
        readonly string[] conj = { "e", "o", "anche", "oltre" };
        readonly string[] nconj = { "non", "nemmeno", "tranne", "senza", "eccetto", "ne", "né" };

        public SmartSearchTimeParser()
        {
        }

        public SmartSearchDateRange GetTime(string[] text, int index)
        {
            if (index >= text.Length || index == -1)
            {
                return new SmartSearchDateRange();
            }
            if (IsPrep(text[index]))
            {
                if (text[index].ToLower() == "dal" || text[index].ToLower() == "tra" || text[index].ToLower() == "da" || text[index].ToLower() == "dai" || text[index].ToLower() == "dalla" || text[index].ToLower() == "dalle")
                {
                    int n = Next(text, index);
                    while (n != -1 && !IsConj(text[n]) && !IsPrep(text[n]))
                    {
                        n = Next(text, n);
                    }
                    if (n == -1)
                    {
                        return GetTime(text, Next(text, index));
                    }
                    var r1 = GetTime(text, Next(text, index));
                    var r2 = GetTime(text, n);
                    return new SmartSearchDateRange(r1.DateMin, r2.DateMax);
                }
                if (text[index].ToLower() == "a" || text[index].ToLower() == "al" || text[index].ToLower() == "ai" || text[index].ToLower() == "alla" || text[index].ToLower() == "alle")
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
                return new SmartSearchDateRange(new DateTime(y, 1, 1), new DateTime(y+1, 1, 1));
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

        public int Next(string[] text, int index)
        {
            do
            {
                index++;
            }
            while (index < text.Length && !IsSomething(text[index]));
            return index < text.Length ? index : -1;
        }

        public bool IsSomething(string text)
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
                || IsYear(text);
        }

        public bool IsNconj(string text)
        {
            return nconj.Contains(text.ToLower());
        }

        public bool IsConj(string text)
        {
            return conj.Contains(text.ToLower());
        }

        public bool IsPrep(string text)
        {
            return preps.Contains(text.ToLower());
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

        public SmartSearchDateRange Text2Hour(string[] text, int index)
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
            return expressions2.Contains(text.ToLower());
        }

        public bool IsName(string text)
        {
            return names.Contains(text.ToLower());
        }

        public SmartSearchDateRange Text2Expression2(string[] text, int index)
        {
            int diff = expressions2.ToList().IndexOf(text[index].ToLower()) - 1;
            int n = 0;
            while(n != -1 && !IsName(text[n]))
            {
                n = Next(text, n);
            }
            if (n == -1)
                return new SmartSearchDateRange();
            int y = DateTime.Now.Year;
            int m = DateTime.Now.Month;
            int d = DateTime.Now.Day;
            switch(text[n].ToLower())
            {
                case "giorno":
                    return new SmartSearchDateRange(new DateTime(y, m, d).AddDays(diff), new DateTime(y, m, d).AddDays(diff + 1));
                case "settimana":
                    return new SmartSearchDateRange(StartOfWeek().AddDays(7*diff), StartOfWeek().AddDays(7*(diff+1)));
                case "mese":
                    return new SmartSearchDateRange(new DateTime(y, m, d).AddMonths(diff), new DateTime(y, m, d).AddMonths(diff + 1));
                case "anno":
                    return new SmartSearchDateRange(new DateTime(y, m, d).AddYears(diff), new DateTime(y, m, d).AddYears(diff + 1));
                default:
                    return new SmartSearchDateRange();
            }
        }

        public bool IsHour(string text)
        {
            return Regex.Match(text, "^(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$").Success;
        }

        public DateTime Text2Expression(string text)
        {
            return DateTime.Today.AddDays(expressions.ToList().IndexOf(text.ToLower())-2);
        }

        public bool IsExpression(string text)
        {
            return expressions.Contains(text.ToLower());
        }

        public int Text2DayOfWeek(string text)
        {
            return daysofweek.ToList().IndexOf(text.ToLower()) + 1;
        }

        public bool IsDayOfWeek(string text)
        {
            return daysofweek.Contains(text.ToLower());
        }

        public static DateTime StartOfWeek()
        {
            int diff = (7 + (DateTime.Today.DayOfWeek - DayOfWeek.Monday)) % 7;
            return DateTime.Today.AddDays(-1 * diff);
        }

        public bool IsMonth(string text)
        {
            return months.Contains(text.ToLower());
        }

        public SmartSearchDateRange Text2Month(string[] text, int index)
        {
            int m = months.ToList().IndexOf(text[index].ToLower()) + 1;
            int n = Next(text, index);
            while (n != -1 && !IsYear(text[n]))
            {
                n = Next(text, n);
            }
            int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
            return new SmartSearchDateRange(new DateTime(y, m, 1), new DateTime(y, m + 1, 1));
        }

        public SmartSearchDateRange Text2Day(string[] text, int index)
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
    }
}