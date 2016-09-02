// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParsedMessage.cs" company="">
//   
// </copyright>
// <summary>
//   The parsed message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using EEPhysics;

    /// <summary>
    /// The parsed message.
    /// </summary>
    public class ParsedMessage
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the caller.
        /// </summary>
        public PhysicsPlayer Caller { get; set; }

        /// <summary>
        /// Gets or sets the raw message.
        /// </summary>
        public string RawMessage { get; set; }

        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        public Task[] Tasks { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        public Token[] Tokens { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}