namespace Massive.Hierarchy
{
	public struct Relationship
	{
		public Entity FirstChild;

		public Entity PrevSibling;
		public Entity NextSibling;

		public Entity Parent;
	}
}