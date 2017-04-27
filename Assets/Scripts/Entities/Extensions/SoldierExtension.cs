using Atto;
using System.Collections.Generic;
using UnityEngine;

namespace AntWars
{
    public static class SoldierExtension
    {
        public static Vector3 GetPosition(this Soldier soldier)
        {
            return soldier.behaviour.transform.position;
        }

        public static Soldier Instantiate(Player player, Vector3 position)
        {
            var soldier = new Soldier();
            soldier.owner = player;
            soldier.health = SoldierConstants.MAX_HEALTH;

            var gameManager = Core.Get<GameManager>();

            gameManager.soldiers.Add(soldier);

            soldier.behaviour = GameObject.Instantiate(gameManager.prefabs.soldierPrefab, position, Quaternion.identity).GetComponent<SoldierBehaviour>();
            soldier.behaviour.soldierData = soldier;

			soldier.state = SoldierState.Protect;
			soldier.attackTarget = null;
			soldier.stealEggTarget = null;
			soldier.hasEgg = false;
			soldier.harvestPileTarget = null;
			soldier.hasFood = false;
			soldier.isDead = false;

			if(gameManager.onSoldierCreate != null)
			{
				gameManager.onSoldierCreate(soldier);
			}

            return soldier;
        }

        public static void Update(this Soldier soldier)
        {
            var gameManager = Core.Get<GameManager>();

            var playerLair = gameManager.GetPlayerLair(soldier.owner);

            if (soldier.state == SoldierState.Harvest)
            {
				if(soldier.hasFood)
				{
					soldier.targetPosition = playerLair.GetPosition();

					if (Vector3.Distance(soldier.GetPosition(), playerLair.GetPosition()) < SoldierConstants.FOOD_HARVEST_MIN_DISTANCE)
                    {
						playerLair.foodCount += 1;

						if(gameManager.onAddFoodInLair != null)
						{
							gameManager.onAddFoodInLair(soldier, playerLair);
						}

                        soldier.hasFood = false;
						soldier.behaviour.CarryNone();

						if(gameManager.onFoodDrop != null)
						{
							gameManager.onFoodDrop(soldier);
						}
                    }
				}
				else
				{
					if(soldier.harvestUnitTarget != null && soldier.harvestPileTarget == null)
					{
						soldier.targetPosition = soldier.harvestUnitTarget.GetPosition();

						if (Vector3.Distance(soldier.GetPosition(), soldier.harvestUnitTarget.GetPosition()) < SoldierConstants.FOOD_HARVEST_MIN_DISTANCE)
						{
							soldier.hasFood = true;
							soldier.behaviour.CarryFood();
							soldier.harvestUnitTarget.Destroy();

							if(gameManager.onFoodPickup != null)
							{
								gameManager.onFoodPickup(soldier);
							}
						}
					}
					else if(soldier.harvestPileTarget != null)
					{
						soldier.targetPosition = soldier.harvestPileTarget.GetPosition();

						if(soldier.harvestPileTarget.behaviour.GetFoodCount() == 0)
						{
							soldier.harvestPileTarget = null;
						}
						else if (Vector3.Distance(soldier.GetPosition(), soldier.harvestPileTarget.GetPosition()) < SoldierConstants.FOOD_HARVEST_MIN_DISTANCE)
						{
							if(soldier.harvestPileTarget.behaviour.GetFoodUnit())
							{
								soldier.hasFood = true;
								soldier.behaviour.CarryFood();

								if(gameManager.onFoodPickup != null)
								{
									gameManager.onFoodPickup(soldier);
								}
							}
							else
							{
								soldier.harvestPileTarget = null;
							}
						}
					}
					else
					{
						FoodPile nearestFoodPile = null;
						FoodUnit nearestFoodUnit = null;

						foreach (var foodPile in gameManager.foodPiles)
						{
							if(foodPile.behaviour.GetFoodCount() == 0)
							{
								continue;
							}

							if(nearestFoodPile == null || Vector3.Distance(foodPile.GetPosition(), soldier.GetPosition()) < Vector3.Distance(nearestFoodPile.GetPosition(), soldier.GetPosition()))
							{
								nearestFoodPile = foodPile;
							}
						}

						foreach (var foodUnit in gameManager.foodUnits)
						{
							if(nearestFoodUnit == null || Vector3.Distance(foodUnit.GetPosition(), soldier.GetPosition()) < Vector3.Distance(nearestFoodUnit.GetPosition(), soldier.GetPosition()))
							{
								nearestFoodUnit = foodUnit;
							}
						}

						var goWithPlayer = false;
						var goWithFoodPile = false;
						var goWithFoodUnit = false;

						if(nearestFoodPile != null && nearestFoodUnit != null)
						{
							if(
								Vector3.Distance(soldier.GetPosition(), soldier.owner.GetPosition()) > Vector3.Distance(soldier.GetPosition(), nearestFoodPile.GetPosition()) ||
								Vector3.Distance(soldier.GetPosition(), soldier.owner.GetPosition()) > Vector3.Distance(soldier.GetPosition(), nearestFoodUnit.GetPosition()))
							{
								if(Vector3.Distance(soldier.GetPosition(), nearestFoodPile.GetPosition()) < Vector3.Distance(soldier.GetPosition(), nearestFoodUnit.GetPosition()))
								{
									goWithFoodPile = true;
								}
								else
								{
									goWithFoodUnit = true;
								}
							}
							else
							{
								goWithPlayer = true;
							}
						}
						else if(nearestFoodPile != null)
						{
							if(Vector3.Distance(soldier.GetPosition(), nearestFoodPile.GetPosition()) < Vector3.Distance(soldier.GetPosition(), soldier.owner.GetPosition()))
							{
								goWithFoodPile = true;
							}
							else
							{
								goWithPlayer = true;
							}
						}
						else if(nearestFoodUnit != null)
						{
							if(Vector3.Distance(soldier.GetPosition(), nearestFoodUnit.GetPosition()) < Vector3.Distance(soldier.GetPosition(), soldier.owner.GetPosition()))
							{
								goWithFoodUnit = true;
							}
							else
							{
								goWithPlayer = true;
							}
						}
						else
						{
							goWithPlayer = true;
						}

						if(goWithPlayer)
						{
							soldier.state = SoldierState.Protect;
							soldier.targetPosition = soldier.GetPosition();

							if (gameManager.onSoldierChangeState != null)
							{
								gameManager.onSoldierChangeState(soldier, SoldierState.Harvest);
							}
						}
						else if(goWithFoodPile)
						{
							soldier.state = SoldierState.Harvest;
							soldier.harvestPileTarget = nearestFoodPile;

							if(gameManager.onSoldierChangeState != null)
							{
								gameManager.onSoldierChangeState(soldier, SoldierState.Harvest);
							}
						}
						else if(goWithFoodUnit)
						{
							soldier.state = SoldierState.Harvest;
							soldier.harvestPileTarget = null;
							soldier.harvestUnitTarget = nearestFoodUnit;

							if(gameManager.onSoldierChangeState != null)
							{
								gameManager.onSoldierChangeState(soldier, SoldierState.Harvest);
							}
						}
					}
				}
            }
            else if (soldier.state == SoldierState.StealEgg)
            {
                if (soldier.stealEggTarget == null)
                {
                    soldier.state = SoldierState.Protect;
                    soldier.targetPosition = soldier.GetPosition();

                    if (gameManager.onSoldierChangeState != null)
                    {
                        gameManager.onSoldierChangeState(soldier, SoldierState.StealEgg);
                    }
                }
				else if(soldier.hasEgg)
				{
					soldier.targetPosition = playerLair.GetPosition();

					if (Vector3.Distance(soldier.GetPosition(), playerLair.GetPosition()) < (SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR / 1.3f))
                    {
						soldier.hasEgg = false;
						soldier.behaviour.CarryNone();
						soldier.stealEggTarget.behaviour.gameObject.SetActive(true);
						soldier.stealEggTarget.behaviour.transform.position = playerLair.GetPosition();

						if(gameManager.onEggDrop != null)
						{
							gameManager.onEggDrop(soldier, soldier.stealEggTarget);
						}

						soldier.stealEggTarget = null;
					}
				}
                else
                {
                    soldier.targetPosition = soldier.stealEggTarget.GetPosition();

					if(!soldier.stealEggTarget.behaviour.gameObject.activeInHierarchy)
					{
						soldier.stealEggTarget = null;
					}
					else if (Vector3.Distance(soldier.stealEggTarget.GetPosition(), soldier.GetPosition()) < SoldierConstants.EGG_STEAL_MIN_DISTANCE)
					{
						soldier.hasEgg = true;
						soldier.behaviour.CarryEgg();
						soldier.stealEggTarget.behaviour.gameObject.SetActive(false);

						if(gameManager.onEggPickup != null)
						{
							gameManager.onEggPickup(soldier, soldier.stealEggTarget);
						}
					}
                }
            }
            else if (soldier.state == SoldierState.Attack)
            {
                if (soldier.attackTarget == null)
                {
					var nearEnemySoldiers = new List<Soldier>();

					foreach (var enemySoldier in gameManager.soldiers)
					{
						if(enemySoldier.owner != soldier.owner && Vector3.Distance(soldier.GetPosition(), enemySoldier.GetPosition()) < SoldierConstants.ATTACK_AGRO_MIN_DISTANCE)
						{
							nearEnemySoldiers.Add(enemySoldier);
						}
					}

					if(nearEnemySoldiers.Count > 0)
					{
						soldier.state = SoldierState.Attack;
						soldier.attackTarget = nearEnemySoldiers[0];

						if(gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Attack);
						}
					}
					else
					{
						soldier.state = SoldierState.Protect;
						soldier.targetPosition = soldier.GetPosition();

						if (gameManager.onSoldierChangeState != null)
						{
							gameManager.onSoldierChangeState(soldier, SoldierState.Attack);
						}
					}
                }
                else if (Vector3.Distance(soldier.attackTarget.GetPosition(), soldier.GetPosition()) > SoldierConstants.ATTACK_DISTANCE)
                {
                    soldier.targetPosition = soldier.attackTarget.GetPosition();
                }
                else
                {
                    soldier.targetPosition = soldier.GetPosition();

                    soldier.attackTarget.health -= SoldierConstants.DAMAGE_RATE_SOLDIERS * Time.deltaTime;

					if(soldier.attackTarget.state == SoldierState.Attack || soldier.attackTarget.state == SoldierState.Protect)
					{
						soldier.health -= SoldierConstants.DAMAGE_RATE_SOLDIERS * Time.deltaTime;
					}

                    if (soldier.attackTarget.health <= 0)
                    {
						soldier.attackTarget.isDead = true;
                    }

					if(soldier.health <= 0)
					{
						soldier.isDead = true;
					}
                }
            }
            else // SoldierState.Protect
            {
                soldier.targetPosition = soldier.GetPlayerProtectPosition();

                foreach (var enemySoldier in gameManager.soldiers)
                {
                    if (enemySoldier.owner != soldier.owner)
                    {
                        if (Vector3.Distance(soldier.GetPosition(), enemySoldier.GetPosition()) <= (SoldierConstants.ATTACK_AGRO_MIN_DISTANCE / 1.5f))
                        {
                            soldier.state = SoldierState.Attack;
                            soldier.attackTarget = enemySoldier;

                            if (gameManager.onSoldierChangeState != null)
                            {
                                gameManager.onSoldierChangeState(soldier, SoldierState.Protect);
                            }

                            break;
                        }
                    }
                }
            }

