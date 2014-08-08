using System;

namespace Penguin
{
    public struct Config
    {
        public string Language { get; set; }

        public string ConfirmationPrefix { get; set; }

        public string MisunderstoodMessage { get; set; }

        public Config(string language) : this()
        {
            Language = language;

            ConfirmationPrefix = "Did you mean";

            MisunderstoodMessage = "Pardon? Please rephrase that.";
        }

        
    }
}
