namespace Penguin
{
    public class ParsedMessage
    {
        public ITask[] Tasks { get; set; }

        public Token[] Tokens { get; set; }

        public string RawMessage { get; set; }

        public string Value { get; set; }
    }
}
