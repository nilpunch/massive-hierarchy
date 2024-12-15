using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Massive.Hierarchy
{
	public struct Relationship
	{
		public int Children;
		public Entity FirstChild;

		public Entity PrevSibling;
		public Entity NextSibling;

		public Entity Parent;
	}
}