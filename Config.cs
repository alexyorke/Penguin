// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="">
//   
// </copyright>
// <summary>
//   The config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System;

    /// <summary>
    /// The config.
    /// </summary>
    public struct Config
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> struct.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        public Config(string language)
            : this()
        {
            this.Random = new Random();

            this.Language = language;

            this.ConfirmationPrefix = "Did you mean";

            this.MisunderstoodMessages = new[]
                                             {
                                                 "Pardon? Please rephrase that.", 
                                                 "I didn't understand your answer, try again.", 
                                                 "I'm not finding any meaning in your answer, try something else.",
                                                 "Could you repeat that? I don't know what that is.",
                                                 "Sorry, please repeat that.",
                                                 "I'm having a hard time understanding, try again.",
                                                 "The language option can be switched using the <CMD> flag.",
                                                 "Please try again; I can't understand.",
                                                 "What? I mean, I'm not sure I understand fully.",
                                                 "Sorry, I'm just a program and not sure what you said.",
                                                 "Peep! Please be more specific; I don't know what you mean"
                                             };
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the confirmation prefix.
        /// </summary>
        public string ConfirmationPrefix { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the misunderstood messages.
        /// </summary>
        public string[] MisunderstoodMessages { get; set; }

        /// <summary>
        /// Gets the random.
        /// </summary>
        public Random Random { get; private set; }

        #endregion
    }
}