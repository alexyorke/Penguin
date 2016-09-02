// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="English.cs">
//   
// </copyright>
// <summary>
//   The english word.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
namespace EnglishStemmer
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The english word.
    /// </summary>
    public class EnglishWord
    {
        #region Constants

        /// <summary>
        /// The valid li ending.
        /// </summary>
        private const string ValidLIEnding = "cdeghkmnrt";

        #endregion

        #region Static Fields

        /// <summary>
        /// The double chars.
        /// </summary>
        private static readonly string[] DoubleChars = new[]
                                                           {
                                                              "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" 
                                                           };

        /// <summary>
        /// The not short syllable non vowels.
        /// </summary>
        private static readonly char[] NotShortSyllableNonVowels = new[]
                                                                       {
                                                                          'a', 'e', 'i', 'o', 'u', 'y', 'w', 'x', 'Y' 
                                                                       };

        /// <summary>
        /// The vowels.
        /// </summary>
        private static readonly char[] Vowels = new[] { 'a', 'e', 'i', 'o', 'u', 'y' };

        #endregion

        #region Fields

        /// <summary>
        /// The _cached up to date.
        /// </summary>
        private bool _cachedUpToDate;

        /// <summary>
        /// The _original.
        /// </summary>
        private string _original = "";

        /// <summary>
        /// The _r 1.
        /// </summary>
        private WordRegion _r1;

        /// <summary>
        /// The _r 2.
        /// </summary>
        private WordRegion _r2;

        /// <summary>
        /// The _stem.
        /// </summary>
        private string _stem = string.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EnglishWord"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="input">
        /// Text that we use to build the stem
        /// </param>
        public EnglishWord(string input)
        {
            this.Create(input);
            this.GenerateStem();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnglishWord"/> class. 
        ///     Default constructor for Unit Testing
        /// </summary>
        internal EnglishWord()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get
            {
                return this.Stem.Length;
            }
        }

        /// <summary>
        /// Gets the original.
        /// </summary>
        public string Original
        {
            get
            {
                return this._original;
            }
        }

        /// <summary>
        /// Gets or sets the stem.
        /// </summary>
        public string Stem
        {
            get
            {
                return this._stem;
            }

            set
            {
                if (this._stem != value)
                {
                    this._stem = value;
                    this._cachedUpToDate = false;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// R1 is the region after the first non-vowel following a vowel, or is the null region at the end of the word if there
        ///     is no such non-vowel.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <returns>
        /// The <see cref="WordRegion"/>.
        /// </returns>
        internal static WordRegion CalculateR(string word, int offset)
        {
            if (offset >= word.Length)
            {
                return new WordRegion(word.Length, word.Length);
            }

            int firstVowel = word.IndexOfAny(Vowels, offset);
            int firstNonVowel = IndexOfNone(word, Vowels, firstVowel);
            int nextVowel = firstNonVowel + 1;

            // int nextNonVowel = IndexOfNone(word, nextVowel, vowels);
            var result = new WordRegion();
            if (nextVowel > 0 && nextVowel < word.Length)
            {
                result.Start = nextVowel;
            }
            else
            {
                result.Start = word.Length;
            }

            result.End = word.Length;
            return result;
        }

        /// <summary>
        /// The create for test.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="EnglishWord"/>.
        /// </returns>
        internal static EnglishWord CreateForTest(string text)
        {
            var word = new EnglishWord();
            word.Create(text);
            return word;
        }

        /// <summary>
        /// The create with r 1 r 2.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="EnglishWord"/>.
        /// </returns>
        internal static EnglishWord CreateWithR1R2(string text)
        {
            EnglishWord result = CreateForTest(text);
            result._r1 = CalculateR(result.Stem, 0);
            result._r2 = CalculateR(result.Stem, result._r1.Start);
            return result;
        }

        /// <summary>
        /// The index of none.
        /// </summary>
        /// <param name="word">
        /// The word.
        /// </param>
        /// <param name="ignoredChars">
        /// The ignored chars.
        /// </param>
        /// <param name="first">
        /// The first.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int IndexOfNone(string word, char[] ignoredChars, int first)
        {
            if (first < 0)
            {
                return -1;
            }

            int firstNone = first;
            do
            {
                firstNone++;
            }
            while (firstNone < word.Length && word.Substring(firstNone, 1).IndexOfAny(ignoredChars) > -1);
            return firstNone;
        }

        /// <summary>
        ///     Finally, turn any remaining Y letters in the word back into lower case.
        /// </summary>
        internal void Finally()
        {
            this.Stem = this.Stem.Replace("Y", "y");
        }

        /// <summary>
        /// The generate stem.
        /// </summary>
        internal void GenerateStem()
        {
            if (this.IsException1())
            {
                return;
            }

            // If the word has two letters or less, leave it as it is. 
            if (this.Stem.Length < 3)
            {
                return;
            }

            // Remove initial ', if present. +
            this.StandardiseApostrophesAndStripLeading();

            // Set initial y, or y after a vowel, to Y
            this.MarkYs();

            // establish the regions R1 and R2. (See note on vowel marking.)
            if (this.Stem.StartsWith("gener") || this.Stem.StartsWith("arsen"))
            {
                this._r1 = CalculateR(this.Stem, 2);
            }
            else if (this.Stem.StartsWith("commun"))
            {
                this._r1 = CalculateR(this.Stem, 3);
            }
            else
            {
                this._r1 = CalculateR(this.Stem, 0);
            }

            this._r2 = CalculateR(this.Stem, this._r1.Start);

            // Step0
            this.StripTrailingApostrophe();

            // Step1a
            this.StripSuffixStep1a();

            if (this.IsException2())
            {
                return;
            }

            // Step1b
            this.StripSuffixStep1b();

            // Step 1c: * 
            this.ReplaceSuffixStep1c();

            // Step2
            this.ReplaceEndingStep2();

            // Step3
            this.ReplaceEndingStep3();

            // Step4
            this.StripSuffixStep4();

            // Step5
            this.StripSuffixStep5();

            // Finally, turn any remaining Y letters in the word back into lower case. 
            this.Finally();
        }

        /// <summary>
        /// The get r 1.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetR1()
        {
            this.CheckCache();
            return this._r1.Text;
        }

        /// <summary>
        /// The get r 2.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetR2()
        {
            this.CheckCache();
            return this._r2.Text;
        }

        /// <summary>
        /// Define a short syllable in a word as either
        ///     (a) a vowel followed by a non-vowel other than w, x or Y and preceded by a non-vowel, or *
        ///     (b) a vowel at the beginning of the word followed by a non-vowel.
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsShortSyllable(int index)
        {
            // Define a short syllable in a word as either 
            int vowelIndex = this.Stem.IndexOfAny(Vowels, index);
            if (vowelIndex < 0)
            {
                // Don't have a vowel
                return false;
            }

            // (a) a vowel followed by a non-vowel other than w, x or Y and preceded by a non-vowel, 
            if (vowelIndex > 0)
            {
                int expectedShortEnd = vowelIndex + 2; // Check word has room for a single non-vowel 

                // and so we add two because length is one more than index of last char
                int nextVowelIndex = this.Stem.IndexOfAny(Vowels, vowelIndex + 1);
                if (nextVowelIndex == expectedShortEnd || this.Stem.Length == expectedShortEnd)
                {
                    int nonVowelIndex = this.Stem.IndexOfAny(NotShortSyllableNonVowels, vowelIndex + 1);
                    int earlyVowelIndex = this.Stem.IndexOfAny(Vowels, vowelIndex - 1);
                    return nonVowelIndex != vowelIndex + 1 // Check not-a-vowel (or w,x,Y) found after vowel
                           && earlyVowelIndex == vowelIndex; // Check no vowels found in char before vowel
                }

                return false;
            }

            return this.Stem.IndexOfAny(Vowels) == 0 && this.Stem.Length > 1 && this.Stem.IndexOfAny(Vowels, 1) != 1;
        }

        /// <summary>
        /// A word is called short if it ends in a short syllable, and if R1 is null.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsShortWord()
        {
            // about to use R1
            this.CheckCache();

            // A word is called short if it ends in a short syllable, and if R1 is null. 
            int lastVowelIndex = this.Stem.LastIndexOfAny(Vowels);
            return lastVowelIndex > -1 && this.IsShortSyllable(lastVowelIndex) && string.IsNullOrEmpty(this.GetR1());
        }

        /// <summary>
        /// The mark ys.
        /// </summary>
        internal void MarkYs()
        {
            var vowelsSearch = new List<char>(Vowels);
            bool previousWasVowel = true;
            var result = new StringBuilder();
            foreach (char c in this.Stem)
            {
                if (c == 'y' && previousWasVowel)
                {
                    result.Append('Y');
                }
                else
                {
                    result.Append(c);
                }

                previousWasVowel = vowelsSearch.Contains(c);
            }

            this.Stem = result.ToString();
        }

        /// <summary>
        ///     Step 2:
        ///     Search for the longest among the following suffixes, and, if found and in R1, perform the action indicated.
        ///     tional:   replace by tion
        ///     enci:   replace by ence
        ///     anci:   replace by ance
        ///     abli:   replace by able
        ///     entli:   replace by ent
        ///     izer   ization:   replace by ize
        ///     ational   ation   ator:   replace by ate
        ///     alism   aliti   alli:   replace by al
        ///     fulness:   replace by ful
        ///     ousli   ousness:   replace by ous
        ///     iveness   iviti:   replace by ive
        ///     biliti   bli+:   replace by ble
        ///     ogi+:   replace by og if preceded by l
        ///     fulli+:   replace by ful
        ///     lessli+:   replace by less
        ///     li+:   delete if preceded by a valid li-ending
        /// </summary>
        internal void ReplaceEndingStep2()
        {
            // Step 2: 
            // Search for the longest among the following suffixes, and, if found and in R1, perform the action indicated. 
            if (this.EndsWithAndInR1("ational"))
            {
                // 7 ational   ation   ator:   replace by ate 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 7) + "ate";
                return;
            }

            ;
            if (this.EndsWithAndInR1("fulness"))
            {
                // 7 fulness:   replace by ful 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 7) + "ful";
                return;
            }

            ;
            if (this.EndsWithAndInR1("iveness"))
            {
                // 7 iveness   iviti:   replace by ive 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 7) + "ive";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ization"))
            {
                // 7 izer   ization:   replace by ize 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 7) + "ize";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ousness"))
            {
                // 7 ousli   ousness:   replace by ous
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 7) + "ous";
                return;
            }

            ;
            if (this.EndsWithAndInR1("biliti"))
            {
                // 6 biliti   bli+:   replace by ble
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 6) + "ble";
                return;
            }

            ;
            if (this.EndsWithAndInR1("lessli"))
            {
                // 6 lessli+:   replace by less 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 6) + "less";
                return;
            }

            ;
            if (this.EndsWithAndInR1("tional"))
            {
                // 6 tional:   replace by tion 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 6) + "tion";
                return;
            }

            ;
            if (this.EndsWithAndInR1("alism"))
            {
                // 5 alism   aliti   alli:   replace by al 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "al";
                return;
            }

            ;
            if (this.EndsWithAndInR1("aliti"))
            {
                // 5 alism   aliti   alli:   replace by al 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "al";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ation"))
            {
                // 5 ational   ation   ator:   replace by ate
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "ate";
                return;
            }

            ;
            if (this.EndsWithAndInR1("entli"))
            {
                // 5 entli:   replace by ent 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "ent";
                return;
            }

            ;
            if (this.EndsWithAndInR1("fulli"))
            {
                // 5 fulli+:   replace by ful 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "ful";
                return;
            }

            ;
            if (this.EndsWithAndInR1("iviti"))
            {
                // 5 iveness   iviti:   replace by ive 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "ive";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ousli"))
            {
                // 5 ousli   ousness:   replace by ous 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "ous";
                return;
            }

            ;
            if (this.EndsWithAndInR1("abli"))
            {
                // 4 abli:   replace by able 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "able";
                return;
            }

            ;
            if (this.EndsWithAndInR1("alli"))
            {
                // 4 alism   aliti   alli:   replace by al 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "al";
                return;
            }

            ;
            if (this.EndsWithAndInR1("anci"))
            {
                // 4 anci:   replace by ance 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "ance";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ator"))
            {
                // 5 ational   ation   ator:   replace by ate
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "ate";
                return;
            }

            ;
            if (this.EndsWithAndInR1("enci"))
            {
                // 4 enci:   replace by ence 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "ence";
                return;
            }

            ;
            if (this.EndsWithAndInR1("izer"))
            {
                // 4 izer   ization:   replace by ize 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4) + "ize";
                return;
            }

            ;
            if (this.EndsWithAndInR1("bli"))
            {
                // 3 biliti   bli+:   replace by ble
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3) + "ble";
                return;
            }

            ;
            if (this.EndsWithAndInR1("ogi"))
            {
                // 3 ogi+:   replace by og if preceded by l 
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3) + "og";
                return;
            }

            ;
            if (this.EndsWithAndInR1("li") && this.IsValidLIEnding())
            {
                // 2 li+:   delete if preceded by a valid li-ending
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 2);
            }

            ;
        }

        /// <summary>
        ///     Step 3:
        ///     Search for the longest among the following suffixes, and, if found and in R1, perform the action indicated.
        ///     tional+:   replace by tion
        ///     ational+:   replace by ate
        ///     alize:   replace by al
        ///     icate   iciti   ical:   replace by ic
        ///     ful   ness:   delete
        ///     ative*:   delete if in R2
        /// </summary>
        internal void ReplaceEndingStep3()
        {
            // ational+:   replace by ate 
            if (this.EndsWithAndInR1("ational"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5) + "e";
                return;
            }

            ;

            // tional+:   replace by tion
            if (this.EndsWithAndInR1("tional"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 2);
                return;
            }

            ;

            // alize:   replace by al
            if (this.EndsWithAndInR1("alize"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3);
                return;
            }

            ;

            // ative*:   delete if in R2 
            if (this.EndsWithAndInR2("ative"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 5);
                return;
            }

            ;

            // icate  :   replace by ic 
            // iciti :   replace by ic 
            if (this.EndsWithAndInR1("icate") || this.EndsWithAndInR1("iciti"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3);
                return;
            }

            ;

            // ical:   replace by ic 
            if (this.EndsWithAndInR1("ical"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 2);
                return;
            }

            ;

            // ness:   delete 
            if (this.EndsWithAndInR1("ness"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 4);
                return;
            }

            ;

            // ful   :   delete 
            if (this.EndsWithAndInR1("ful"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3);
            }

            ;
        }

        /// <summary>
        ///     replace suffix y or Y by i if preceded by a non-vowel which is not the first letter of the word (so cry -> cri, by
        ///     -> by, say -> say)
        /// </summary>
        internal void ReplaceSuffixStep1c()
        {
            // replace suffix y or Y by i if preceded by a non-vowel which is not the first letter of the word (so cry -> cri, by -> by, say -> say)
            if (this.Stem.EndsWith("y", StringComparison.OrdinalIgnoreCase) && (this.Stem.Length > 2)
                && (this.Stem.IndexOfAny(Vowels, this.Stem.Length - 2) != this.Stem.Length - 2))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 1) + "i";
            }
        }

        /// <summary>
        ///     Converts all quote variants `’ " to standard '. Removes an open quote in First char
        /// </summary>
        internal void StandardiseApostrophesAndStripLeading()
        {
            // Make Apostrophes consistent
            this.Stem = this.Stem.Replace('’', '\'').Replace('`', '\'').Replace('"', '\'');

            // Remove initial ', if present. 
            if (this.Stem[0] == '\'')
            {
                this.Stem = this.Stem.Remove(0, 1);
            }
        }

        /// <summary>
        ///     Search for the longest among the following suffixes, and perform the action indicated.
        ///     sses
        ///     replace by ss
        ///     ied+   ies*
        ///     replace by i if preceded by more than one letter, otherwise by ie (so ties -> tie, cries -> cri)
        ///     s
        ///     delete if the preceding word part contains a vowel not immediately before the s (so gas and this retain the s, gaps
        ///     and kiwis lose it)
        ///     us+   ss
        ///     do nothing
        /// </summary>
        internal void StripSuffixStep1a()
        {
            // sses - replace by ss 
            if (this.Stem.EndsWith("sses"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 2); // 4 to remove sses -2 to re-introduce ss
                return;
            }

            // ied+   ies* - replace by i if preceded by more than one letter, otherwise by ie (so ties -> tie, cries -> cri) 
            if (this.Stem.EndsWith("ies") || this.Stem.EndsWith("ied"))
            {
                if (this.Stem.Length > 4)
                {
                    this.Stem = this.Stem.Substring(0, this.Stem.Length - 2);
                }
                else
                {
                    this.Stem = this.Stem.Substring(0, this.Stem.Length - 1);
                }

                return;
            }

            // us+   ss - do nothing 
            if (this.Stem.EndsWith("us") || this.Stem.EndsWith("ss"))
            {
                return;
            }

            // s  - delete if the preceding word part contains a vowel not immediately 
            // 		before the s (so gas and this retain the s, gaps and kiwis lose it)
            if (this.Stem.EndsWith("s") && this.Stem.Length > 2
                && this.Stem.Substring(0, this.Stem.Length - 2).IndexOfAny(Vowels) > -1)
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 1);
            }
        }

        /// <summary>
        ///     Search for the longest among the following suffixes, and perform the action indicated.
        ///     eed   eedly+
        ///     replace by ee if in R1
        ///     ed   edly+   ing   ingly+
        ///     delete if the preceding word part contains a vowel, and then
        ///     if the word ends at, bl or iz add e (so luxuriat -> luxuriate), or
        ///     if the word ends with a double remove the last letter (so hopp -> hop), or
        ///     if the word is short, add e (so hop -> hope)
        /// </summary>
        internal void StripSuffixStep1b()
        {
            // eed   eedly+ - replace by ee if in R1 
            if (this.Stem.EndsWith("eed") || this.Stem.EndsWith("eedly"))
            {
                if (this.EndsWithAndInR1("eed") || this.EndsWithAndInR1("eedly"))
                {
                    if (this._r1.Contains(this.Stem.Length))
                    {
                        this.Stem = this.Stem.Substring(0, this.Stem.LastIndexOf("eed")) + "ee";
                    }
                }

                return;
            }

            // ed   edly+   ing   ingly+ - delete if the preceding word part contains a vowel, and then 
            if ((this.Stem.EndsWith("ed") && this.Stem.IndexOfAny(Vowels, 0, this.Stem.Length - 2) != -1)
                || (this.Stem.EndsWith("edly") && this.Stem.IndexOfAny(Vowels, 0, this.Stem.Length - 4) != -1)
                || (this.Stem.EndsWith("ing") && this.Stem.IndexOfAny(Vowels, 0, this.Stem.Length - 3) != -1)
                || (this.Stem.EndsWith("ingly") && this.Stem.IndexOfAny(Vowels, 0, this.Stem.Length - 5) != -1))
            {
                this.StripEnding(new[] { "ed", "edly", "ing", "ingly" });

                // if the word ends at, bl or iz add e (so luxuriat -> luxuriate), or 
                if (this.Stem.EndsWith("at") || this.Stem.EndsWith("bl") || this.Stem.EndsWith("iz"))
                {
                    this.Stem += "e";
                    return;
                }

                // if the word ends with a double remove the last letter (so hopp -> hop), or 				
                string end2chars = this.Stem.Substring(this.Stem.Length - 2, 2);
                var doubleEndings = new List<string>(DoubleChars);
                if (doubleEndings.Contains(end2chars))
                {
                    this.Stem = this.Stem.Remove(this.Stem.Length - 1);
                    return;
                }

                // if the word is short, add e (so hop -> hope) 
                if (this.IsShortWord())
                {
                    this.Stem += "e";
                }
            }
        }

        /// <summary>
        ///     Step 4: Search for the longest among the following suffixes, and, if found and in R2, perform the action indicated.
        ///     al   ance   ence   er   ic   able   ible   ant   ement   ment   ent   ism   ate   iti   ous   ive   ize delete
        ///     ion delete if preceded by s or t
        /// </summary>
        internal void StripSuffixStep4()
        {
            if (this.EndsWithAndInR2("ement"))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 5);
                return;
            }

            if (this.EndsWithAndInR2("ance") || this.EndsWithAndInR2("ence") || this.EndsWithAndInR2("able")
                || this.EndsWithAndInR2("ible") || this.EndsWithAndInR2("ment"))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 4);
                return;
            }

            if (this.EndsWithAndInR2("ion") && (this.Stem.EndsWith("tion") || this.Stem.EndsWith("sion")))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 3);
                return;
            }

            if (this.Stem.EndsWith("ment"))
            {
                return; // breaking change, but makes the voc.txt parse correctly
            }

            if (this.EndsWithAndInR2("ant") || this.EndsWithAndInR2("ent") || this.EndsWithAndInR2("ism")
                || this.EndsWithAndInR2("ate") || this.EndsWithAndInR2("iti") || this.EndsWithAndInR2("ous")
                || this.EndsWithAndInR2("ize") || this.EndsWithAndInR2("ive"))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 3);
                return;
            }

            if (this.EndsWithAndInR2("al") || this.EndsWithAndInR2("er") || this.EndsWithAndInR2("ic"))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 2);
            }
        }

        /// <summary>
        ///     Step 5: * Search for the the following suffixes, and, if found, perform the action indicated.
        ///     e delete if in R2, or in R1 and not preceded by a short syllable
        ///     l delete if in R2 and preceded by l
        /// </summary>
        internal void StripSuffixStep5()
        {
            if (this.EndsWithAndInR2("e")
                || (this.EndsWithAndInR1("e") && this.IsShortSyllable(this.Stem.Length - 3) == false))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 1);
                return;
            }

            if (this.EndsWithAndInR2("l") && this.Stem.EndsWith("ll"))
            {
                this.Stem = this.Stem.Remove(this.Stem.Length - 1);
            }
        }

        /// <summary>
        ///     Handle the three forms of closing apostrophe
        /// </summary>
        internal void StripTrailingApostrophe()
        {
            if (this.Stem.EndsWith("'s'"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 3);
                return;
            }

            if (this.Stem.EndsWith("'s"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 2);
                return;
            }

            if (this.Stem.EndsWith("'"))
            {
                this.Stem = this.Stem.Substring(0, this.Stem.Length - 1);
            }
        }

        /// <summary>
        /// The check cache.
        /// </summary>
        private void CheckCache()
        {
            if (this._cachedUpToDate == false)
            {
                this._r1.GenerateRegion(this._stem);
                this._r2.GenerateRegion(this._stem);
                this._cachedUpToDate = true;
            }
        }

        /// <summary>
        /// Refactored construction. Gets the object into the correct initial state
        /// </summary>
        /// <param name="input">
        /// </param>
        private void Create(string input)
        {
            this.Stem = input.ToLower();
            this._original = input;
        }

        /// <summary>
        /// The ends with and in r 1.
        /// </summary>
        /// <param name="suffix">
        /// The suffix.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EndsWithAndInR1(string suffix)
        {
            return this.Stem.EndsWith(suffix) && this.GetR1().Contains(suffix);
        }

        /// <summary>
        /// The ends with and in r 2.
        /// </summary>
        /// <param name="suffix">
        /// The suffix.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool EndsWithAndInR2(string suffix)
        {
            return this.Stem.EndsWith(suffix) && this.GetR2().Contains(suffix);
        }

        /// <summary>
        /// The is exception 1.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsException1()
        {
            switch (this.Stem)
            {
                case "skis":
                    this.Stem = "ski";
                    return true;
                case "skies":
                    this.Stem = "sky";
                    return true;
                case "dying":
                    this.Stem = "die";
                    return true;
                case "lying":
                    this.Stem = "lie";
                    return true;
                case "tying":
                    this.Stem = "tie";
                    return true;

                case "idly":
                    this.Stem = "idl";
                    return true;
                case "gently":
                    this.Stem = "gentl";
                    return true;
                case "ugly":
                    this.Stem = "ugli";
                    return true;
                case "early":
                    this.Stem = "earli";
                    return true;
                case "only":
                    this.Stem = "onli";
                    return true;
                case "singly":
                    this.Stem = "singl";
                    return true;

                case "sky":
                    return true;
                case "news":
                    return true;
                case "howe":
                    return true;

                case "atlas":
                    return true;
                case "cosmos":
                    return true;
                case "bias":
                    return true;
                case "andes":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// define exception2 as (
        ///     [substring] atlimit among(
        ///     'inning' 'outing' 'canning' 'herring' 'earring'
        ///     'proceed' 'exceed' 'succeed'
        ///     // ... extensions possible here ...
        ///     )
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsException2()
        {
            switch (this.Stem)
            {
                case "inning":
                    return true;
                case "outing":
                    return true;
                case "canning":
                    return true;
                case "herring":
                    return true;
                case "earring":
                    return true;
                case "proceed":
                    return true;
                case "exceed":
                    return true;
                case "succeed":
                    return true;
            }

            return false;
        }

        /// <summary>
        /// The is valid li ending.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsValidLIEnding()
        {
            if (this.Stem.Length > 2)
            {
                string preLi = this.Stem.Substring(this.Stem.Length - 3, 1);
                return ValidLIEnding.Contains(preLi);
            }

            return false;
        }

        /// <summary>
        /// The strip ending.
        /// </summary>
        /// <param name="endings">
        /// The endings.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool StripEnding(string[] endings)
        {
            foreach (string ending in endings)
            {
                if (this.Stem.EndsWith(ending))
                {
                    this.Stem = this.Stem.Remove(this.Stem.Length - ending.Length);
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}