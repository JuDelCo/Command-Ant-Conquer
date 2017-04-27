using UnityEngine;

namespace AntWars
{
	public enum SoldierState
	{
		Protect,
		Attack,
		Harvest,
		StealEgg
	}

	public class Soldier
	{
		public Player owner;
		public float health;
		public Vector3 targetPosition;
		public SoldierState state;
		public SoldierBehaviour behaviour;

		public Soldier attackTarget;
		public SoldierEgg stealEggTarget;
		public bool hasEgg;
		public FoodPile harvestPileTarget;
		public FoodUnit harvestUnitTarget;
		public bool hasFood;
		public bool isDead;
	}
}
