// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Region.cs" company="">
//   
// </copyright>
// <summary>
//   The word region.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EnglishStemmer
{
    using System;

    /// <summary>
    /// The word region.
    /// </summary>
    internal class WordRegion
    {
        #region Fields

        /// <summary>
        /// The end.
        /// </summary>
        public int End;

        /// <summary>
        /// The start.
        /// </summary>
        public int Start;

        /// <summary>
        /// The _text.
        /// </summary>
        private string _text;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WordRegion"/> class.
        /// </summary>
        public WordRegion()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordRegion"/> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        public WordRegion(int start, int end)
        {
            this.Start = start;
            this.End = end;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text
        {
            get
            {
                return this._text;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool Contains(int index)
        {
            return index >= this.Start && index <= this.End;
        }

        /// <summary>
        /// The generate region.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void GenerateRegion(string text)
        {
            if (text.Length > this.Start)
            {
                this._text = text.Substring(this.Start, Math.Min(this.End, text.Length) - this.Start);
            }
            else
            {
                this._text = string.Empty;
            }
        }

        #endregion
    }
}