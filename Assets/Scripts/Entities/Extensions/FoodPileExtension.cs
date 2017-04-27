using UnityEngine;

namespace AntWars
{
	public static class FoodPileExtension
	{
		public static Vector3 GetPosition(this FoodPile foodPile)
		{
			return foodPile.behaviour.transform.position;
		}
	}
}
