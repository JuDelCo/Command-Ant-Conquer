using UnityEngine;

namespace AntWars
{
	public class FoodUnitBehaviour : MonoBehaviour
	{
		[HideInInspector] public FoodUnit foodUnitData;

		public int GetFoodCount()
		{
			return 1;
		}
	}
}
