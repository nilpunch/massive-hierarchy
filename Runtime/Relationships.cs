namespace Massive.Hierarchy
{
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

				var children = relationship.Children;
				var currentChild = relationship.FirstChild;
				for (int i = 0; i < children; i++)
				{
					ref var childRelationship = ref _relationships.Get(currentChild.Id);
					currentChild = childRelationship.NextSibling;
					childRelationship.PrevSibling = Entity.Dead;
					childRelationship.NextSibling = Entity.Dead;
					childRelationship.Parent = Entity.Dead;
				}

				relationship = default;
			}
		}

		public void AssignChild(Entity parent, Entity child)
		{
			_relationships.Assign(parent.Id);
			_relationships.Assign(child.Id);

			ref var parentRelationship = ref _relationships.Get(parent.Id);
			ref var childRelationship = ref _relationships.Get(child.Id);

			if (childRelationship.Parent == parent)
			{
				return;
			}

			if (childRelationship.Parent != parent && childRelationship.Parent != Entity.Dead)
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
				var children = parentRelationship.Children;
				for (int i = 0; i < children - 1; i++)
				{
					currentChild = _relationships.Get(currentChild.Id).NextSibling;
				}
				_relationships.Get(currentChild.Id).NextSibling = child;
				childRelationship.PrevSibling = currentChild;
			}
		}

		public void UnassignChild(Entity parent, Entity child)
		{
			_relationships.Assign(parent.Id);
			_relationships.Assign(child.Id);

			ref var parentRelationship = ref _relationships.Get(parent.Id);

			var currentChild = parentRelationship.FirstChild;
			var children = parentRelationship.Children;
			for (int i = 0; i < children; i++)
			{
				var currentRelationship = _relationships.Get(currentChild.Id);
				if (currentChild == child)
				{
					if (currentRelationship.PrevSibling == Entity.Dead)
					{
						parentRelationship.FirstChild = currentRelationship.NextSibling;
						_relationships.Get(currentRelationship.NextSibling.Id).PrevSibling = Entity.Dead;
					}
					else
					{
						_relationships.Get(currentRelationship.PrevSibling.Id).NextSibling = currentRelationship.NextSibling;
						_relationships.Get(currentRelationship.NextSibling.Id).PrevSibling = currentRelationship.PrevSibling;
					}
					currentRelationship.PrevSibling = Entity.Dead;
					currentRelationship.NextSibling = Entity.Dead;
					currentRelationship.Parent = Entity.Dead;
					parentRelationship.Children -= 1;
					return;
				}
				currentChild = currentRelationship.NextSibling;
			}
		}
	}
}
