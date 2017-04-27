using UnityEngine;

namespace AntWars
{
	public static class LairExtension
	{
		public static Vector3 GetPosition(this Lair lair)
		{
			return lair.behaviour.transform.position;
		}
	}
}
