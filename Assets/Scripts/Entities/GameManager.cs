using System.Collections.Generic;

namespace AntWars
{
	public delegate void PlayerActionEvent(Player player, PlayerAction action, Soldier relatedSoldier);
	public delegate void SoldierChangeStateEvent(Soldier soldier, SoldierState previousState);
	public delegate void SoldierCreationEvent(Soldier soldier);
	public delegate void SoldierDestroyEvent(Soldier soldier);
	public delegate void FoodPickupEvent(Soldier soldier);
	public delegate void FoodDropEvent(Soldier soldier);
	public delegate void AddFoodInLairEvent(Soldier soldier, Lair lair);
	public delegate void EggPickupEvent(Soldier soldier, SoldierEgg egg);
	public delegate void EggDropEvent(Soldier soldier, SoldierEgg egg);
	public delegate void GameFinishEvent(Player player);

	public class GameManager
	{
		public List<Player> players;
		public List<Soldier> soldiers;
		public List<SoldierEgg> eggs;
		public List<Lair> lairs;
		public List<FoodPile> foodPiles;
		public List<FoodUnit> foodUnits;

		public Prefabs prefabs;

		public PlayerActionEvent onPlayerAction;
		public SoldierChangeStateEvent onSoldierChangeState;
		public SoldierCreationEvent onSoldierCreate;
		public SoldierDestroyEvent onSoldierDestroy;
		public FoodPickupEvent onFoodPickup;
		public FoodPickupEvent onFoodDrop;
		public AddFoodInLairEvent onAddFoodInLair;
		public EggPickupEvent onEggPickup;
		public EggDropEvent onEggDrop;
		public GameFinishEvent onGameFinish;
	}
}
