using System;
using System.Text;

namespace Penguin
{
    using System.Collections.Generic;

    /// <summary>
    /// The token type.
    /// </summary>
    public enum TokenType
    {
        Block,
        Command
    }

    public class Token
    {
        /// <summary>
        /// Gets or sets the keyword descriptors.
        /// </summary>
        public KeywordDescription[] Descriptors { get; set; }

        /// <summary>
        /// Gets the descriptor count.
        /// </summary>
        public int DescriptorCount { get { return Descriptors != null ? Descriptors.Length : 0; } }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public TokenType Type { get; set; }

        /// <summary>
        /// Unknown token value
        /// </summary>
        public bool IsUnknown { get; set; }

        /// <summary>
        /// Gets a value indicating whether the block description is ambigious.
        /// </summary>
        public bool IsAmbigious
        {
            get
            {
                return this.Value != null && this.Value.Length > 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the token has a value.
        /// </summary>
        public bool HasValue
        {
            get
            {
                return this.Value != null && this.Value.Length > 0;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string[] Value { get; set; }

        /// <summary>
        /// Gets the ambigious descriptor
        /// </summary>
        private KeywordDescription GetAmbiguity()
        {
            for (int i = 0; i < DescriptorCount; i++)
            {
                for (int j = 0; j < DescriptorCount; j++)
                {
                    if (Descriptors[i].Equals(Descriptors[j]))
                    {
                        return Descriptors[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Generates confirmation message for ambiguity
        /// </summary>
        /// <param name="config">Tokenizer configuration</param>
        /// <returns>User friendly message</returns>
        public string GetUserConfirmation(Config config)
        {
            if (IsUnknown)
            {
                if (Type == TokenType.Command)
                {
                    throw new PenguinException("Invalid token, token cannot be an unknown command.");
                }

                return "What type of block did you mean by \"" + Value[0] + "\"?";
            }

            List<string> currentOption = new List<string>();
            List<List<string>> options = new List<List<string>>();

            int occurance = 1;
            KeywordDescription amgiguity = GetAmbiguity();
            for (int i = 0; i < DescriptorCount; i++)
            {
                currentOption.Add(Descriptors[i].Keyword);
                if (occurance % 2 == 0)
                {
                    occurance = 1;                   
                    options.Add(new List<string>(currentOption));
                    currentOption.Clear();
                }

                if (Descriptors[i].Equals(amgiguity))
                {
                    occurance++;
                }            
            }

            var builder = new StringBuilder();
            builder.Append(config.ConfirmationPrefix);
            builder.Append(' ');
            for (int i = 0; i < options.Count; i++)
            {
                string current = string.Join(" ", options[i]);
                builder.Append(current);
                if (i != options.Count - 1)
                {
                    builder.Append(" or ");
                }
            }

            builder.Append("?");
            return builder.ToString();
        }

        /// <summary>
        /// Parses response from user
        /// </summary>
        /// <param name="tokenizer">The tokenizer</param>
        /// <param name="response">The response</param>
        /// <returns>If the parsed response was parsed</returns>
        public bool ParseResponse(Tokenizer tokenizer, string response)
        {
            string responseClean = tokenizer.CleanRaw(response);

            if (IsUnknown)
            {
                if (Type == TokenType.Command)
                {
                    throw new PenguinException("Invalid token, token cannot be an unknown command.");
                }

                Token token = tokenizer.GetToken(tokenizer.ProcessedTokens, responseClean.Split(' '));
                if (token.HasValue)
                {
                    this.Descriptors = token.Descriptors;
                    this.Value = token.Value;
                    this.IsUnknown = false;
                }

                return token.HasValue;
            }

            var currentOption = new List<string>();
            var options = new List<List<string>>();

            int occurance = 1;
            KeywordDescription amgiguity = GetAmbiguity();
            for (int i = 0; i < DescriptorCount; i++)
            {
                currentOption.Add(Descriptors[i].Keyword);
                if (occurance % 2 == 0)
                {
                    occurance = 1;
                    options.Add(new List<string>(currentOption));
                    currentOption.Clear();
                }

                if (Descriptors[i].Equals(amgiguity))
                {
                    occurance++;
                }
            }        

            int position = 0;
            for (int i = 0; i < options.Count; i++)
            {
                string optionName = string.Join(" ", options[i]);
                if (string.Compare(optionName, responseClean, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    KeywordDescription[] dest = new KeywordDescription[options[i].Count];
                    Array.Copy(Descriptors, position, dest, 0, dest.Length);
                    Descriptors = dest;

                    Value = tokenizer.GetBlockToken(Descriptors).Value;
                    return true;
                }

                position += options[i].Count;
            }

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        public Token()
        {
            Descriptors = new KeywordDescription[0];
            Value = new string[0];
        }

        /// <summary>
        /// Combine two tokens together.
        /// </summary>
        /// <param name="t1">
        /// The first token.
        /// </param>
        /// <param name="t2">
        /// The second token.
        /// </param>
        /// <returns>
        /// The <see cref="Token"/>.
        /// </returns>
        /// <exception cref="PenguinException">
        /// If the tokens are not of the same type.
        /// </exception>
        public static Token Combine(Token t1, Token t2)
        {
            if (t1.Type != t2.Type)
            {
                throw new PenguinException("Cannnot combine two tokens of different types.");
            }

            var result = new Token
                             {
                                 Type = t1.Type,
                                 Descriptors = new KeywordDescription[t1.DescriptorCount + t2.DescriptorCount]
                             };

            Array.Copy(t1.Descriptors, 0, result.Descriptors, 0, t1.DescriptorCount);
            Array.Copy(t2.Descriptors, 0, result.Descriptors, t1.DescriptorCount, t2.DescriptorCount);
            
            result.Value = new string[t1.Value.Length + t2.Value.Length];
            Array.Copy(t1.Value, 0, result.Value, 0, t1.Value.Length);
            Array.Copy(t2.Value, 0, result.Value, t1.Value.Length, t2.Value.Length);
            return result;
        }
    }
}
