using Seventh.Resource.Data.Enums;

namespace Namespace
{
    public class CharacterVoice
    {
        public long CharacterVoiceId { get; set; }
		public int CharacterId { get; set; }
		public VoiceSituationType Type { get; set; }
		public int ConditionId { get; set; }
		public string Message { get; set; }
		public string FileName { get; set; }
		public string CueSheetName { get; set; }
		public int Rate { get; set; }
		public bool DeleteFlg { get; set; }

    }
}