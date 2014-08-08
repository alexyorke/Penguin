// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Script.cs" company="None">
//   Copyright somethiung
// </copyright>
// <summary>
//   The script.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Penguin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// The x, contains 2-D array from terms.txt
        /// </summary>
        private string[][] x;

        /// <summary>
        /// The recursive find.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="theNeedle">
        /// The the needle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool recursiveFind(IEnumerable<string[]> x, string theNeedle)
        {
            // check if any of the sub items contains that sub string.
            return x.Any(thisItem => thisItem.Any(t => t.Is(theNeedle)));
        }

        /// <summary>
        /// The block searcher.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="search_term">
        /// The search_term.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string BlockSearcher(string[][] x, string[] search_term)
        {
            // search term is in the form of {"gate", "green"}
            var finalFinal = new List<string>();

            // resolve phrases like "move the right blocks right one block"
            this.ambigiousResolver(string.Empty);

            // remove the duplicats that are recrusively contained within the search terms (side effect)
            var y = this.duplicateRemover(x.Where(thisItem => this.recursiveContains(search_term, thisItem)).ToArray());
            return y[0][0]; // the first item however
            // here the user would be prompted with a list of semi-relavent blocks
            // that they can choose from.
        }

        /// <summary>
        /// </summary>
        /// <param name="term">
        /// </param>
        private void ambigiousResolver(string term)
        {
            // Empty

            return;
        }

        /// <summary>
        /// Function to search x terms in y
        /// </summary>
        /// <param name="x">
        /// Items to search for in y
        /// </param>
        /// <param name="y">
        /// Items to be searched
        /// </param>
        /// <returns>
        /// If any X terms were found in Y
        /// </returns>
        private bool recursiveContains(IEnumerable<string> x, string[] y)
        {
            return x.Select(t => t.Replace(" ", string.Empty)).Any(thisItem => y.Any(t1 => string.Compare(thisItem, t1.Replace(" ", string.Empty), StringComparison.OrdinalIgnoreCase) == 0));
        }

        /// <summary>
        /// The is integer function. Returns true if the function is an integer.
        /// </summary>
        /// <param name="str">
        /// The string.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> of whether or not it is indeed an integer.
        /// </returns>
        private bool IsInteger(string str)
        {
            var result = -1;
            return int.TryParse(str, out result);
        }

        /// <summary>
        /// The duplicate remover. Removes duplicates in lists.
        /// </summary>
        /// <param name="x">
        /// The array.
        /// </param>
        /// <returns>
        /// The unique <see cref="string[][]"/>.
        /// </returns>
        private string[][] duplicateRemover(string[][] x)
        {
            var final = new List<string[]>();
            var finalStrings = new List<string>();
            foreach (var thisItem in x.Where(thisItem => !thisItem.ToString().IsIn(finalStrings)))
            {
                final.Add(thisItem);
                finalStrings.Add(thisItem.ToString());
            }

            return final.ToArray();
        }

        /// <summary>
        /// Checks if the sub-arrays contain a certain string. If so, return the number
        /// of matches. Useful for finding search terms.
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="search_term">
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int superFind(string[] x, string search_term)
        {
            return x.Count(thisItem => thisItem.Contains(search_term));
        }

        /// <summary>
        /// Parse function to be called after block id's have been tokenized.
        /// This means that this function compiles the phrase into Iceberg.
        /// </summary>
        /// <param name="the_phrase">
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private string[] parse(string the_phrase)
        {
            var allowed_words = As.theWordsOf("delete remove undo move change find and them those replace it if then however cancel stop");

            var englishNumbers = As.theWordsOf("one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen");
            string currentBlock = null;
            var inReplaceAll = false;
            const bool InReplace = false;
            var inRemove = false;
            var separatedQuery = the_phrase.Split(' ');
            var processed = new StringBuilder();

            for (var i = 0; i < separatedQuery.Length; i++)
            {
                var thisItem = separatedQuery[i];

                if (i < separatedQuery.Length - 1)
                {
                    if (thisItem.Is("replace") && separatedQuery[i + 1].Is("all"))
                    {
                        if (InReplace) // always false apparently :p
                        {
                            // set end of processed to "REPLACE_ALL:"
                            processed.AppendLine("REPLACE_ALL:");
                        }

                        inReplaceAll = true;
                    }
                }

                if (thisItem.Is("replace"))
                {                    
                    processed.AppendLine("REPLACE:");
                }

                if (thisItem.IsIn("remove", "delete", "erase"))
                {
                    processed.AppendLine("REMOVE:");
                    inRemove = true; // make sure to check if we are in a function
                    // so that the "->" can be added correctly.
                }

                if (thisItem.Is("with"))
                {
                    processed.AppendLine("->");
                }

                if (thisItem.Is("then"))
                {
                    processed.AppendLine(thisItem);
                    if (!inReplaceAll && !InReplace && !inRemove)
                    {
                        processed.AppendLine("return");
                    }

                    currentBlock = thisItem;
                }

                if (thisItem.IsIn("those", "them"))
                {
                    processed.AppendLine(currentBlock); // use the current block that
                    // was mentioned the last time.
                }

                if (thisItem.Is("move"))
                {
                    processed.AppendLine("return");
                    processed.AppendLine("MOVE:");
                }

                if ((thisItem.Is("left") || thisItem.Is("up")) && separatedQuery.HasNext(i))
                {
                    processed.AppendLine("->-");

                    // Optimization instead of checking if exists then finding index,
                    // just check if index is not -1
                    var possibleNumIndex = this.item_offset(separatedQuery[i + 1], englishNumbers);
                    if (possibleNumIndex != -1)
                    {
                        // add a coordinate (y coordiante) if moving left or right
                        var number = possibleNumIndex + 1; // one is at 0th index
                        processed.AppendLine(number.ToString(CultureInfo.InvariantCulture));

                        processed.AppendLine(thisItem.Is("up") ? "Y" : "X");
                    }
                }

                if ((thisItem.Is("down") || thisItem.Is("right")) && separatedQuery.HasNext(i))
                {
                    processed.AppendLine("->");
                    var possibleNumIndex = this.item_offset(separatedQuery[i + 1], englishNumbers);
                    if (possibleNumIndex != -1)
                    {
                        var number = possibleNumIndex + 1; // one is at 0th index
                        processed.AppendLine(number.ToString(CultureInfo.InvariantCulture));
                        processed.AppendLine(thisItem.Is("down") ? "X" : "Y");
                    }
                }

                if (thisItem.IsIn("nevermind", "cancel", "abort", "stop", "undo"))
                {
                    processed.AppendLine("CANCEL_LAST_COMMAND");
                }
            }

            return this.findDuplicatesNextToEachOther(this.Optimize(processed.ToString()));
        }

        // Better for future translations instead of System.String.IndexOf

        /// <summary>
        /// The item offset. The offset of an item in a list.
        /// </summary>
        /// <param name="needle">
        /// The needle.
        /// </param>
        /// <param name="haystack">
        /// The haystack.
        /// </param>
        /// <returns>
        /// The offset <see cref="int"/>.
        /// </returns>
        private int item_offset(string needle, IList<string> haystack)
        {
            for (var i = 0; i < haystack.Count; i++)
            {
                var thisItem = haystack[i];
                if (needle.Is(thisItem))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The filter query function. This function does goes through the query
        /// and checks if the item isn't empty. if it is, remove it.
        /// </summary>
        /// <param name="theQuery">
        /// The the_query.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private string[] filter_query(string[] theQuery)
        {
            return theQuery.Where(thisItem => thisItem.IsNot(string.Empty)).ToArray();
        }

        /// <summary>
        /// The optimize function. This optimizes Iceberg code but isn't ultra important since
        /// the map composite can be generated if given access to the map this is pretty
        /// much useless.
        /// Optimizations include combining multiple add and remove functions as well as
        /// removing dead code that moves blocks off of the screen or replaces non existant
        /// blocks that were previously replaced.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The<see cref="string[]"/>.
        /// </returns>
        private string[] Optimize(string query)
        {
            var theQuery = query.Split('\r', '\n');

            // List<string> the_query_filtered = new List<string>();
            for (var i = 0; i < theQuery.Length; i++)
            {
                var thisItem = theQuery[i];

                // check if the user cancelled the last command. if so, tell Iceberg about it.
                if (!thisItem.Is("CANCEL_LAST_COMMAND"))
                {
                    continue;
                }

                theQuery[i - 1] = string.Empty;
                theQuery[i] = string.Empty;
            }

            theQuery = this.filter_query(theQuery);
            for (var i = 0; i < theQuery.Length - 1; i++)
            {
                var thisItem = theQuery[i];
                if (thisItem.Is(theQuery[i + 1]))
                {
                    theQuery[i + 1] = string.Empty;
                }
            }

            theQuery = this.filter_query(theQuery);
            string lastItem = null;
            for (var i = 0; i < theQuery.Length; i++)
            {
                var thisItem = theQuery[i];
                if (i > 0)
                {
                    lastItem = theQuery[i - 1]; 
                }

                if (thisItem.Is(lastItem))
                {
                    theQuery[i] = string.Empty;
                }

                if (i >= theQuery.Length - 1)
                {
                    continue;
                }

                if (!thisItem.Contains("REPLACE"))
                {
                    continue;
                }

                if (!theQuery[i + 1].Contains("REMOVE"))
                {
                    continue;
                }

                var itemOneSubject = thisItem.Split(':')[1];
                var itemThing = itemOneSubject.Split(new[] { "->" }, StringSplitOptions.None)[0];
                itemOneSubject = itemOneSubject.Split(new[] { "->" }, StringSplitOptions.None)[1];
                var itemTwoSubject = theQuery[i + 1].Split(':')[1];

                if (itemOneSubject.Is(itemTwoSubject))
                {
                    theQuery[i] = "REMOVE:" + itemThing;
                }
            }

            // Remove replace queries if their blocks have already been removed (nothing to replace)
            return theQuery;
        }

        /// <summary>
        /// The find duplicates next to each other function. This function checks
        /// if the same command is specified twice. If it is then it merges it into one
        /// command to reduce the load on the server. This function is only used by the 
        /// optimize function.
        /// </summary>
        /// <param name="localX">
        /// The local_x.
        /// </param>
        /// <returns>
        /// The resulting array.
        /// </returns>
        private string[] findDuplicatesNextToEachOther(string[] localX)
        {
            var final = new List<string>();
            for (var i = 0; i < localX.Length; i++)
            {
                var thisItem = localX[i];
                if (i < localX.Length - 1)
                {
                    if (thisItem.IsNot(localX[i + 1]))
                    {
                        final.Add(thisItem);
                    }
                }
                else
                {
                    final.Add(thisItem);
                }
            }

            return final.ToArray();
        }

        /// <summary>
        /// Loads x terms which hold block id descriptions.
        /// </summary>
        private void LoadX()
        {
            var x = new List<string[]>();
            var content = File.ReadAllText("terms.txt").Substring(1);

            var brakcetIndex = -1;
            var currentContent = content;
            while ((brakcetIndex = currentContent.IndexOf("{", StringComparison.Ordinal)) != -1)
            {
                var bracketStr = currentContent.Substring(brakcetIndex + 1);
                var endBracket = bracketStr.IndexOf("}", StringComparison.Ordinal);

                var array = bracketStr.Substring(0, endBracket).Replace("\"", string.Empty);
                var arrayElements = array.Split(',');
                for (var i = 0; i < arrayElements.Length; i++)
                {
                    arrayElements[i] = arrayElements[i].Trim();
                }

                x.Add(arrayElements);
                currentContent = bracketStr.Substring(endBracket + 3 > currentContent.Length ? endBracket : endBracket + 3);
            }

            this.x = x.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class. 
        /// Applescript 
        /// </summary>
        public Script()
        {
            this.LoadX();
        }

        /// <summary>
        /// The run. Run the entire script.
        /// </summary>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public int[] Run()
        {
            // remove, replace, delete, move, cancel, undo*,
            var the_phrase = As.theWordsOf("remove all of the 883 blocks and replace those with 993 blocks then delete them thanks bye. replace all 883 with 993  Oh sorry could you move all of those blocks right seven blocks? Wait wait sorry cancel that last thing");

            var final = new List<string[]>();
            var tmp = new List<string>();

            var flagOne = false;
            var flagTwo = false;
            var flagThree = false;

            for (var i = 0; i < the_phrase.Length; i++)
            {
                if (flagTwo)
                {
                    flagTwo = false;
                    i++;
                }
                else if (flagThree)
                {
                    flagThree = false;
                    i++;
                }

                // this is the tokenizer. It looks for a word that exists in the search
                // terms and checks if the next word is also in the search term. If so,
                // it looks for the third word and checks if that is also in the search
                // terms. If it is then it creates a token with that block description in it,
                // and breaks when there are more than three words or the next word is not contained
                // in the search list.
                if (this.recursiveFind(this.x, the_phrase[i]))
                {
                    tmp.Add(the_phrase[i]);

                    // set FLAG_ONE to true
                    if (this.recursiveFind(this.x, the_phrase[i + 1]))
                    {
                        tmp.Add(the_phrase[i + 1]);
                        flagTwo = true;

                        if (this.recursiveFind(this.x, the_phrase[i + 2]))
                        {
                            tmp.Add(the_phrase[i + 2]);
                            flagThree = true;
                        }
                    }
                }

                if (tmp.Count <= 0)
                {
                    continue;
                }

                final.Add(tmp.ToArray());
                tmp.Clear();
            }

            // final_ids is just the ids. These ids need to replace the tokenized word. It is very important that this occurs.
            return final.Select(thisItem => int.Parse(this.BlockSearcher(this.x, thisItem))).ToArray();
        }
    }
}
