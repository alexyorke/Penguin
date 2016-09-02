// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Language.cs" company="">
//   
// </copyright>
// <summary>
//   The language.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System.Collections.Generic;

    /// <summary>
    /// The language.
    /// </summary>
    public class Language
    {
        #region Fields

        /// <summary>
        /// The english translations.
        /// </summary>
        private readonly Dictionary<string, string> englishTranslations;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Language"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public Language(string name)
        {
            this.Name = name;
            this.englishTranslations = new Dictionary<string, string>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Add an entry to the english translations.
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="translation">
        /// The translation.
        /// </param>
        public void AddEntry(string term, string translation)
        {
            this.englishTranslations.Add(term, translation);
        }

        /// <summary>
        /// Get a translation for a particular term.
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTranslation(string term)
        {
            if (this.englishTranslations.ContainsKey(term))
            {
                return this.englishTranslations[term];
            }

            // Handle error by returning non-translated term for now
            return term;
        }

        /// <summary>
        /// The has translation.
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool HasTranslation(string term)
        {
            return this.englishTranslations.ContainsKey(term);
        }

        #endregion
    }
}