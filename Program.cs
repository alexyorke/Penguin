namespace Penguin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Newtonsoft.Json;
    using PlayerIOClient;
    using System.Threading;

    internal class Program
    {
        public static void AskUser(string message)
        {
            Console.Write(message);
        }

        public static void RunTestsLocal()
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            KeywordDescription[] descriptions = JsonConvert.DeserializeObject<KeywordDescription[]>(jsonRaw);

            Config config = new Config("German");
            Tokenizer tokenizer = new Tokenizer(config, descriptions);
            tokenizer.ProcessPhrase("Username", "füllen diesen Bereich mit diesen Blöcken", AskUser);

            string message = null;
            while ((message = Console.ReadLine()) != null)
            {
                tokenizer.HandlePhrase("Username", message, AskUser);
            }
        }

        public static void RunTestsEE()
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            KeywordDescription[] descriptions = JsonConvert.DeserializeObject<KeywordDescription[]>(jsonRaw);

            Config config = new Config("English");
            Tokenizer tokenizer = new Tokenizer(config, descriptions);
            tokenizer.OnMessageParsed += delegate(ParsedMessage message)
            {
                //Handle commands here
                Console.WriteLine(message.RawMessage + " -> " + message.Value);
            };

            Client client = PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", "hexanywhere@gmail.com", "penguinpassword");
            Connection connection = client.Multiplayer.JoinRoom("PWabBXJqbebEI", null);
            Dictionary<int, string> players = new Dictionary<int, string>();
            connection.OnMessage += delegate(object s, Message m)
            {
                switch (m.Type)
                {
                    case "add":
                        players.Add(m.GetInt(0), m.GetString(1));
                        break;
                    case "remove":
                        players.Remove(m.GetInt(0));
                        break;
                    case "say":
                        if (players.ContainsKey(m.GetInt(0)))
                        {
                            string username = players[m.GetInt(0)];
                            tokenizer.ProcessPhrase(username, m.GetString(1), delegate(string message)
                            {
                                connection.Send("say", message);
                                Thread.Sleep(15);
                            });
                        }

                        break;
                }
            };

            connection.Send("init");
            connection.Send("init2");
            Console.ReadKey();

            connection.Disconnect();
        }

        public static void Main(string[] args)
        {
            RunTestsEE();
        }
    }
}