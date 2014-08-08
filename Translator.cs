// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translator.cs" company="">
//   
// </copyright>
// <summary>
//   The translator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Penguin
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;

    using NHunspell;

    /// <summary>
    /// The translator.
    /// </summary>
    public class Translator
    {
        #region Static Fields

        /// <summary>
        /// The languages.
        /// </summary>
        public static Language[] languages;

        /// <summary>
        /// The language map.
        /// </summary>
        private static readonly Dictionary<string, string> languageMap;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Translator"/> class.
        /// </summary>
        static Translator()
        {
            languageMap = new Dictionary<string, string>();
            languageMap.Add("afrikaans", "af");
            languageMap.Add("albanian", "sq");
            languageMap.Add("arabic", "ar");
            languageMap.Add("armenian", "hy");
            languageMap.Add("azerbaijani", "az");
            languageMap.Add("basque", "eu");
            languageMap.Add("belarusian", "be");
            languageMap.Add("bengali", "bn");
            languageMap.Add("bulgarian", "bg");
            languageMap.Add("catalan", "ca");
            languageMap.Add("chinese", "zh-CN");
            languageMap.Add("croatian", "hr");
            languageMap.Add("czech", "cs");
            languageMap.Add("danish", "da");
            languageMap.Add("dutch", "nl");
            languageMap.Add("english", "en");
            languageMap.Add("esperanto", "eo");
            languageMap.Add("estonian", "et");
            languageMap.Add("filipino", "tl");
            languageMap.Add("finnish", "fi");
            languageMap.Add("french", "fr");
            languageMap.Add("galician", "gl");
            languageMap.Add("german", "de");
            languageMap.Add("georgian", "ka");
            languageMap.Add("greek", "el");
            languageMap.Add("haitian Creole", "ht");
            languageMap.Add("hebrew", "iw");
            languageMap.Add("hindi", "hi");
            languageMap.Add("hungarian", "hu");
            languageMap.Add("icelandic", "is");
            languageMap.Add("indonesian", "id");
            languageMap.Add("irish", "ga");
            languageMap.Add("italian", "it");
            languageMap.Add("japanese", "ja");
            languageMap.Add("korean", "ko");
            languageMap.Add("lao", "lo");
            languageMap.Add("latin", "la");
            languageMap.Add("latvian", "lv");
            languageMap.Add("lithuanian", "lt");
            languageMap.Add("macedonian", "mk");
            languageMap.Add("malay", "ms");
            languageMap.Add("maltese", "mt");
            languageMap.Add("norwegian", "no");
            languageMap.Add("persian", "fa");
            languageMap.Add("polish", "pl");
            languageMap.Add("portuguese", "pt");
            languageMap.Add("romanian", "ro");
            languageMap.Add("russian", "ru");
            languageMap.Add("serbian", "sr");
            languageMap.Add("slovak", "sk");
            languageMap.Add("slovenian", "sl");
            languageMap.Add("spanish", "es");
            languageMap.Add("swahili", "sw");
            languageMap.Add("swedish", "sv");
            languageMap.Add("tamil", "ta");
            languageMap.Add("telugu", "te");
            languageMap.Add("thai", "th");
            languageMap.Add("turkish", "tr");
            languageMap.Add("ukrainian", "uk");
            languageMap.Add("urdu", "ur");
            languageMap.Add("vietnamese", "vi");
            languageMap.Add("welsh", "cy");
            languageMap.Add("yiddish", "yi");

            var welsh = new Language("welsh");
            welsh.AddEntry("Undo", "Cefn");
            welsh.AddEntry("Fill", "Llenwch");
            welsh.AddEntry("Erase", "Dileu");
            welsh.AddEntry("Replace", "Newid");
            welsh.AddEntry("Line", "Llinell");
            welsh.AddEntry("Lines", "Llinellau");
            welsh.AddEntry("Circle", "Cylch");
            welsh.AddEntry("Move", "Hwb");
            welsh.AddEntry("Resize", "Newid");
            welsh.AddEntry("Cancel", "Diddymu");
            welsh.AddEntry("Stop", "Arosiadau");
            welsh.AddEntry("Here", "Yma");
            welsh.AddEntry("And", "a");

            languages = new[] { welsh };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Translator"/> class.
        /// </summary>
        /// <param name="userLanguage">
        /// The user language.
        /// </param>
        public Translator(string userLanguage)
        {
            this.UserLanguage = userLanguage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the user language.
        /// </summary>
        public string UserLanguage { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The check spelling.
        /// </summary>
        /// <param name="hunspell">
        /// The hunspell.
        /// </param>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="suggestions">
        /// The suggestions.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckSpelling(Hunspell hunspell, string term, out List<string> suggestions)
        {
            // Translate word if necessary to english
            string word = this.GetTranslation(term);

            // Get suggestions
            suggestions = hunspell.Suggest(word);

            // Return if word was spelled correctly
            return hunspell.Spell(word);
        }

        /// <summary>
        /// The get translation.
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTranslation(string term)
        {
            // Check if language is already english
            if (string.Compare(this.UserLanguage, "English", true) == 0)
            {
                return term;
            }

            // Language is not enlish, check cache for translation
            for (int i = 0; i < languages.Length; i++)
            {
                if (string.Compare(languages[i].Name, this.UserLanguage) == 0)
                {
                    if (languages[i].HasTranslation(term))
                    {
                        return languages[i].GetTranslation(term);
                    }
                }
            }

            // Check google for translation
            string translated = this.GoogleTranslate(term, this.UserLanguage, "English");

            // Return term back if google was unable to translate
            return translated ?? term;
        }

        /// <summary>
        /// The google translate.
        /// </summary>
        /// <param name="phrase">
        /// The term.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GoogleTranslate(string phrase, string from, string to)
        {
            // Check if language is already english
            if (string.Compare(from, to, true) == 0)
            {
                return phrase;
            }

            string fromCulture = languageMap[from.ToLowerInvariant()];
            string toCulture = languageMap[to.ToLowerInvariant()];

            string url =
                string.Format(
                    @"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}", 
                    HttpUtility.UrlEncode(phrase), 
                    fromCulture, 
                    toCulture);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0";

            string html = null;
            using (WebResponse response = request.GetResponse())
            {
                using (var rdr = new StreamReader(response.GetResponseStream()))
                {
                    html = rdr.ReadToEnd();
                }
            }

            // Trim " to deserialize json result
            return Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value.Trim('\"');
        }

        #endregion
    }
}