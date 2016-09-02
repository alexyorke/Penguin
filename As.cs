// --------------------------------------------------------------------------------------------------------------------
// <copyright file="As.cs" company="">
//   
// </copyright>
// <summary>
//   Applescript extensions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Applescript extensions
    /// </summary>
    public static class As
    {
        #region Public Methods and Operators

        /// <summary>
        /// The combine.
        /// </summary>
        /// <param name="arrays">
        /// The arrays.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public static string[] Combine(params string[][] arrays)
        {
            var list = new List<string>();
            for (int i = 0; i < arrays.Length; i++)
            {
                list.AddRange(arrays[i]);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Calculates if the array has another element after an index
        /// </summary>
        /// <param name="currrent">
        /// The currrent.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool HasNext(this string[] currrent, int index)
        {
            return index + 1 < currrent.Length;
        }

        /// <summary>
        /// The is.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Is(this string current, string str)
        {
            // Access translations?
            return string.Compare(current, str, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// The is in.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

        /// <summary>
        /// The is in.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsIn(this string current, List<string> list)
        {
            // Remove punctuation
            string currentTrimmed =
                current.Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("!", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("'", string.Empty)
                    .Replace("`", string.Empty)
                    .Replace("?", string.Empty);

            // ignoring case, diacriticals, hyphens, punctuation and white space
            for (int i = 0; i < list.Count; i++)
            {
                string elementTrimmed =
                    list[i].Replace(" ", string.Empty)
                        .Replace("-", string.Empty)
                        .Replace("!", string.Empty)
                        .Replace(".", string.Empty)
                        .Replace("'", string.Empty)
                        .Replace("`", string.Empty)
                        .Replace("?", string.Empty);

                if (string.Compare(currentTrimmed, elementTrimmed, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The is not.
        /// </summary>
        /// <param name="current">
        /// The current.
        /// </param>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsNot(this string current, string str)
        {
            return !Is(current, str);
        }

        /// <summary>
        /// Splits the phrase by spaces, ignoring empty ones
        /// </summary>
        /// <param name="phrase">
        /// Phrase to get the words of
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        public static string[] TheWordsOf(string phrase)
        {
            return phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion
    }
}