using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Penguin
{
    public static class As
    {
        public static string[] theWordsOf(string phrase)
        {
            return phrase.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool Is(this string current, string str)
        {
            //Access translations?
            return string.Compare(current, str, true) == 0;
        }

        public static bool IsNot(this string current, string str)
        {
            return !Is(current, str);
        }

        public static bool IsIn(this string current, params string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (Is(current, array[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIn(this string current, List<string> list)
        {
            //Remove punctuation
            string currentTrimmed = current
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("!", string.Empty)
                .Replace(".", string.Empty)
                .Replace("'", string.Empty)
                .Replace("`", string.Empty)
                .Replace("?", string.Empty);

            //ignoring case, diacriticals, hyphens, punctuation and white space
            for (int i = 0; i < list.Count; i++)
            {
                string elementTrimmed = list[i]
                    .Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("!", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("'", string.Empty)
                    .Replace("`", string.Empty)
                    .Replace("?", string.Empty);

                if (string.Compare(currentTrimmed, elementTrimmed, true) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasNext(this string[] currrent, int index)
        {
            return index + 1 < currrent.Length;
        }
    }
}
