using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Best20HackerNews.Models
{
    public class Story
    {
        public string Title { get; set; }
        public string Url { get; set; }
        [JsonProperty("by")]
        public string PostedBy { get; set; }
        [JsonConverter(typeof(MicrosecondEpochConverter))]
        public DateTime Time { get; set; }
        public int Score { get; set; }

        [JsonProperty("descendants")]
        public int CommentCount { get; set; }
    }

    public class MicrosecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddMilliseconds((long)reader.Value / 1000d);
        }
    }
}
