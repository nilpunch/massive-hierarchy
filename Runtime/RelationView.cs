// using Unity.IL2CPP.CompilerServices;
//
// namespace Massive
// {
// 	[Il2CppSetOption(Option.NullChecks, false)]
// 	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
// 	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
// 	public readonly struct RelationView : IView
// 	{
// 		public World World { get; }
// 		public int RootId { get; }
//
// 		public RelationView(World world, int rootId)
// 		{
// 			World = world;
// 			RootId = rootId;
// 		}
//
// 		public void ForEach<TAction>(ref TAction action)
// 			where TAction : IEntityAction
// 		{
// 			Assert.IsAlive(World, RootId);
//
// 			var relations = World.Relations.Set;
//
// 			var currentChild = relations.Get(RootId).FirstChildId;
// 			while (currentChild != Constants.InvalidId)
// 			{
// 				var childRelation = relations.Get(currentChild);
// 				var nextChild = childRelation.NextSiblingId;
//
// 				action.Apply(currentChild);
//
// 				if (childRelation.FirstChildId != Constants.InvalidId)
// 				{
// 					new RelationView(World, currentChild).ForEach(ref action);
// 				}
//
// 				currentChild = nextChild;
// 			}
// 		}
// 	}
// }
