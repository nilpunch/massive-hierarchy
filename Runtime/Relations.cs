using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Relations
	{
		private Entities Entities { get; }

		public DataSet<Relation> Set { get; }

		public Relations(DataSet<Relation> relationships, Entities entities)
		{
			Set = relationships;
			Entities = entities;

			Set.BeforeUnassigned += UnassignRelationship;

			void UnassignRelationship(int id)
			{
				ref var relationship = ref Set.Get(id);

				if (relationship.ParentId != Constants.InvalidId)
				{
					UnassignChild(relationship.ParentId, id);
				}

				var currentChild = relationship.FirstChildId;
				while (currentChild != Constants.InvalidId)
				{
					ref var childRelationship = ref Set.Get(currentChild);
					currentChild = childRelationship.NextSiblingId;
					childRelationship.PrevSiblingId = Constants.InvalidId;
					childRelationship.NextSiblingId = Constants.InvalidId;
					childRelationship.ParentId = Constants.InvalidId;
				}
			}
		}

		/// <summary>
		/// Assigns child as first. Faster then <see cref="AssignChildAsLast"/>.
		/// </summary>
		public void AssignChild(int parent, int child)
		{
			Assert.IsAlive(Entities, parent);
			Assert.IsAlive(Entities, child);

			ref var parentRelationship = ref Set.Get(parent);
			ref var childRelationship = ref Set.Get(child);

			if (childRelationship.ParentId == parent)
			{
				return;
			}

			if (childRelationship.ParentId != Constants.InvalidId)
			{
				UnassignChild(childRelationship.ParentId, child);
			}

			childRelationship.ParentId = parent;

			var nextChild = parentRelationship.FirstChildId;
			parentRelationship.FirstChildId = child;
			childRelationship.NextSiblingId = nextChild;
		}

		public void AssignChildAsLast(int parent, int child)
		{
			Assert.IsAlive(Entities, parent);
			Assert.IsAlive(Entities, child);

			ref var parentRelationship = ref Set.Get(parent);
			ref var childRelationship = ref Set.Get(child);

			if (childRelationship.ParentId == parent)
			{
				return;
			}

			if (childRelationship.ParentId != Constants.InvalidId)
			{
				UnassignChild(childRelationship.ParentId, child);
			}

			childRelationship.ParentId = parent;

			if (parentRelationship.FirstChildId == Constants.InvalidId)
			{
				parentRelationship.FirstChildId = child;
			}
			else
			{
				var currentChild = parentRelationship.FirstChildId;
				ref var currentRelationship = ref Set.Get(currentChild);
				while (currentRelationship.NextSiblingId != Constants.InvalidId)
				{
					currentChild = currentRelationship.NextSiblingId;
					currentRelationship = ref Set.Get(currentChild);
				}
				currentRelationship.NextSiblingId = child;
				childRelationship.PrevSiblingId = currentChild;
			}
		}

		public void UnassignChild(int parent, int child)
		{
			Assert.IsAlive(Entities, parent);
			Assert.IsAlive(Entities, child);

			ref var childRelationship = ref Set.Get(child);

			if (childRelationship.ParentId != parent)
			{
				return;
			}

			if (childRelationship.PrevSiblingId != Constants.InvalidId)
			{
				Set.Get(childRelationship.PrevSiblingId).NextSiblingId = childRelationship.NextSiblingId;
			}
			else
			{
				Set.Get(parent).FirstChildId = childRelationship.NextSiblingId;
			}

			if (childRelationship.NextSiblingId != Constants.InvalidId)
			{
				Set.Get(childRelationship.NextSiblingId).PrevSiblingId = childRelationship.PrevSiblingId;
			}

			childRelationship.PrevSiblingId = Constants.InvalidId;
			childRelationship.NextSiblingId = Constants.InvalidId;
			childRelationship.ParentId = Constants.InvalidId;
		}

		public int GetLastChild(int parent)
		{
			Assert.IsAlive(Entities, parent);
			
			ref var relationship = ref Set.Get(parent);

			if (relationship.FirstChildId == Constants.InvalidId)
			{
				return Constants.InvalidId;
			}

			var child = relationship.FirstChildId;
			ref var childRelationship = ref Set.Get(relationship.FirstChildId);

			while (childRelationship.NextSiblingId != Constants.InvalidId)
			{
				child = childRelationship.NextSiblingId;
				childRelationship = ref Set.Get(child);
			}

			return child;
		}
	}
}
