namespace PenguinSdk
{
    class unused
    {
    }
}
/// <summary>
/// Disabled because map composition creation is much much more efficient.
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
/*private string[] Optimize(string query)
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

    theQuery = this.FilterQuery(theQuery);
    for (var i = 0; i < theQuery.Length - 1; i++)
    {
        var thisItem = theQuery[i];
        if (thisItem.Is(theQuery[i + 1]))
        {
            theQuery[i + 1] = string.Empty;
        }
    }

    theQuery = this.FilterQuery(theQuery);
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
}*/


/// <summary>
/// The duplicate remover. Removes duplicates in lists.
/// </summary>
/// <param name="searchDb">
/// The Search DB.
/// </param>
/// <returns>
/// The unique <see cref="string[][]"/>.
/// </returns>
/*private string[] RemoveDuplicates(BlockDescription[] descriptions)
{
    var final = new List<string[]>();
    var finalStrings = new List<string>();
    foreach (var thisItem in descriptions.Where(thisItem => !thisItem.ToString().IsIn(finalStrings)))
    {
        final.Add(thisItem.Keyword);
        finalStrings.Add(thisItem.Keyword.ToString());
    }

    return final.ToArray();
}*/






/*
        
*/


/*
        /// <summary>
        /// The find duplicates next to each other function. This function checks
        ///     if the same command is specified twice. If it is then it merges it into one
        ///     command to reduce the load on the server. This function is only used by the
        ///     optimize function.
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
            for (int i = 0; i < localX.Length; i++)
            {
                string thisItem = localX[i];
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
*/

/*
        /// <summary>
        /// Resolves ambiguity arising from phrases such as "move the left blocks left 2 blocks" since
        ///     the term "left" is both a command word and a search term.
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        private void AmbigiousResolver(string[] term)
        {
            var ambigiousTerms = new string[] { "left", "right", "up", "down" };
            for (int i = 0; i < term.Length; i++)
            {
                if (term[i].IsIn(ambigiousTerms))
                {
                    if (term[i-1] != "move")
                }
            }
            // Empty
        }
*/


/*
        /// <summary>
        /// The filter query function. This function does goes through the query
        ///     and checks if the item isn't empty. if it is, remove it.
        /// </summary>
        /// <param name="theQuery">
        /// The the_query.
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private string[] FilterQuery(string[] theQuery)
        {
            return theQuery.Where(thisItem => thisItem.IsNot(string.Empty)).ToArray();
        }
*/


// Better for future translations instead of System.String.IndexOf

/*

*/


/*
        /// <summary>
        /// Checks if the sub-arrays contain a certain string. If so, return the number
        ///     of matches. Useful for finding search terms.
        /// </summary>
        /// <param name="searchDb">
        /// </param>
        /// <param name="searchTerm">
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int SuperFind(IEnumerable<string> searchDb, string searchTerm)
        {
            return searchDb.Count(thisItem => thisItem.Contains(searchTerm));
        }
*/


/*
        /// <summary>
        /// Parse function to be called after block id's have been tokenized.
        ///     This means that this function compiles the phrase into Iceberg.
        /// </summary>
        /// <param name="thePhrase">
        /// </param>
        /// <returns>
        /// The <see cref="string[]"/>.
        /// </returns>
        private StringBuilder Parse(string thePhrase)
        {
            string[] englishNumbers =
                As.TheWordsOf(
                    "one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen twenty");

            string currentBlock = null;
            bool inReplaceAll = false;
            bool inReplace = false;
            bool inRemove = false;
            string[] separatedQuery = thePhrase.Split(' ');
            var processed = new StringBuilder();

            for (int i = 0; i < separatedQuery.Length; i++)
            {
                string thisItem = separatedQuery[i];

                if (i < separatedQuery.Length - 1)
                {
                    if (thisItem.IsIn(ReplaceCommands) && separatedQuery[i + 1].IsIn("all", "al"))
                    {
                        // this expression always evaluates to false apparently
                        if (inReplace)
                        {
                            // always false apparently :p
                            // set end of processed to "REPLACE_ALL:"
                            processed.AppendLine("REPLACE_ALL:");
                            inReplace = false;
                        }

                        inReplaceAll = true;
                    }
                }

                if (thisItem.IsIn(ReplaceCommands))
                {
                    processed.AppendLine("REPLACE:");
                    inReplace = true;
                }

                if (thisItem.IsIn(EraseCommands))
                {
                    processed.AppendLine("DELETE:");
                    inRemove = true; // make sure to check if we are in a function

                    // so that the "->" can be added correctly.
                }

                // Got it here I think
                if (thisItem.Is("with"))
                {
                    processed.AppendLine("->");
                }

                if (thisItem.IsIn("and", "then"))
                {
                    processed.AppendLine(thisItem);
                    if (!inReplaceAll && !inReplace && !inRemove)
                    {
                        processed.AppendLine("return");
                    }

                    currentBlock = thisItem;
                }

                if (thisItem.IsIn(BlockReferenceKeywords))
                {
                    processed.AppendLine(currentBlock); // use the current block that

                    // was mentioned the last time.
                }

                if (thisItem.IsIn(MoveCommands))
                {
                    processed.AppendLine("return");
                    processed.AppendLine("MOVE:");
                }

                if (thisItem.IsIn("left", "up") && separatedQuery.HasNext(i))
                {
                    processed.AppendLine("->-");

                    // Optimization instead of checking if exists then finding index,
                    // just check if index is not -1
                    int possibleNumIndex = this.ItemOffset(separatedQuery[i + 1], englishNumbers);
                    if (possibleNumIndex != -1)
                    {
                        // add a coordinate (y coordiante) if moving left or right
                        int number = possibleNumIndex + 1; // one is at 0th index
                        processed.AppendLine(number.ToString(CultureInfo.InvariantCulture));

                        processed.AppendLine(thisItem.Is("up") ? "Y" : "X");
                    }
                }

                if (thisItem.IsIn("down", "right") && separatedQuery.HasNext(i))
                {
                    processed.AppendLine("->");
                    int possibleNumIndex = this.ItemOffset(separatedQuery[i + 1], englishNumbers);
                    if (possibleNumIndex != -1)
                    {
                        int number = possibleNumIndex + 1; // one is at 0th index
                        processed.AppendLine(number.ToString(CultureInfo.InvariantCulture));
                        processed.AppendLine(thisItem.Is("down") ? "X" : "Y");
                    }
                }

                if (thisItem.IsIn(CancelCommands))
                {
                    processed.AppendLine("CANCEL_LAST_COMMAND");
                }
            }

            return processed;
        }
*/

