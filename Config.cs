using System;

namespace Penguin
{
    public struct Config
    {
        public Random Random { get; private set; }

        public string Language { get; set; }

        public string ConfirmationPrefix { get; set; }

        public string[] MisunderstoodMessages { get; set; }

        public Config(string language) : this()
        {
            Random = new Random();

            Language = language;

            ConfirmationPrefix = "Did you mean";

            MisunderstoodMessages = new string[] 
            {
                "Pardon? Please rephrase that.",
                "I didn't understand your answer, try again.",
                "I'm not finding any meaning in your answer, try something else."
            };
        }

        
    }
}
