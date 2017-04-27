using Atto;
using System.Collections.Generic;
using UnityEngine;

namespace AntWars
{
	public static class PlayerExtension
	{
		public static Vector3 GetPosition(this Player player)
		{
			return player.behaviour.transform.position;
		}

		public static void Update(this Player player)
		{
			if(player.ia == null)
			{
				player.behaviour.Move();
			}
			else
			{
				player.ia.Update();
			}

			player.PlayerLogic();
		}

		public static bool IsActionButtonPressed(this Player player)
		{
			if(player.ia == null)
			{
				return (player.behaviour.GetRecoverButtonStatus() == ButtonState.Pressed || player.behaviour.GetActionButtonStatus() == ButtonState.Pressed);
			}
			else
			{
				return player.ia.UseAction();
			}
		}

		public static void PlayerLogic(this Player player)
		{
			var gameManager = Core.Get<GameManager>();

			{
				var playerLair = gameManager.GetPlayerLair(player);

				var followingSoldiers = new List<Soldier>();
				var nearEnemySoldiers = new List<Soldier>();
				var nearFoodPiles = new List<FoodPile>();
				var nearFoodUnits = new List<FoodUnit>();
				var nearEnemySoldierEggs = new List<SoldierEgg>();

				foreach (var soldier in gameManager.GetPlayerSoldiers(player))
				{
					if(soldier.state == SoldierState.Protect)
					{
						followingSoldiers.Add(soldier);
					}
				}

				foreach (var soldier in gameManager.soldiers)
				{
					if(soldier.owner != player && Vector3.Distance(player.GetPosition(), soldier.GetPosition()) < SoldierConstants.ATTACK_AGRO_MIN_DISTANCE)
					{
						nearEnemySoldiers.Add(soldier);
					}
				}

				foreach (var foodPile in gameManager.foodPiles)
				{
					if(foodPile.behaviour.GetFoodCount() == 0)
					{
						continue;
					}

					if(Vector3.Distance(player.GetPosition(), foodPile.GetPosition()) < SoldierConstants.FOOD_HARVEST_MIN_DISTANCE)
					{
						nearFoodPiles.Add(foodPile);
					}
				}

				foreach (var foodUnit in gameManager.foodUnits)
				{
					if(Vector3.Distance(player.GetPosition(), foodUnit.GetPosition()) < SoldierConstants.FOOD_HARVEST_MIN_DISTANCE)
					{
						nearFoodUnits.Add(foodUnit);
					}
				}

				foreach (var egg in gameManager.eggs)
				{
					bool isBeingStolen = false;

					foreach (var soldier in gameManager.soldiers)
					{
						if(soldier.state == SoldierState.StealEgg && soldier.stealEggTarget == egg)
						{
							isBeingStolen = true;
							break;
						}
					}

					if(isBeingStolen)
					{
						continue;
					}

					if(!egg.behaviour.gameObject.activeInHierarchy)
					{
						continue;
					}

					bool inPlayerLair = Vector3.Distance(egg.GetPosition(), playerLair.GetPosition()) < SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR;

					if(!inPlayerLair && Vector3.Distance(player.GetPosition(), egg.GetPosition()) < SoldierConstants.EGG_STEAL_MIN_DISTANCE)
					{
						nearEnemySoldierEggs.Add(egg);
					}
				}

				if(nearEnemySoldiers.Count > 0)
				{
					foreach (var soldier in followingSoldiers)
					{
						if(gameManager.onPlayerAction != null)
						{
							gameManager.onPlayerAction(player, PlayerAction.OrderAttack, soldier);
						}

						soldier.state = SoldierState.Attack;
						soldier.attackTarget = nearEnemySoldiers[0];

						if(gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Protect);
						}
					}
				}
				else if(nearFoodPiles.Count > 0)
				{
					foreach (var soldier in followingSoldiers)
					{
						if(gameManager.onPlayerAction != null)
						{
							gameManager.onPlayerAction(player, PlayerAction.OrderHarvestFood, soldier);
						}

						soldier.state = SoldierState.Harvest;
						soldier.harvestPileTarget = nearFoodPiles[0];

						if(gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Protect);
						}
					}
				}
				else if(nearFoodUnits.Count > 0)
				{
					foreach (var soldier in followingSoldiers)
					{
						if(gameManager.onPlayerAction != null)
						{
							gameManager.onPlayerAction(player, PlayerAction.OrderHarvestFood, soldier);
						}

						soldier.state = SoldierState.Harvest;
						soldier.harvestPileTarget = null;
						soldier.harvestUnitTarget = nearFoodUnits[0];

						if(gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Protect);
						}
					}
				}
				else if(nearEnemySoldierEggs.Count > 0)
				{
					foreach (var soldier in followingSoldiers)
					{
						if(gameManager.onPlayerAction != null)
						{
							gameManager.onPlayerAction(player, PlayerAction.OrderStealEgg, soldier);
						}

						soldier.state = SoldierState.StealEgg;
						soldier.stealEggTarget = nearEnemySoldierEggs[0];

						if(gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Protect);
						}
					}
				}
				else if(Vector3.Distance(playerLair.GetPosition(), player.GetPosition()) < PlayerConstants.LAIR_EGG_CREATION_MIN_DISTANCE)
				{
					if(player.lastEggTime + PlayerConstants.EGG_DELAY_TIME <= Time.fixedTime && playerLair.foodCount >= PlayerConstants.FOOD_SOLDIER_COST && gameManager.GetPlayerSoldiers(player).Count + gameManager.GetPlayerEggs(player).Count < PlayerConstants.MAX_SOLDIERS)
					{
						player.lastEggTime = Time.fixedTime;

						if(gameManager.onPlayerAction != null)
						{
							gameManager.onPlayerAction(player, PlayerAction.SpawnEgg, null);
						}

						playerLair.foodCount -= PlayerConstants.FOOD_SOLDIER_COST;

						SoldierEggExtension.Instantiate(playerLair.GetPosition() + new Vector3(
							Random.Range(-(SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR / 2.2f), (SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR / 2.2f)),
							0,
							Random.Range(-(SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR / 2.2f), (SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR / 2.2f))
						));
					}
				}
			}

			if(player.IsActionButtonPressed())
			{
				var soldiers = gameManager.GetPlayerSoldiers(player);
				var soldiersNotGuarding = new List<Soldier>();

				foreach (var soldier in soldiers)
				{
					if(soldier.state != SoldierState.Protect)
					{
						soldiersNotGuarding.Add(soldier);
					}
				}

				if(soldiersNotGuarding.Count > 0)
				{
					foreach (var soldier in soldiersNotGuarding)
					{
						if(Vector3.Distance(soldier.GetPosition(), player.GetPosition()) <= PlayerConstants.SOLDIER_RECOVER_MIN_DISTANCE)
						{
							if(gameManager.onPlayerAction != null)
							{
								gameManager.onPlayerAction(player, PlayerAction.OrderRecover, soldier);
							}

							var previousState = soldier.state;

							soldier.state = SoldierState.Protect;

							if(soldier.hasFood)
							{
								soldier.hasFood = false;
								soldier.behaviour.CarryNone();
								FoodUnitExtension.Instantiate(soldier.GetPosition());

								if(gameManager.onFoodDrop != null)
								{
									gameManager.onFoodDrop(soldier);
								}
							}

							if(soldier.hasEgg)
							{
								soldier.hasEgg = false;
								soldier.behaviour.CarryNone();
								soldier.stealEggTarget.behaviour.transform.position = soldier.GetPosition();
								soldier.stealEggTarget.behaviour.gameObject.SetActive(true);

								if(gameManager.onEggDrop != null)
								{
									gameManager.onEggDrop(soldier, soldier.stealEggTarget);
								}

								soldier.stealEggTarget = null;
							}

							if (gameManager.onSoldierChangeState != null)
							{
								gameManager.onSoldierChangeState(soldier, previousState);
							}
						}
					}
				}
			}
		}
	}
}
