using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Penguin
{
    public class Translator
    {
        public string UserLanguage { get; set; }

        public string GetTranslation(string term)
        {
            for (int i = 0; i < languages.Length; i++)
            {
                if (string.Compare(languages[i].Name, UserLanguage) == 0)
                {
                    if (languages[i].HasTranslation(term))
                    {
                        return languages[i].GetTranslation(term);
                    }
                }
            }

            //Check google
            string translated = GoogleTranslate(term);

            //Return term back was google was unable to translate
            return translated ?? term;
        }

        public string GoogleTranslate(string term)
        {
            string fromCulture = languageMap[UserLanguage.ToLower()];

            string url = string.Format(@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",
                                       HttpUtility.UrlEncode(term), fromCulture, "en");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0";

            string html = null;
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
                {
                    html = rdr.ReadToEnd();
                }
            }

            //Trim " to deserialize json result
            return Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value.Trim('\"');
        }

        public Translator(string language)
        {
            this.UserLanguage = language;
        }

        private static readonly Dictionary<string, string> languageMap;

        public static Language[] languages;

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


            Language welsh = new Language("welsh");
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

            languages = new Language[]
            {
                welsh
            };
        }

    }
}
