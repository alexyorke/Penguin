namespace Penguin
{
    using Newtonsoft.Json;

    public class BlockDescription
    {
        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("b_ids")]
        public int[] BlockIds { get; set; }
    }
}
