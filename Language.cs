using System;
using System.Collections.Generic;

namespace Penguin
{
    public class Language
    {
        private Dictionary<string, string> englishTranslations;

        public string Name { get; set; }

        public void AddEntry(string term, string translation)
        {
            englishTranslations.Add(term, translation);
        }

        public bool HasTranslation(string term)
        {
            return englishTranslations.ContainsKey(term);
        }

        public string GetTranslation(string term)
        {
            if (englishTranslations.ContainsKey(term))
            {
                return englishTranslations[term];
            }
            else
            {
                //Handle error by returning non-translated term for now
                return term;
            }
        }

        public Language(string name)
        {
            Name = name;
            englishTranslations = new Dictionary<string, string>();
        }
    }
}
