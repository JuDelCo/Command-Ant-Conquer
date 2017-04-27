using AntWars.Bot;

namespace AntWars
{
	public enum PlayerAction
	{
		SpawnEgg,
		OrderAttack,
		OrderStealEgg,
		OrderHarvestFood,
		OrderPatrol,
		OrderRecover
	}

	public class Player
	{
		public float lastEggTime;
		public PlayerBehaviour behaviour;
		public IA ia;
	}
}
