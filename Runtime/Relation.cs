using System.Runtime.CompilerServices;

namespace Massive
{
	public struct Relation
	{
		public int FirstChildIdWithOffset;
		public int PrevSiblingIdWithOffset;
		public int NextSiblingIdWithOffset;
		public int ParentIdWithOffset;

		public int FirstChildId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => FirstChildIdWithOffset - Constants.IdOffset;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => FirstChildIdWithOffset = value + Constants.IdOffset;
		}

		public int PrevSiblingId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => PrevSiblingIdWithOffset - Constants.IdOffset;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => PrevSiblingIdWithOffset = value + Constants.IdOffset;
		}

		public int NextSiblingId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => NextSiblingIdWithOffset - Constants.IdOffset;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => NextSiblingIdWithOffset = value + Constants.IdOffset;
		}

		public int ParentId
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			readonly get => ParentIdWithOffset - Constants.IdOffset;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => ParentIdWithOffset = value + Constants.IdOffset;
		}
	}
}
