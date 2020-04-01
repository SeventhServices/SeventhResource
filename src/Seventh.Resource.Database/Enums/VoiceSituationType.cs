using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Seventh.Resource.Data.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VoiceSituationType
    {
        MypageDefault = 1,
        MypageTap,
        MypageTimer,
        LiveResults = 10,
        LiveResult,
        LiveStart,
        LiveSkill,
        FullCombo,
        LiveDialog
    }
}
