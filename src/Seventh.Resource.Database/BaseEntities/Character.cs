namespace Seventh.Resource.Database.Entity
{
    public class Character
    {
        public int CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string FirstName { get; set; }
        public int CharaNormalImageId { get; set; }
        public string Nickname { get; set; }
        public string SortName { get; set; }
        public int? Age { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public int? Bust { get; set; }
        public int? Waist { get; set; }
        public int? Hips { get; set; }
        public string Cup { get; set; }
        public int BirthMonth { get; set; }
        public int Birthday { get; set; }
        public int Constellation { get; set; }
        public int BloodType { get; set; }
        public string SpecialAbility { get; set; }
        public string Favorite { get; set; }
        public string Affiliation { get; set; }
        public string Cv { get; set; }
        public int EpisodeSortId { get; set; }
        public int SegmentId { get; set; }
        public string EnglishName { get; set; }
        public bool DeleteFlg { get; set; }
    }
}