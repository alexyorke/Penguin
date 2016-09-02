// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tokenizer.cs" company="">
//   
// </copyright>
// <summary>
//   The script.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace PenguinSdk
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

    using EEPhysics;
    using PenguinSdk.Tasks;

    /// <summary>
    ///     The ask user.
    /// </summary>
    /// <param name="message">
    ///     The message.
    /// </param>
    public delegate void UserMessageHandler(string message);

    /// <summary>
    /// The message parsed handler.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public delegate void MessageParsedHandler(ParsedMessage message);

    /// <summary>
    /// The replace event handler.
    /// </summary>
    /// <param name="sourceID">
    /// The source id.
    /// </param>
    /// <param name="replaceID">
    /// The replace id.
    /// </param>
    public delegate void ReplaceEventHandler(int sourceID, int replaceID);

    /// <summary>
    /// The fill event handler.
    /// </summary>
    /// <param name="blockID">
    /// The block id.
    /// </param>
    public delegate void FillEventHandler(int blockID);

    /// <summary>
    /// The delete event handler.
    /// </summary>
    /// <param name="blockID">
    /// The block id.
    /// </param>
    public delegate void DeleteEventHandler(int blockID);

    /// <summary>
    /// The undo event handler.
    /// </summary>
    public delegate void UndoEventHandler();

    /// <summary>
    ///     The script.
    /// </summary>
    public class Tokenizer : IDisposable
    {
        #region Static Fields

        /// <summary>
        ///     The cancel commands.
        /// </summary>
        public static readonly string[] CancelCommands = { "cancel", "stop", "halt", "abort", "staph" };

        /// <summary>
        ///     The erase commands.
        /// </summary>
        public static readonly string[] EraseCommands =
            {
                "erase", "remove", "vanish", "disappear", "delete", "nullify", 
                "void", "expunge", "abolish", "eliminate"
            };

        /// <summary>
        ///     The fill commands.
        /// </summary>
        public static readonly string[] FillCommands = { "fill", "pad", "bucket" };

        /// <summary>
        ///     The find commands.
        /// </summary>
        public static readonly string[] FindCommands = { "find", "locate", "see", "observe", "hunt", "seek" };

        /// <summary>
        ///     The move commands.
        /// </summary>
        public static readonly string[] MoveCommands = { "move", "push" };

        /// <summary>
        /// Command joint keywords
        /// </summary>
        public static readonly string[] CommandJointKeywords = { "and" };

        /// <summary>
        ///     The block reference keywords.
        /// </summary>
        public static readonly string[] BlockReferenceKeywords = { "these", "those", "them", "it", "that" };

        /// <summary>
        ///     The replace commands.
        /// </summary>
        public static readonly string[] ReplaceCommands =
            {
                "replace", "change", "restore", "compensate", "patch", 
                "alter"
            };

        /// <summary>
        ///     The undo commands.
        /// </summary>
        public static readonly string[] UndoCommands =
            {
                "undo", "back", "free", "reverse", "revert", "reappear", 
                "rewind"
            };

        #endregion

        #region Fields

        /// <summary>
        ///     List of all combined categories of commands (NOT KEYWORDS)
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
        private readonly List<Token> ambigiousTokens;

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
        ///     Task queue
        /// </summary>
        private readonly TaskQueue taskQueue;

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
        ///     The processed tokens.
        /// </summary>
        private List<Token> processedTokens;

        /// <summary>
        ///     Raw message
        /// </summary>
        private string rawMessage;

        /// <summary>
        ///     Penguin selection
        /// </summary>
        private ISelection selection;

        /// <summary>
        ///     Physics world
        /// </summary>
        private PhysicsWorld world;

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

            this.ambigiousTokens = new List<Token>();
            this.taskQueue = new TaskQueue();
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Event for when message from the user is parsed
        /// </summary>
        public event MessageParsedHandler OnMessageParsed;

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

        /// <summary>
        ///     Username ID penguin is talking to
        /// </summary>
        public int TalkingToId { get; set; }

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
        /// Gets the last task exluding T
        /// </summary>
        /// <typeparam name="T">
        /// Type to exclude
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public List<Task> GetFinished()
        {
            return this.taskQueue.GetFinished();
        }

        /// <summary>
        /// Get running task excluding T
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public List<Task> GetRunning()
        {
            return this.taskQueue.GetRunning();
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

                if (searchTerm[0].IsIn(BlockReferenceKeywords))
                {
                    for (int i = processed.Count - 1; i >= 0; i--)
                    {
                        if (processed[i].Type == TokenType.Block)
                        {
                            return processed[i];
                        }
                    }

                    token.Type = TokenType.Block;
                    token.Value = new[] { searchTerm[0].ToLowerInvariant() };
                    
                    token.IsUnknown = true;
                    return token;
                }

                if (searchTerm[0].IsIn(CommandJointKeywords))
                {
                    //Search for previous command, otherwise ignore
                    for (int i = processed.Count - 1; i >= 0; i--)
                    {
                        if (processed[i].Type == TokenType.Command)
                        {
                            return processed[i];
                        }
                    }
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
            if (potentialMatches.Length > 0)
            {                
                for (int i = processed.Count - 1; i >= 0; i--)
                {
                    if (processed[i].DescriptorCount > 1)
                    {
                        for (int j = processed[i].DescriptorCount - 1; j >= 0; j--)
                        {
                            Token blockToken = this.GetBlockToken(new[] { potentialMatches[0], processed[i].Descriptors[j] });
                            if (blockToken != null)
                            {
                                combined = Token.Combine(combined, blockToken);
                            }
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
        /// <param name="usernameId">
        /// The username Id.
        /// </param>
        /// <param name="phrase">
        /// Message from the user
        /// </param>
        /// <param name="callback">
        /// </param>
        public void HandlePhrase(int usernameId, string phrase, UserMessageHandler callback)
        {
            if (world != null && world.Players.ContainsKey(usernameId))
            {
                string username = this.world.Players[usernameId].Name;
                if (this.TalkingTo != null)
                {
                    switch (string.Compare(this.TalkingTo, username, StringComparison.OrdinalIgnoreCase))
                    {
                        case 0:
                            if (this.ambigiousTokens.Count > 0)
                            {
                                Token waiting = this.ambigiousTokens[0];
                                string translatedResponse = this.translator.Translate(
                                    phrase, 
                                    this.config.Language, 
                                    "English");
                                if (waiting.ParseResponse(this, translatedResponse))
                                {
                                    this.processedTokens.Add(waiting);

                                    this.ambigiousTokens.RemoveAt(0);
                                    if (this.ambigiousTokens.Count > 0)
                                    {
                                        Token next = this.ambigiousTokens[0];
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
                                    // Translate config message
                                    int length = this.config.MisunderstoodMessages.Length;
                                    string misunderstoodMessage =
                                        this.config.MisunderstoodMessages[this.config.Random.Next(length)];
                                    string translatedMisundersoodMessage = this.translator.Translate(
                                        misunderstoodMessage, 
                                        "English", 
                                        this.config.Language);
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

                this.ProcessPhrase(usernameId, phrase, callback);
            }
        }

        /// <summary>
        /// Loads a map for the tokenizer to use for processing
        /// </summary>
        /// <param name="map">
        /// Map to use
        /// </param>
        /// <param name="world">
        /// The world.
        /// </param>
        public void Load(PenguinMap map, PhysicsWorld world)
        {
            this.selection = new RectangularSelection(map);
            this.world = world;
        }

        /// <summary>
        /// Queues the task to be run
        /// </summary>
        /// <param name="task">
        /// Task to queue
        /// </param>
        /// <param name="runAysnc">
        /// The run Aysnc.
        /// </param>
        public void QueueTask(Task task, bool runAysnc)
        {
            this.taskQueue.Queue(task, runAysnc);
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

            int i = 0;
            while (i < thePhraseWords.Length)
            {
                // this is the tokenizer. It looks for a word that exists in the search
                // terms and checks if the next word is also in the search term. If so,
                // it looks for the third word and checks if that is also in the search
                // terms. If it is then it creates a token with that block description in it,
                // and breaks when there are more than three words or the next word is not contained
                // in the search list.
                if (this.RecursiveFind(thePhraseWords[i]))
                {
                    tmp.Add(thePhraseWords[i]);
                }

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


                if (tmp.Count <= 0)
                {
                    i++;
                    continue;
                }

                indicies.Add(i - count, count);
                final.Add(tmp.ToArray());
                tmp.Clear();

                if (count == 0)
                {
                    i++;
                }
            }

            var processed = new List<Token>();
            for (int j = 0; j < final.Count; j++)
            {
                Token t = this.GetToken(processed, final[j]);
                t.Index = j;

                if (t.IsAmbigious || t.IsUnknown)
                {
                    this.ambigiousTokens.Add(t);
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
        ///     The finalize processing.
        /// </summary>
        private void FinalizeProcessing()
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
            var message = new ParsedMessage();
            message.Caller = this.world.Players[this.TalkingToId];
            message.Value = tokenizedMessage.ToString();
            message.RawMessage = this.rawMessage;
            message.Tokens = this.processedTokens.ToArray();

            var tasks = new List<Task>();
            for (int i = 0; i < this.processedTokens.Count; i++)
            {
                if (this.processedTokens[i].Type == TokenType.Command)
                {
                    if (this.processedTokens[i].Value[0].IsIn(ReplaceCommands))
                    {
                        // Check for 2 args
                        if (this.HasValidArguments(i, 2))
                        {
                            var replace = new Replace(
                                this, 
                                this.selection, 
                                int.Parse(this.processedTokens[i + 1].Value[0]), 
                                int.Parse(this.processedTokens[i + 2].Value[0]));
                            tasks.Add(replace);
                        }
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(EraseCommands))
                    {
                        // Check for 1 args
                        if (this.HasValidArguments(i, 1))
                        {
                            var erase = new Erase(this, this.selection, int.Parse(this.processedTokens[i + 1].Value[0]));
                            tasks.Add(erase);
                        }
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(FillCommands))
                    {
                        // Check for 1 args
                        if (this.HasValidArguments(i, 1))
                        {
                            int posX = (int)Math.Round(message.Caller.X / 16.0);
                            int posY = (int)Math.Round(message.Caller.Y / 16.0);
                            var fill = new Fill(this, this.selection, int.Parse(this.processedTokens[i + 1].Value[0]), posX, posY);
                            tasks.Add(fill);
                        }
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(FindCommands))
                    {
                        // Check for 1 args
                        if (this.HasValidArguments(i, 1))
                        {
                        }
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(MoveCommands))
                    {
                        // Check for 2 args
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(UndoCommands))
                    {
                        // Check for 0 args
                        var undo = new Undo(this, this.selection);
                        tasks.Add(undo);
                    }
                    else if (this.processedTokens[i].Value[0].IsIn(CancelCommands))
                    {
                        // Check for 0 args
                        var cancel = new Cancel(this, this.selection);
                        tasks.Add(cancel);
                    }
                }
            }

            message.Tasks = tasks.ToArray();
            this.TalkingTo = null;
            this.TalkingToId = -1;
            if (this.OnMessageParsed != null)
            {
                this.OnMessageParsed(message);
            }
        }

        /// <summary>
        /// Checks to see if a command has enough arguments to execute
        /// </summary>
        /// <param name="index">
        /// Index of the command
        /// </param>
        /// <param name="args">
        /// Block Arguments needed
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool HasValidArguments(int index, int args)
        {
            int count = 0;
            for (int i = index + 1; i <= index + args; i++)
            {
                if (i >= processedTokens.Count)
                {
                    return false;
                }
                else if (this.processedTokens[i].Type == TokenType.Block)
                {
                    count++;
                    if (count >= args)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

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
        /// The run pseudo.
        /// </summary>
        /// <param name="usernameId">
        /// The username Id.
        /// </param>
        /// <param name="rawMessage">
        /// The raw message.
        /// </param>
        /// <param name="callback">
        /// User confirmation callback
        /// </param>
        private void ProcessPhrase(int usernameId, string rawMessage, UserMessageHandler callback)
        {
            this.TalkingToId = usernameId;
            this.TalkingTo = this.world.Players[usernameId].Name;
            this.rawMessage = rawMessage;

            // (2) Translate raw message into english
            string translatedMessage = this.translator.Translate(rawMessage, this.config.Language, "English");

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

            // (4.4) Check if the processedTokens contains command
            bool hasCommands = false;
            for (int j = 0; j < ambigiousTokens.Count; j++)
            {
                Token current = ambigiousTokens[j];
                for (int i = 0; i < Math.Min(processedTokens.Count, current.Index); i++)
                {
                    if (this.processedTokens[i].Type == TokenType.Command)
                    {
                        if (!(this.processedTokens[i].Value[0].IsIn(CancelCommands)) &&
                            !(this.processedTokens[i].Value[0].IsIn(UndoCommands)))
                        {
                            hasCommands = true;
                            break;
                        }
                    }
                }

                if (!hasCommands)
                {
                    ambigiousTokens.RemoveAt(j);
                    j--;
                }
                else
                {
                    break;
                }
            }

            if (this.ambigiousTokens.Count > 0 && hasCommands)
            {
                // Start asking user
                Token next = this.ambigiousTokens[0];
                string question = next.GetUserConfirmation(this.config);
                string translatedQuestion = this.translator.Translate(question, "English", this.config.Language);
                callback(translatedQuestion);
            }
            else
            {
                ambigiousTokens.Clear();
                this.FinalizeProcessing();
            }
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

            if (theNeedle.IsIn(BlockReferenceKeywords))
            {
                return true;
            }

            if (theNeedle.IsIn(CommandJointKeywords))
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