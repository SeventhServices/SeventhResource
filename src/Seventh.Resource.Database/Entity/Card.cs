namespace Seventh.Resource.Database.Entity
{
	public class Card
	{
		public int CardId { get; set; }
		public int CharacterId { get; set; }
		public string PotentialGroup { get; set; }
		public int RarityId { get; set; }
		public int CardTypeId { get; set; }
		public int FacialImageId { get; set; }
		public int CardMessageId { get; set; }
		public int Cost { get; set; }
		public string CardName { get; set; }
		public string Description { get; set; }
		public int MaxLevel { get; set; }
		public int BreakMaxLevel { get; set; }
		public int HpGrowType { get; set; }
		public int AttackGrowType { get; set; }
		public int DefaultHp { get; set; }
		public int DefaultAttack { get; set; }
		public int MaxHp { get; set; }
		public int MaxAttack { get; set; }
		public int BreakMaxHp { get; set; }
		public int BreakMaxAttack { get; set; }
		public int MemberSkillId { get; set; }
		public int LeaderSkillId { get; set; }
		public int BasicExp { get; set; }
		public int Payback7thPt { get; set; }
		public bool TypePotential { get; set; }
		public bool SkillPotential { get; set; }
		public string ComboIds { get; set; }
		public int LiveLeaderSkillId { get; set; }
		public int LiveMemberSkillId { get; set; }
		public int MaxIntimate { get; set; }
		public bool StockFlg { get; set; }
		public bool SignFlg { get; set; }
		public string Role { get; set; }
		public bool LimitedFlg { get; set; }
		public long StartTime { get; set; }
		public bool DeleteFlg { get; set; }
	}
}