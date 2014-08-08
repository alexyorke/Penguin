namespace Penguin
{
    using Newtonsoft.Json;

    public class KeywordDescription
    {
        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("b_ids")]
        public int[] BlockIds { get; set; }

        public override string ToString()
        {
            //Debug view
            return string.Format("{0}: {1}", Keyword, string.Join(", ", BlockIds));
        }

        public bool Equals(KeywordDescription obj)
        {
            //Do not ignore casing
            return string.Compare(Keyword, obj.Keyword, false) == 0;
        }
    }
}
