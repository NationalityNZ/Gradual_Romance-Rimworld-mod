using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Gradual_Romance
{
    public static class GRGrammarUtility
    {
        public static string SayList(List<string> items, string conjoiner = "AND", string seperator = "COMMA", bool oxfordComma = true)
        {
            if (items.Count() == 1)
            {
                return items[0].UncapitalizeFirst();
            }
            if (items.Count() == 2)
            {
                return (items[0].UncapitalizeFirst() + " " + conjoiner.Translate() + " " + items[1].UncapitalizeFirst());
            }
            int listSize = items.Count();
            StringBuilder listToSay = new StringBuilder();
            for (int i = 0; i < listSize; i++)
            {
                if (i == 0)
                {
                    listToSay.Append(items[0].UncapitalizeFirst());
                    continue;
                }
                if (i == listSize - 1)
                {
                    listToSay.AppendWithSeparator(items[i].UncapitalizeFirst() + " ", conjoiner.Translate());
                    continue;
                }

                listToSay.AppendWithComma(items[i].UncapitalizeFirst() + " ");
            }
            return listToSay.ToString();
        }
    }
}
