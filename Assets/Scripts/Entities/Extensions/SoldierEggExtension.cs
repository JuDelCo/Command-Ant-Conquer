using Atto;
using UnityEngine;

namespace AntWars
{
	public static class SoldierEggExtension
	{
		public static Vector3 GetPosition(this SoldierEgg egg)
		{
			return egg.behaviour.transform.position;
		}

		public static void Instantiate(Vector3 position)
		{
			var egg = new SoldierEgg();
			egg.hatchProgress = 0f;

			var gameManager = Core.Get<GameManager>();

			gameManager.eggs.Add(egg);

			egg.behaviour = GameObject.Instantiate(gameManager.prefabs.soldierEggPrefab, position, Quaternion.identity).GetComponent<SoldierEggBehaviour>();
			egg.behaviour.eggData = egg;
		}

		public static void Update(this SoldierEgg egg)
		{
			if(!egg.behaviour.gameObject.activeInHierarchy)
			{
				return;
			}

			var nearestLair = egg.GetNearestLair();

			if(Vector3.Distance(nearestLair.GetPosition(), egg.GetPosition()) < SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR)
			{
				egg.hatchProgress += Time.deltaTime;

				if(egg.hatchProgress > SoldierEggConstants.HATCH_TIME)
				{
					SoldierExtension.Instantiate(nearestLair.owner, egg.GetPosition());
					egg.Destroy();
				}
			}
		}

		public static void Destroy(this SoldierEgg egg)
		{
			var gameManager = Core.Get<GameManager>();
			gameManager.eggs.Remove(egg);
			GameObject.Destroy(egg.behaviour.gameObject);

			foreach (var it in gameManager.soldiers)
			{
				if(it.stealEggTarget == egg)
				{
					it.stealEggTarget = null;
					it.hasEgg = false;

					if(it.state == SoldierState.StealEgg)
					{
						it.state = SoldierState.Protect;

						if (gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(it, SoldierState.StealEgg);
						}
					}
				}
			}
		}

		public static Lair GetNearestLair(this SoldierEgg egg)
		{
			Lair nearestLair = null;

			foreach (var lair in Core.Get<GameManager>().lairs)
			{
				if(nearestLair == null || Vector3.Distance(lair.GetPosition(), egg.GetPosition()) < Vector3.Distance(nearestLair.GetPosition(), egg.GetPosition()))
				{
					nearestLair = lair;
				}
			}

			return nearestLair;
		}
	}
}
