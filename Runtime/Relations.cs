using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Relations
	{
		private Entities Entities { get; }

		public DataSet<Relation> DataSet { get; }

		public Relations(DataSet<Relation> relationships, Entities entities)
		{
			DataSet = relationships;
			Entities = entities;

			DataSet.BeforeRemoved += RemoveRelationship;

			void RemoveRelationship(int id)
			{
				ref var relationship = ref DataSet.Get(id);

				if (relationship.ParentId != Constants.InvalidId)
				{
					RemoveChild(relationship.ParentId, id);
				}

				var currentChild = relationship.FirstChildId;
				while (currentChild != Constants.InvalidId)
				{
					ref var childRelationship = ref DataSet.Get(currentChild);
					currentChild = childRelationship.NextSiblingId;
					childRelationship.PrevSiblingId = Constants.InvalidId;
					childRelationship.NextSiblingId = Constants.InvalidId;
					childRelationship.ParentId = Constants.InvalidId;
				}
			}
		}

		/// <summary>
		/// Adds child as first. Faster then <see cref="AddChildAsLast"/>.
		/// </summary>
		public void AddChild(int parent, int child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			ref var parentRelationship = ref DataSet.Get(parent);
			ref var childRelationship = ref DataSet.Get(child);

			if (childRelationship.ParentId == parent)
			{
				return;
			}

			if (childRelationship.ParentId != Constants.InvalidId)
			{
				RemoveChild(childRelationship.ParentId, child);
			}

			childRelationship.ParentId = parent;

			var nextChild = parentRelationship.FirstChildId;
			parentRelationship.FirstChildId = child;
			childRelationship.NextSiblingId = nextChild;
		}

		public void AddChildAsLast(int parent, int child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			ref var parentRelationship = ref DataSet.Get(parent);
			ref var childRelationship = ref DataSet.Get(child);

			if (childRelationship.ParentId == parent)
			{
				return;
			}

			if (childRelationship.ParentId != Constants.InvalidId)
			{
				RemoveChild(childRelationship.ParentId, child);
			}

			childRelationship.ParentId = parent;

			if (parentRelationship.FirstChildId == Constants.InvalidId)
			{
				parentRelationship.FirstChildId = child;
			}
			else
			{
				var currentChild = parentRelationship.FirstChildId;
				ref var currentRelationship = ref DataSet.Get(currentChild);
				while (currentRelationship.NextSiblingId != Constants.InvalidId)
				{
					currentChild = currentRelationship.NextSiblingId;
					currentRelationship = ref DataSet.Get(currentChild);
				}
				currentRelationship.NextSiblingId = child;
				childRelationship.PrevSiblingId = currentChild;
			}
		}

		public void RemoveChild(int parent, int child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			ref var childRelationship = ref DataSet.Get(child);

			if (childRelationship.ParentId != parent)
			{
				return;
			}

			if (childRelationship.PrevSiblingId != Constants.InvalidId)
			{
				DataSet.Get(childRelationship.PrevSiblingId).NextSiblingId = childRelationship.NextSiblingId;
			}
			else
			{
				DataSet.Get(parent).FirstChildId = childRelationship.NextSiblingId;
			}

			if (childRelationship.NextSiblingId != Constants.InvalidId)
			{
				DataSet.Get(childRelationship.NextSiblingId).PrevSiblingId = childRelationship.PrevSiblingId;
			}

			childRelationship.PrevSiblingId = Constants.InvalidId;
			childRelationship.NextSiblingId = Constants.InvalidId;
			childRelationship.ParentId = Constants.InvalidId;
		}

		public int GetLastChild(int parent)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			
			ref var relationship = ref DataSet.Get(parent);

			if (relationship.FirstChildId == Constants.InvalidId)
			{
				return Constants.InvalidId;
			}

			var child = relationship.FirstChildId;
			ref var childRelationship = ref DataSet.Get(relationship.FirstChildId);

			while (childRelationship.NextSiblingId != Constants.InvalidId)
			{
				child = childRelationship.NextSiblingId;
				childRelationship = ref DataSet.Get(child);
			}

			return child;
		}
	}
}
