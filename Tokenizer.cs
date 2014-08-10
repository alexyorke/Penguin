// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tokenizer.cs" company="">
//   
// </copyright>
// <summary>
//   The script.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Penguin
{
    // THE entire program will do something like this:
    /*
       (1) Get the message from the server. This is the raw message.
     * (2) Remove punctuation and clean message from excess whitespace and other garbage
     * (3) Split message into phrases. Each phrase contains a command.
     * (3.1) almost always split phrases at "and"
     * (3.2) almost always split at command terms
     * (3.3) split at "if's" (I'll have to think of something here)
     * (4) This command is then goes through the first stage parser which
     * tokenizes the keywords.
     * (4.1) Each word is checked for proper spelling
     * (4.1.1) If it is not proper prompt the user.
     * (4.2) checks if it exists in any of the block descriptions
     * (4.3) if it does then add it to the token
     * (4.4) Replace the in-text block descriptions with their corresponding tokens.
     * (4.3.5) If it does not then close off the token. Create a new token.
     * (5) The tokens then passes through a search function which eventually
     * resolves them into specific block ids (and prompts the user for clarification
     * if necessary)
     * (5.1.0) The search will work somewhat to the following
     * (5.1) if the token is not specific enough then prompt the user
     * (5.1.5) if the token has already been resolved and exists in the text then
     * use that resolution rather than prompting the user twice (or n-times)
     * (6) That phrase then goes through the Penguin parser which:
     * - Identifies keywords such as "remove" and "replace" and generates corresponding iceberg
     * code
     * - Locates and identifies syntax errors
     */


    /*
     * Marquee stuff (to make replace a bit more interesting):
     * Given an initial coordinate at the top left hand side and another secondary
     * coordinate at the bottom right hand side what coordinates govern the top right
     * hand side and the bottom left hand side? Well,
     * Let (x1,y1) be the initial coordinate and (x2,y2) be the secondary coordinate.
     * The top right hand coordinate would be given by:
     * (x2,y1) and the bottom left hand coordinate would be:
     * (x1,y2) which would govern an area of (x2-x1)*(y2-y1) blocks
     * 
     */
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using NHunspell;
    using Penguin.Tasks;

    /// <summary>
    ///     The ask user.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    public delegate void UserMessageHandler(string message);

    public delegate void MessageParsedHandler(ParsedMessage message);

    public delegate void ReplaceEventHandler(int sourceID, int replaceID);

    public delegate void FillEventHandler(int blockID);

    public delegate void DeleteEventHandler(int blockID);

    public delegate void UndoEventHandler();

    /// <summary>
    ///     The script.
    /// </summary>
    public class Tokenizer : IDisposable
    {
        #region Static Fields

        /// <summary>
        /// The cancel commands.
        /// </summary>
        public static readonly string[] CancelCommands = { "cancel", "stop", "halt", "abort", "staph" };

        /// <summary>
        /// The erase commands.
        /// </summary>
        public static readonly string[] EraseCommands =
        {
            "erase", "remove", "vanish", "disappear", "delete", "nullify", 
            "void", "expunge", "abolish", "eliminate"
        };

        /// <summary>
        /// The fill commands.
        /// </summary>
        public static readonly string[] FillCommands = { "fill", "pad", "bucket" };

        /// <summary>
        /// The find commands.
        /// </summary>
        public static readonly string[] FindCommands = { "find", "locate", "see", "observe", "hunt", "seek" };

        /// <summary>
        /// The move commands.
        /// </summary>
        public static readonly string[] MoveCommands = { "move", "push" };

        /// <summary>
        /// The reference keywords.
        /// </summary>
        public static readonly string[] ReferenceKeywords = { "these", "those", "them", "it", "that" };

        /// <summary>
        /// The replace commands.
        /// </summary>
        public static readonly string[] ReplaceCommands =
        {
            "replace", "change", "restore", "compensate", "patch", 
            "alter"
        };

        /// <summary>
        /// The undo commands.
        /// </summary>
        public static readonly string[] UndoCommands =
        {
            "undo", "back", "free", "reverse", "revert", "reappear", 
            "rewind", "was"
        };

        #endregion

        #region Fields

        /// <summary>
        ///     List of all combined categories of commands
        /// </summary>
        private readonly string[] Commands = As.Combine(
            ReplaceCommands, 
            MoveCommands, 
            UndoCommands, 
            FindCommands, 
            FillCommands, 
            CancelCommands, 
            EraseCommands);

        /// <summary>
        ///     Queue for ambigious tokens
        /// </summary>
        private readonly Queue<Token> ambigiousTokens;

        /// <summary>
        ///     English spell checker
        /// </summary>
        private readonly Hunspell checker;

        /// <summary>
        ///     The config.
        /// </summary>
        private readonly Config config;

        /// <summary>
        ///     Array of keywords and block ids
        /// </summary>
        private readonly KeywordDescription[] descriptions;

        /// <summary>
        ///     Translator resource
        /// </summary>
        private readonly Translator translator;

        /// <summary>
        ///     Processed Indicies
        /// </summary>
        private Dictionary<int, int> processedIndicies;

        /// <summary>
        ///     Processed message
        /// </summary>
        private string processedMessage;

        /// <summary>
        /// Raw message
        /// </summary>
        private string rawMessage;

        /// <summary>
        /// The processed tokens.
        /// </summary>
        private List<Token> processedTokens;

        #endregion

        #region Properties And Events

        /// <summary>
        /// Event for when message from the user is parsed
        /// </summary>
        public event MessageParsedHandler OnMessageParsed;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="config">
        /// User configurations
        /// </param>
        /// <param name="descriptions">
        /// The descriptions.
        /// </param>
        public Tokenizer(Config config, KeywordDescription[] descriptions)
        {
            this.config = config;
            this.descriptions = descriptions;

            this.checker = new Hunspell("en_US.aff", "en_US.dic");
            this.translator = new Translator(config.Language);

            this.ambigiousTokens = new Queue<Token>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     List of processed tokens
        /// </summary>
        public List<Token> ProcessedTokens
        {
            get
            {
                return this.processedTokens;
            }
        }

        /// <summary>
        ///     Username penguin is talking to
        /// </summary>
        public string TalkingTo { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Cleans the raw message of unecessary punctuation and whitespace
        /// </summary>
        /// <param name="rawMessage">
        /// Raw message from user
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string CleanRaw(string rawMessage)
        {
            bool whitespace = true;
            char[] rawArray = rawMessage.ToArray();
            var builder = new StringBuilder();
            for (int i = 0; i < rawArray.Length; i++)
            {
                if (char.IsPunctuation(rawArray[i]))
                {
                    continue;
                }

                if (char.IsWhiteSpace(rawArray[i]))
                {
                    if (!whitespace)
                    {
                        whitespace = true;
                        builder.Append(rawArray[i]);
                    }
                }
                else
                {
                    whitespace = false;
                    builder.Append(rawArray[i]);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.checker != null)
            {
                this.checker.Dispose();
            }
        }

        /// <summary>
        /// The finalize processing.
        /// </summary>
        public void FinalizeProcessing()
        {
            // (4.5) replace the in-text block descriptions with block ids
            var tokenizedMessage = new StringBuilder();
            for (int i = 0; i < this.processedTokens.Count; i++)
            {
                tokenizedMessage.Append(string.Join(" ", this.processedTokens[i].Value));
                if (i != this.processedTokens.Count - 1)
                {
                    tokenizedMessage.Append(' ');
                }
            }

            // (6) That phrase then goes through the Penguin parser which:
            // * - Identifies keywords such as "remove" and "replace" and generates corresponding iceberg code
            // * - Locates and identifies syntax errors
            ParsedMessage message = new ParsedMessage();
            message.Value = tokenizedMessage.ToString();
            message.RawMessage = rawMessage;
            message.Tokens = processedTokens.ToArray();

            List<ITask> tasks = new List<ITask>();
            for (int i = 0; i < processedTokens.Count; i++)
            {
                if (processedTokens[i].Type == TokenType.Command)
                {
                    if (processedTokens[i].Value[0].IsIn(ReplaceCommands))
                    {
                        //Check for 2 args
                        if (HasValidArguments(i, 2))
                        {
                            Replace replace = new Replace(int.Parse(processedTokens[i + 1].Value[0]), int.Parse(processedTokens[i + 2].Value[0]));
                            tasks.Add(replace);
                        }
                    }
                    else if (processedTokens[i].Value[0].IsIn(EraseCommands))
                    {
                        //Check for 1 args
                        if (HasValidArguments(i, 1))
                        {
                            Erase erase = new Erase(int.Parse(processedTokens[i + 1].Value[0]));
                            tasks.Add(erase);
                        }
                    }
                    else if (processedTokens[i].Value[0].IsIn(FindCommands))
                    {
                        //Check for 1 args
                        if (HasValidArguments(i, 1))
                        {

                        }
                    }
                    else if (processedTokens[i].Value[0].IsIn(MoveCommands))
                    {
                        //Check for 2 args
                        
                    }
                    else if (processedTokens[i].Value[0].IsIn(UndoCommands))
                    {
                        //Check for 0 args
                        Undo undo = new Undo();
                        tasks.Add(undo);
                    }
                    else if (processedTokens[i].Value[0].IsIn(CancelCommands))
                    {
                        //Check for 0 args
                    }
                }
            }

            message.Tasks = tasks.ToArray();
            if (OnMessageParsed != null)
            {
                OnMessageParsed(message);
            }
        } 

        /// <summary>
        /// Checks to see if a command has enough arguments to execute
        /// </summary>
        /// <param name="index">Index of the command</param>
        /// <param name="args">Block Arguments needed</param>
        public bool HasValidArguments(int index, int args)
        {
            int count = 0;
            for (int i = index; i < processedTokens.Count; i++)
            {
                if (processedTokens[i].Type == TokenType.Block)
                {
                    count++;
                    if (count >= args)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// The get block token.
        /// </summary>
        /// <param name="potentialMatches">
        /// The potential matches.
        /// </param>
        /// <returns>
        /// The <see cref="Token"/>.
        /// </returns>
        /// <exception cref="PenguinException">
        /// </exception>
        public Token GetBlockToken(KeywordDescription[] potentialMatches)
        {
            var token = new Token();
            if (potentialMatches.Length < 2)
            {
                throw new PenguinException(
                    "GetBlockToken should not be called unless multiple block desciptors have been found. Check the call stack.");
            }

            var idList = new List<int>();
            var matches = new List<int>();

            // Y contains all potential blocks for the search, loop through the array, and find matching block ids
            for (int i = 0; i < potentialMatches.Length; i++)
            {
                idList.AddRange(potentialMatches[i].BlockIds);
            }

            // Look for duplicates in idList
            for (int a = 0; a < idList.Count; a++)
            {
                for (int b = 0; b < idList.Count; b++)
                {
                    if (b != a)
                    {
                        if (idList[a] == idList[b])
                        {
                            bool contains = false;
                            for (int c = 0; c < matches.Count; c++)
                            {
                                if (matches[c] == idList[a])
                                {
                                    contains = true;
                                    break;
                                }
                            }

                            if (!contains)
                            {
                                matches.Add(idList[a]);
                            }
                        }
                    }
                }
            }

            if (matches.Count > 0)
            {
                token.Descriptors = potentialMatches;
                token.Type = TokenType.Block;
                token.Value = new[] { matches[0].ToString(CultureInfo.InvariantCulture) };
                return token;
            }

            return null;
        }

        /// <summary>
        /// The block searcher. Returns a list of relavent blocks depending on the search token.
        ///     The search token is an array that contains search terms.
        /// </summary>
        /// <param name="processed">
        /// The processed.
        /// </param>
        /// <param name="searchTerm">
        /// The search_term.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public Token GetToken(List<Token> processed, string[] searchTerm)
        {
            var token = new Token();

            // Do command tokenizing first
            if (searchTerm.Length == 1)
            {
                if (searchTerm[0].IsIn(this.Commands))
                {
                    token.Type = TokenType.Command;
                    token.Value = new[] { searchTerm[0].ToLowerInvariant() };
                    return token;
                }

                if (searchTerm[0].IsIn(ReferenceKeywords))
                {
                    for (int i = processed.Count - 1; i >= 0; i--)
                    {
                        if (processed[i].Type == TokenType.Block)
                        {
                            return processed[i];
                        }
                    }

                    token.Type = TokenType.Block;
                    token.Value = new[] { searchTerm[0].ToLowerInvariant() }; ;
                    token.IsUnknown = true;
                    return token;
                }
            }

            // Figure out references

            // remove the duplicates that are recrusively contained within the search terms (side effect)
            KeywordDescription[] potentialMatches =
                this.descriptions.Where(thisItem => this.RecursiveContains(thisItem, searchTerm)).ToArray();

            // "delete remove undo move change find and them those replace it if then however cancel stop"
            if (potentialMatches.Length > 1)
            {
                Token blockToken = this.GetBlockToken(potentialMatches);
                if (blockToken != null)
                {
                    return blockToken;
                }
            }

            var combined = new Token();
            for (int i = processed.Count - 1; i >= 0; i--)
            {
                if (processed[i].DescriptorCount > 1)
                {
                    for (int j = processed[i].DescriptorCount - 1; j >= 0; j--)
                    {
                        Token blockToken = this.GetBlockToken(new[] { potentialMatches[0], processed[i].Descriptors[j] });
                        if (blockToken != null)
                        {
                            combined = Token.Combine(
                                combined, 
                                blockToken);
                        }
                    }
                }
            }

            if (combined.HasValue)
            {
                return combined;
            }

            token.Type = TokenType.Block;
            if (potentialMatches.Length > 0)
            {
                token.Value = new[] { potentialMatches[0].BlockIds[0].ToString(CultureInfo.InvariantCulture) };
            }
            else
            {
                token.IsUnknown = true;
            }

            return token;
        }

        /// <summary>
        /// Handles a PlayerIO player message (step one)
        /// </summary>
        /// <param name="username">
        /// Username speaking
        /// </param>
        /// <param name="phrase">
        /// Message from the user
        /// </param>
        /// <param name="callback">
        /// </param>
        public void HandlePhrase(string username, string phrase, UserMessageHandler callback)
        {
            if (this.TalkingTo != null)
            {
                switch (string.Compare(this.TalkingTo, username, StringComparison.OrdinalIgnoreCase))
                {
                    case 0:
                        if (this.ambigiousTokens.Count > 0)
                        {
                            Token waiting = this.ambigiousTokens.Peek();
                            string translatedResponse = translator.Translate(phrase, config.Language, "English");
                            if (waiting.ParseResponse(this, translatedResponse))
                            {
                                this.processedTokens.Add(waiting);

                                this.ambigiousTokens.Dequeue();
                                if (this.ambigiousTokens.Count > 0)
                                {
                                    Token next = this.ambigiousTokens.Peek();
                                    string question = next.GetUserConfirmation(this.config);
                                    string translatedQuestion = this.translator.Translate(
                                        question,
                                        "English",
                                        this.config.Language);
                                    callback(translatedQuestion);

                                    // Cant finalize processing yet, return.
                                    return;
                                }
                            }
                            else
                            {
                                //Translate config message
                                int length = config.MisunderstoodMessages.Length;
                                string misunderstoodMessage = config.MisunderstoodMessages[config.Random.Next(length)];
                                string translatedMisundersoodMessage = translator.Translate(misunderstoodMessage, "English", config.Language);
                                callback(translatedMisundersoodMessage);

                                // Cant finalize processing yet, return.
                                return;
                            }
                        }
                        this.FinalizeProcessing();
                        break;
                    default:
                        return;
                }
            }

            this.ProcessPhrase(username, phrase, callback);
        }

        /// <summary>
        /// The run pseudo.
        /// </summary>
        /// <param name="username">
        /// </param>
        /// <param name="rawMessage">
        /// The raw message.
        /// </param>
        /// <param name="callback">
        /// User confirmation callback
        /// </param>
        public void ProcessPhrase(string username, string rawMessage, UserMessageHandler callback)
        {
            this.TalkingTo = username;
            this.rawMessage = rawMessage;

            // (2) Translate raw message into english
            string translatedMessage = translator.Translate(rawMessage, config.Language, "English");

            // (2.1) Remove punctuation and clean message from excess whitespace and other garbage
            string message = this.CleanRaw(translatedMessage);

            // (4) This command is then goes through the first stage parser which tokenizes the keywords.
            // (4.1) Each word is checked for proper spelling
            string[] words = message.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];

                var suggestions = new List<string>();
                if (!this.translator.CheckSpelling(this.checker, word, out suggestions) && suggestions.Count > 0)
                {
                    words[i] = suggestions[0];
                }
            }

            // Final spell checked word
            message = string.Join(" ", words);

            // (4.2) checks if it exists in any of the block descriptions
            // (4.3) if it does then add it to the token
            this.processedTokens = new List<Token>(this.TokenizeKeywords(message, out this.processedIndicies));
            this.processedMessage = message;

            if (this.ambigiousTokens.Count > 0)
            {
                // Start asking user
                Token next = this.ambigiousTokens.Peek();
                string question = next.GetUserConfirmation(this.config);
                string translatedQuestion = this.translator.Translate(question, "English", this.config.Language);
                callback(translatedQuestion);
            }
            else
            {
                // No ambiguity, continue processing
                this.FinalizeProcessing();
            }
        }

        /// <summary>
        /// Tokenizes the keywords and replaces the word
        /// </summary>
        /// <param name="thePhrase">
        /// The the Phrase.
        /// </param>
        /// <param name="indicies">
        /// The indicies.
        /// </param>
        /// <returns>
        /// The <see cref="int[]"/>.
        /// </returns>
        public List<Token> TokenizeKeywords(string thePhrase, out Dictionary<int, int> indicies)
        {
            indicies = new Dictionary<int, int>();
            string[] thePhraseWords = As.TheWordsOf(thePhrase);

            var final = new List<string[]>();
            var tmp = new List<string>();

            for (int i = 0; i < thePhraseWords.Length; i++)
            {
                // this is the tokenizer. It looks for a word that exists in the search
                // terms and checks if the next word is also in the search term. If so,
                // it looks for the third word and checks if that is also in the search
                // terms. If it is then it creates a token with that block description in it,
                // and breaks when there are more than three words or the next word is not contained
                // in the search list.
                int count = 0;
                while (i < thePhraseWords.Length)
                {
                    if (this.RecursiveFindBlocks(this.descriptions, thePhraseWords[i]))
                    {
                        tmp.Add(thePhraseWords[i]);
                        count++;
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (count == 0 && this.RecursiveFind(thePhraseWords[i]))
                {
                    tmp.Add(thePhraseWords[i]);
                }

                if (tmp.Count <= 0)
                {
                    continue;
                }

                indicies.Add(i - count, count);
                final.Add(tmp.ToArray());
                tmp.Clear();
            }

            var processed = new List<Token>();
            for (int i = 0; i < final.Count; i++)
            {
                Token t = this.GetToken(processed, final[i]);
                if (t.IsAmbigious || t.IsUnknown)
                {
                    this.ambigiousTokens.Enqueue(t);
                }
                else
                {
                    processed.Add(t);
                }
            }

            return processed;
        }

        #endregion

        #region Methods

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
        private int ItemOffset(string needle, IList<string> haystack)
        {
            for (int i = 0; i < haystack.Count; i++)
            {
                string thisItem = haystack[i];
                if (needle.Is(thisItem))
                {
                    return i;
                }
            }

            return -1;
        }

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

                if (thisItem.IsIn(ReferenceKeywords))
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

        /// <summary>
        /// Function to search x terms in y
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="y">
        /// Items to be searched
        /// </param>
        /// <returns>
        /// If any X terms were found in Y
        /// </returns>
        private bool RecursiveContains(KeywordDescription description, IEnumerable<string> y)
        {
            return y.Any(query => query.Replace(" ", string.Empty).Is(description.Keyword.Replace(" ", string.Empty)));
        }

        /// <summary>
        /// The recursive find.
        /// </summary>
        /// <param name="theNeedle">
        /// The the needle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool RecursiveFind(string theNeedle)
        {
            if (theNeedle.IsIn(this.Commands))
            {
                return true;
            }

            if (theNeedle.IsIn(ReferenceKeywords))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The recursive find.
        /// </summary>
        /// <param name="needle">
        /// </param>
        /// <param name="theNeedle">
        /// The the needle.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool RecursiveFindBlocks(KeywordDescription[] needle, string theNeedle)
        {
            // check if any of the sub items contains that sub string.
            return this.descriptions.Any(thisItem => thisItem.Keyword.Is(theNeedle));
        }

        #endregion
    }
}