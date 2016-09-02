// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeywordDescription.cs" company="">
//   
// </copyright>
// <summary>
//   The keyword description.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using Newtonsoft.Json;

    /// <summary>
    /// The keyword description.
    /// </summary>
    public class KeywordDescription
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the block ids.
        /// </summary>
        [JsonProperty("b_ids")]
        public int[] BlockIds { get; set; }

        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(KeywordDescription obj)
        {
            // Do not ignore casing
            return System.String.CompareOrdinal(this.Keyword, obj.Keyword) == 0;
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            // Debug view
            return string.Format("{0}: {1}", this.Keyword, string.Join(", ", this.BlockIds));
        }

        #endregion
    }
}