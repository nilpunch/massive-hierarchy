using Unity.IL2CPP.CompilerServices;

namespace Massive.Hierarchy
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class Relationships
	{
		private readonly DataSet<Relationship> _relationships;

		public Relationships(Registry registry)
		{
			_relationships = registry.DataSet<Relationship>();

			var entities = registry.Entities;
			_relationships.BeforeUnassigned += UnassignRelationship;

			void UnassignRelationship(int id)
			{
				ref var relationship = ref _relationships.Get(id);

				if (relationship.Parent != Entity.Dead)
				{
					UnassignChild(relationship.Parent, entities.GetEntity(id));
				}

				var currentChild = relationship.FirstChild;
				while (currentChild != Entity.Dead)
				{
					ref var childRelationship = ref _relationships.Get(currentChild.Id);
					currentChild = childRelationship.NextSibling;
					childRelationship.PrevSibling = Entity.Dead;
					childRelationship.NextSibling = Entity.Dead;
					childRelationship.Parent = Entity.Dead;
				}
			}
		}

		/// <summary>
		/// Assigns child as first. Faster then <see cref="AssignChildAsLast"/>.
		/// </summary>
		public void AssignChild(Entity parent, Entity child)
		{
			ref var parentRelationship = ref _relationships.Get(parent.Id);
			ref var childRelationship = ref _relationships.Get(child.Id);

			if (childRelationship.Parent == parent)
			{
				return;
			}

			if (childRelationship.Parent != Entity.Dead)
			{
				UnassignChild(childRelationship.Parent, child);
			}

			childRelationship.Parent = parent;

			var nextChild = parentRelationship.FirstChild;
			parentRelationship.FirstChild = child;
			childRelationship.NextSibling = nextChild;
		}

		public void AssignChildAsLast(Entity parent, Entity child)
		{
			ref var parentRelationship = ref _relationships.Get(parent.Id);
			ref var childRelationship = ref _relationships.Get(child.Id);

			if (childRelationship.Parent == parent)
			{
				return;
			}

			if (childRelationship.Parent != Entity.Dead)
			{
				UnassignChild(childRelationship.Parent, child);
			}

			childRelationship.Parent = parent;

			if (parentRelationship.FirstChild == Entity.Dead)
			{
				parentRelationship.FirstChild = child;
			}
			else
			{
				var currentChild = parentRelationship.FirstChild;
				ref var currentRelationship = ref _relationships.Get(currentChild.Id);
				while (currentRelationship.NextSibling != Entity.Dead)
				{
					currentChild = currentRelationship.NextSibling;
					currentRelationship = ref _relationships.Get(currentChild.Id);
				}
				currentRelationship.NextSibling = child;
				childRelationship.PrevSibling = currentChild;
			}
		}

		public void UnassignChild(Entity parent, Entity child)
		{
			ref var childRelationship = ref _relationships.Get(child.Id);

			if (childRelationship.Parent != parent)
			{
				return;
			}

			if (childRelationship.PrevSibling != Entity.Dead)
			{
				_relationships.Get(childRelationship.PrevSibling.Id).NextSibling = childRelationship.NextSibling;
			}
			else
			{
				_relationships.Get(parent.Id).FirstChild = childRelationship.NextSibling;
			}

			if (childRelationship.NextSibling != Entity.Dead)
			{
				_relationships.Get(childRelationship.NextSibling.Id).PrevSibling = childRelationship.PrevSibling;
			}

			childRelationship.PrevSibling = Entity.Dead;
			childRelationship.NextSibling = Entity.Dead;
			childRelationship.Parent = Entity.Dead;
		}

		public Entity GetLastChild(Entity parent)
		{
			ref var relationship = ref _relationships.Get(parent.Id);

			if (relationship.FirstChild == Entity.Dead)
			{
				return Entity.Dead;
			}

			var child = relationship.FirstChild;
			ref var childRelationship = ref _relationships.Get(relationship.FirstChild.Id);

			while (childRelationship.NextSibling != Entity.Dead)
			{
				child = childRelationship.NextSibling;
				childRelationship = ref _relationships.Get(child.Id);
			}

			return child;
		}
	}
}
