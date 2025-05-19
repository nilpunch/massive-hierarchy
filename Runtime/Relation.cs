namespace Massive
{
	public struct Relation
	{
		public int FirstChildId;
		public int PrevSiblingId;
		public int NextSiblingId;
		public int ParentId;

		[DefaultValue]
		public static Relation Default => new Relation()
		{
			FirstChildId = Constants.InvalidId,
			PrevSiblingId = Constants.InvalidId,
			NextSiblingId = Constants.InvalidId,
			ParentId = Constants.InvalidId
		};
	}
}
