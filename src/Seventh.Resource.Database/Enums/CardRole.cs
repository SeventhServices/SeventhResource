using System.Text.Json.Serialization;

namespace Seventh.Resource.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CardRole
    {
        Normal,
        SparkleJoker,
        SpecialLessonJoker,
        GroupLessonJoker
    }
}
