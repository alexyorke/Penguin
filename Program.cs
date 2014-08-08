namespace Penguin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    internal class Program
    {
        public static void RunTests()
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            BlockDescription[] descriptions = JsonConvert.DeserializeObject<BlockDescription[]>(jsonRaw);

            Tokenizer tokenizer = new Tokenizer(descriptions);
            

            Translator translator = new Translator("german");
            string english = translator.GoogleTranslate("hallo");
        }

        public static void Main(string[] args)
        {
            RunTests();
        }
    }
}