            soldier.behaviour.MoveTo(soldier.targetPosition);
        }

        public static void Kill(this Soldier soldier)
        {
            var gameManager = Core.Get<GameManager>();

			if(gameManager.onSoldierDestroy != null)
			{
				gameManager.onSoldierDestroy(soldier);
			}

			if(soldier.hasFood)
			{
				FoodUnitExtension.Instantiate(soldier.GetPosition());

				if(gameManager.onFoodDrop != null)
				{
					gameManager.onFoodDrop(soldier);
				}
			}

			if(soldier.hasEgg)
			{
				soldier.stealEggTarget.behaviour.transform.position = soldier.GetPosition();
				soldier.stealEggTarget.behaviour.gameObject.SetActive(true);

				if(gameManager.onEggDrop != null)
				{
					gameManager.onEggDrop(soldier, soldier.stealEggTarget);
				}
			}

            gameManager.soldiers.Remove(soldier);
            GameObject.Destroy(soldier.behaviour.gameObject);

            foreach (var it in gameManager.soldiers)
            {
                if (it.attackTarget == soldier)
                {
                    it.attackTarget = null;

                    if (it.state == SoldierState.Attack)
                    {
                        it.state = SoldierState.Protect;

                        if (gameManager.onSoldierChangeState != null)
                        {
                            gameManager.onSoldierChangeState(it, SoldierState.Attack);
                        }
                    }
                }
            }
        }

        public static Vector3 GetPlayerProtectPosition(this Soldier soldier)
        {
			var gameManager = Core.Get<GameManager>();

			var followingSoldiers = new List<Soldier>();

			foreach (var it in gameManager.GetPlayerSoldiers(soldier.owner))
			{
				if(it.state == SoldierState.Protect)
				{
					followingSoldiers.Add(it);
				}
			}

            var soldierIndex = followingSoldiers.IndexOf(soldier) + 1;
			var multiplier = Mathf.Min(Mathf.Max(soldierIndex / 10, 1), 5);

            return soldier.owner.GetPosition() + new Vector3(
                Mathf.Sin(((360f / Mathf.Min(10, followingSoldiers.Count)) * soldierIndex) * Mathf.Deg2Rad) * (SoldierConstants.FOLLOW_PLAYER_DISTANCE * multiplier),
                0,
                Mathf.Cos(((360f / Mathf.Min(10, followingSoldiers.Count)) * soldierIndex) * Mathf.Deg2Rad) * (SoldierConstants.FOLLOW_PLAYER_DISTANCE * multiplier));
        }
    }
}
