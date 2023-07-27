﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        readonly string[] preps = { "di", "a", "da", "in", "con", "su", "per", "tra", "fra", "dal", "dai", "dalle", "dalla", "dei", "della", "dello", "delle", "al", "alle", "ai", "del" };
        readonly string[] conj = { "e", "o", "anche", "oltre" };
        readonly string[] nconj = { "non", "nemmeno", "tranne", "senza", "eccetto", "ne", "né" };

        public SmartSearchDateRange GetTime(string[] text, int index)
        {
            if(index >= text.Length || index == -1)
            {
                return new SmartSearchDateRange();
            }
            if (IsPrep(text[index]))
            {
                if (text[index].ToLower() == "dal" || text[index].ToLower() == "tra")
                {
                    int n = Next(text, index);
                    while (n != -1 && !IsConj(text[n]) && !IsPrep(text[n]))
                    {
                        n = Next(text, n);
                    }
                    var r1 = GetTime(text, index + 1);
                    var r2 = GetTime(text, n);
                    return new SmartSearchDateRange(r1.datemin, r2.datemax);
                }
                return GetTime(text, Next(text, index));
            }
            if (IsConj(text[index]))
            {
                return GetTime(text, Next(text, index));
            }
            if (IsNconj(text[index]))
            {
                SmartSearchDateRange r = GetTime(text, Next(text, index));
                r.include = false;
                return r;
            }
            if (IsExpression(text[index]))
            {
                return new SmartSearchDateRange(text2Expression(text[index]), text2Expression(text[index]).AddHours(24));
            }
            if (IsYear(text[index]))
            {
                int y = int.Parse(text[index]);
                return new SmartSearchDateRange(new DateTime(y, 1, 1), new DateTime(y, 12, DateTime.DaysInMonth(y, 12)));
            }
            if (IsMonth(text[index]))
            {
                int m = text2Month(text[index]);
                int n = Next(text, index);
                while (n != -1 && !IsYear(text[n]))
                {
                    n = Next(text, n);
                }
                int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
                return new SmartSearchDateRange(new DateTime(y, m, 1), new DateTime(y, m, DateTime.DaysInMonth(y, m)).AddHours(24));
            }
            if (IsDay(text[index]))
            {
                int d = int.Parse(text[index]);
                int n = Next(text, index);
                while (n != -1 && !IsYear(text[n]) )
                {
                    n = Next(text, n);
                }
                int y = n != -1 ? int.Parse(text[n]) : DateTime.Today.Year;
                n = Next(text, index);
                while (n != -1 && !IsMonth(text[n]))
                {
                    n = Next(text, n);
                }
                int m = n != -1 ? text2Month(text[n]) : DateTime.Today.Month;
                return new SmartSearchDateRange(new DateTime(y, m, d), new DateTime(y, m, d).AddHours(24));
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
            return index < text.Length && IsSomething(text[index]) ? index : -1;
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

        public DateTime text2Hour(string text)
        {
            string[] time = text.Split(':');
            return DateTime.Today.AddHours(int.Parse(time[0])).AddMinutes(int.Parse(time[1]));
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

        public bool IsHour(string text)
        {
            return Regex.Match(text, "^(0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$").Success;
        }

        public DateTime text2Expression(string text)
        {
            return DateTime.Today.AddDays(expressions.ToList().IndexOf(text.ToLower())-2);
        }

        public bool IsExpression(string text)
        {
            return expressions.Contains(text.ToLower());
        }

        public int text2DayOfWeek(string text)
        {
            return daysofweek.ToList().IndexOf(text.ToLower()) + 1;
        }

        public bool IsDayOfWeek(string text)
        {
            return daysofweek.Contains(text.ToLower());
        }

        public bool IsMonth(string text)
        {
            return months.Contains(text.ToLower());
        }

        public int text2Month(string text)
        {
            return months.ToList().IndexOf(text.ToLower()) + 1;
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