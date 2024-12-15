namespace Massive.Hierarchy
{
	public static class RegistryHierarchyExtensions
	{
		public static void AssignChild(this Registry registry, Entity parent, Entity child)
		{
			if (!registry.IsAlive(parent) || !registry.IsAlive(child))
			{
				return;
			}

			var relationships = registry.DataSet<Relationship>();

			relationships.Assign(parent.Id);
			relationships.Assign(child.Id);

			ref var parentRelationship = ref relationships.Get(parent.Id);
			ref var childRelationship = ref relationships.Get(child.Id);

			if (childRelationship.Parent == parent)
			{
				return;
			}
			else if (childRelationship.Parent != parent && childRelationship.Parent != Entity.Dead)
			{
				registry.UnassignChild(childRelationship.Parent, child);
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
					currentChild = relationships.Get(currentChild.Id).NextSibling;
				}
				relationships.Get(currentChild.Id).NextSibling = child;
				childRelationship.PrevSibling = currentChild;
			}
		}

		private static void UnassignParent(this Registry registry, Entity child)
		{
			registry.UnassignChild(registry.Get<Relationship>(child).Parent, child);
		}

		private static void UnassignChild(this Registry registry, Entity parent, Entity child)
		{
			var relationships = registry.DataSet<Relationship>();

			ref var parentRelationship = ref relationships.Get(parent.Id);

			var currentChild = parentRelationship.FirstChild;
			var children = parentRelationship.Children;
			for (int i = 0; i < children; i++)
			{
				var currentRelationship = relationships.Get(currentChild.Id);
				if (currentChild == child)
				{
					if (currentRelationship.PrevSibling == Entity.Dead)
					{
						parentRelationship.FirstChild = currentRelationship.NextSibling;
						relationships.Get(currentRelationship.NextSibling.Id).PrevSibling = Entity.Dead;
					}
					else
					{
						relationships.Get(currentRelationship.PrevSibling.Id).NextSibling = currentRelationship.NextSibling;
						relationships.Get(currentRelationship.NextSibling.Id).PrevSibling = currentRelationship.PrevSibling;
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
