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
        public static void AskUser(string message)
        {
            Console.Write(message);
        }

        public static void RunTests()
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            KeywordDescription[] descriptions = JsonConvert.DeserializeObject<KeywordDescription[]>(jsonRaw);

            Config config = new Config("English");
            Tokenizer tokenizer = new Tokenizer(config, descriptions);
            tokenizer.ProcessPhrase("Username", "remove all of the orange plastic blocks and replace those with blue plastic ones", AskUser);

            string message = null;
            while ((message = Console.ReadLine()) != null)
            {
                tokenizer.HandlePhrase("Username", message, AskUser);
            }
        }

        public static void Main(string[] args)
        {
            RunTests();
        }
    }
}