namespace PenguinSdk
{
    using System;

    using PlayerIOClient;

    internal class Program
    {
        public static void RunTestsEE()
        {
            Client client = PlayerIO.QuickConnect.SimpleConnect("everybody-edits-su9rn58o40itdbnw69plyw", "email", "pass", null);
            Connection connection = client.Multiplayer.JoinRoom("PWyiCdOcZEbEI", null);
            Penguin.OnMessageForUser += delegate(string message)
            {
                connection.Send("say", message);
            };

            Penguin.OnMessageParsed += delegate(ParsedMessage message)
            {
                Console.WriteLine(message.RawMessage + " -> " + message.Value);
                for (int i = 0; i < message.Tasks.Length; i++)
                {
                    message.Tasks[i].Queue(message.Caller, connection, 15);
                }
            };

            Config config = new Config("English");
            Penguin.Initialize(config);

            connection.OnMessage += delegate(object s, Message m)
            {
                Console.WriteLine(m.Type);
                if (m.Type == "init")
                {
                    connection.Send("init2");
                }
                Penguin.HandleMessage(m);
            };

            connection.Send("init");
            
            Console.ReadKey();

            connection.Disconnect();
            Penguin.Release();
        }

        public static void Main(string[] args)
        {
            RunTestsEE();
        }
    }
}