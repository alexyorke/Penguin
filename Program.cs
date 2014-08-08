namespace Penguin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    class Program
    {
        static void Main(string[] args)
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            BlockDescription[] descriptions = JsonConvert.DeserializeObject<BlockDescription[]>(jsonRaw);

            Translator translator = new Translator("german");
            string english = translator.GoogleTranslate("hallo");
        }
    }
}