using UnityEngine;

namespace AntWars
{
	public class FoodPileBehaviour : MonoBehaviour
	{
		[HideInInspector] public FoodPile foodPileData;

		protected global::FoodPile foodPile;

		protected void Start()
		{
			foodPile = GetComponent<global::FoodPile>();
		}

		public int GetFoodCount()
		{
			return foodPile.currentFoodAmount;
		}

		public bool GetFoodUnit()
		{
			if(GetFoodCount() > 0)
			{
				foodPile.currentFoodAmount -= 1;

				return true;
			}

			return false;
		}
	}
}
