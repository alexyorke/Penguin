// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PenguinException.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin exception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System;

    /// <summary>
    /// The penguin exception.
    /// </summary>
    public class PenguinException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PenguinException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public PenguinException(string message)
            : base(message)
        {
        }

        #endregion
    }
}