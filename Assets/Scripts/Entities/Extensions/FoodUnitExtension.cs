using Atto;
using UnityEngine;

namespace AntWars
{
	public static class FoodUnitExtension
	{
		public static Vector3 GetPosition(this FoodUnit foodUnit)
		{
			return foodUnit.behaviour.transform.position;
		}

		public static void Instantiate(Vector3 position)
		{
			var foodUnit = new FoodUnit();

			var gameManager = Core.Get<GameManager>();

			gameManager.foodUnits.Add(foodUnit);

			foodUnit.behaviour = GameObject.Instantiate(gameManager.prefabs.foodUnitPrefab, position, Quaternion.identity).GetComponent<FoodUnitBehaviour>();
			foodUnit.behaviour.foodUnitData = foodUnit;
		}

		public static void Destroy(this FoodUnit foodUnit)
		{
			var gameManager = Core.Get<GameManager>();
			gameManager.foodUnits.Remove(foodUnit);
			GameObject.Destroy(foodUnit.behaviour.gameObject);

			foreach (var it in gameManager.soldiers)
			{
				if(it.harvestUnitTarget == foodUnit)
				{
					it.harvestUnitTarget = null;

					if(it.state == SoldierState.Harvest && it.harvestPileTarget == null && !it.hasFood)
					{
						it.state = SoldierState.Protect;

						if (gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(it, SoldierState.Harvest);
						}
					}
				}
			}
		}
	}
}
