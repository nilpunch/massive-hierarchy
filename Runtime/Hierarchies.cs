using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Hierarchies
	{
		private Entities Entities { get; }

		public DataSet<Hierarchy> Components { get; }

		public ListAllocator<Entity> Allocator { get; }

		public Hierarchies(DataSet<Hierarchy> components, Entities entities, ListAllocator<Entity> allocator)
		{
			Components = components;
			Entities = entities;
			Allocator = allocator;

			Components.BeforeRemoved += RemoveHierarchy;

			void RemoveHierarchy(int id)
			{
				ref var hierarchy = ref Components.Get(id);

				if (hierarchy.Parent != Entity.Dead)
				{
					RemoveChild(hierarchy.Parent, Entities.GetEntity(id));
				}

				var childs = hierarchy.Childs.In(Allocator);

				foreach (var child in childs)
				{
					ref var childHierarchy = ref Components.Get(child.Id);
					childHierarchy.Parent = Entity.Dead;
				}

				childs.Free();
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AddHierarchy(Entity entity)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, entity);

			if (Components.Add(entity.Id))
			{
				Components.Get(entity.Id).Childs = Allocator.AllocList();
				return true;
			}

			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AddChild(Entity parent, Entity child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			ref var childHierarchy = ref Components.Get(parent.Id);

			if (childHierarchy.Parent == parent)
			{
				return;
			}

			if (childHierarchy.Parent != Entity.Dead)
			{
				RemoveChild(childHierarchy.Parent, child);
			}

			childHierarchy.Parent = parent;

			Components.Get(parent.Id).Childs.In(Allocator).Add(child);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveChild(Entity parent, Entity child)
		{
			EntityNotAliveException.ThrowIfEntityDead(Entities, parent);
			EntityNotAliveException.ThrowIfEntityDead(Entities, child);

			var childs = Components.Get(parent.Id).Childs.In(Allocator);

			if (childs.Remove(child))
			{
				Components.Get(child.Id).Parent = Entity.Dead;
			}
		}
	}
}
