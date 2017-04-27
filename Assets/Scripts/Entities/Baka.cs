using Atto;
using UnityEngine;

namespace AntWars.Bot
{
	public class Baka : IA
	{
		public GameManager game;
		public Player player;
		public Task currentTask;
		public bool firstBreed;

		public static Baka Instantiate(Player player)
		{
			var ia = new Baka();

			ia.player = player;
			ia.game = Core.Get<GameManager>();
			ia.firstBreed = false;

			ia.currentTask = new FreezeTask(5f);
			ia.currentTask.Initialize(ia);

			return ia;
		}

		public override void Update()
		{
			if(currentTask.IsFinished())
			{
				currentTask = GetNewTask();
				currentTask.Initialize(this);
			}

			var newPosition = currentTask.GetNewPosition();
			var distance = (GetCurrentPosition() - newPosition);

			if(distance.magnitude > 0.05f)
			{
				var direction = distance.normalized;
				player.behaviour.MoveTo(new Vector3(newPosition.x, 0, newPosition.y), new Vector2(direction.x, direction.y));
			}
			else
			{
				player.behaviour.MoveTo(player.GetPosition(), Vector2.zero);
			}
		}

		public override bool UseAction()
		{
			return currentTask.UseAction();
		}

		public Task GetNewTask()
		{
			Task newTask = null;

			if(!PlayerHasGuardingSoldiers())
			{
				if(!firstBreed)
				{
					firstBreed = true;
					var playerLair = game.GetPlayerLair(player);
					var playerLairPosition = new Vector2(playerLair.GetPosition().x, playerLair.GetPosition().z);
					newTask = new WaitTask(5f, playerLairPosition + new Vector2(Random.value > 0.5f ? -1f : 1f, Random.value > 0.5f ? -1f : 1f));
				}
				else if(game.GetPlayerLair(player).foodCount < PlayerConstants.FOOD_SOLDIER_COST)
				{
					if(game.GetPlayerSoldiers(player).Count == 0)
					{
						var playerLair = game.GetPlayerLair(player);
						var playerLairPosition = new Vector2(playerLair.GetPosition().x, playerLair.GetPosition().z);

						newTask = new WaitTask(Random.Range(2f, 3f), playerLairPosition + new Vector2(Random.value > 0.5f ? -1f : 1f, Random.value > 0.5f ? -1f : 1f));
					}
					else
					{
						if(game.GetPlayerSoldiers(player).Count > 20)
						{
							newTask = new RecruitTask(5, 10f);
						}
						else
						{
							newTask = new WaitTask(Random.Range(2f, 3f));
						}
					}
				}
				else
				{
					if(Random.value <= 0.9f)
					{
						newTask = new BreedTask(Mathf.Min(game.GetPlayerSoldiers(player).Count < 30 ? 6 : 4, Mathf.FloorToInt(game.GetPlayerLair(player).foodCount / PlayerConstants.FOOD_SOLDIER_COST)));
					}
					else
					{
						newTask = new RecruitTask(5, 10f);
					}
				}
			}
			else
			{
				if(game.GetPlayerSoldiers(player).Count > 30 && Random.value <= 0.3f)
				{
					newTask = new AttackTask();
				}
				else if(game.GetPlayerSoldiers(player).Count > 15 && Random.value <= 0.1f)
				{
					newTask = new AttackTask();
				}
				else
				{
					var foodPile = GetNearestBestFoodPile(GetCurrentPosition());

					if(foodPile.behaviour.GetFoodCount() < 10 && Random.value >= 0.8f)
					{
						newTask = new AttackTask();
					}
					else
					{
						newTask = new HarvestTask();
					}
				}
			}

			return newTask;
		}

		public Vector2 GetCurrentPosition()
		{
			var position = player.GetPosition();

			return new Vector2(position.x, position.z);
		}

		public FoodPile GetNearestBestFoodPile(Vector2 from)
		{
			var minFoodCount = 10;
			var fromPosition = new Vector3(from.x, 0, from.y);
			var nearestFoodPile = game.foodPiles[0];

			foreach (var foodPile in game.foodPiles)
			{
				if(nearestFoodPile.behaviour.GetFoodCount() < minFoodCount)
				{
					if(IsCloser(foodPile.GetPosition(), fromPosition, nearestFoodPile.GetPosition()) || foodPile.behaviour.GetFoodCount() >= minFoodCount)
					{
						nearestFoodPile = foodPile;
					}
				}
				else
				{
					if(IsCloser(foodPile.GetPosition(), fromPosition, nearestFoodPile.GetPosition()) && foodPile.behaviour.GetFoodCount() >= minFoodCount)
					{
						nearestFoodPile = foodPile;
					}
				}
			}

			return nearestFoodPile;
		}

		public FoodUnit GetNearestFoodUnit(Vector2 from)
		{
			if(game.foodUnits.Count == 0)
			{
				return null;
			}

			var fromPosition = new Vector3(from.x, 0, from.y);
			var nearestFoodUnit = game.foodUnits[0];

			foreach (var foodUnit in game.foodUnits)
			{
				if(IsCloser(foodUnit.GetPosition(), fromPosition, nearestFoodUnit.GetPosition()))
				{
					nearestFoodUnit = foodUnit;
				}
			}

			return nearestFoodUnit;
		}

		public Soldier GetNearestEnemy(Vector2 from)
		{
			Soldier nearestSoldier = null;

			var fromPosition = new Vector3(from.x, 0, from.y);

            foreach (var enemySoldier in game.soldiers)
            {
                if (enemySoldier.owner != player && !enemySoldier.isDead)
                {
					if(nearestSoldier == null || IsCloser(enemySoldier.GetPosition(), fromPosition, nearestSoldier.GetPosition()))
					{
						nearestSoldier = enemySoldier;
					}
				}
			}

			return nearestSoldier;
		}

		public int PlayerGuardingSoldiersCount()
		{
			var playerSoldiers = game.GetPlayerSoldiers(player);
			var soldiersFollowingCount = 0;

			foreach (var soldier in playerSoldiers)
			{
				if(soldier.state == SoldierState.Protect)
				{
					soldiersFollowingCount++;
				}
			}

			return soldiersFollowingCount;
		}

		public bool PlayerHasGuardingSoldiers()
		{
			return PlayerGuardingSoldiersCount() > 0;
		}

		public bool IsCloser(Vector3 thisThing, Vector3 fromThisPosition, Vector3 thanThisOtherThing)
		{
			return (Vector3.Distance(thisThing, fromThisPosition) < Vector3.Distance(thanThisOtherThing, fromThisPosition));
		}
	}

	public abstract class Task
	{
		public TaskType type;
		public abstract void Initialize(Baka ia);
		public abstract Vector2 GetNewPosition();
		public abstract bool UseAction();
		public abstract bool IsFinished();
	}

	public enum TaskType
	{
		Freeze,
		Wait,
		Harvest,
		Breed,
		Recruit,
		Attack
	}

	public class FreezeTask : Task
	{
		public float until;
		public Vector2 targetPosition;

		public FreezeTask(float seconds)
		{
			until = Time.fixedTime + seconds;
		}

		public override void Initialize(Baka ia)
		{
			type = TaskType.Freeze;
			targetPosition = ia.GetCurrentPosition();
		}

		public override Vector2 GetNewPosition()
		{
			return targetPosition;
		}

		public override bool UseAction()
		{
			return false;
		}

		public override bool IsFinished()
		{
			return (until <= Time.fixedTime);
		}
	}

	public class WaitTask : Task
	{
		public float until;
		public Vector2 startPosition;
		public Vector2 targetPosition;

		public WaitTask(float seconds)
		{
			until = Time.fixedTime + seconds;
		}

		public WaitTask(float seconds, Vector2 position) : this(seconds)
		{
			startPosition = position;
		}

		public override void Initialize(Baka ia)
		{
			type = TaskType.Wait;

			if(startPosition == Vector2.zero)
			{
				startPosition = ia.GetCurrentPosition();
			}

			RefreshTargetPosition();
		}

		public override Vector2 GetNewPosition()
		{
			if(Random.value > 0.98f)
			{
				RefreshTargetPosition();
			}

			return targetPosition;
		}

		public override bool UseAction()
		{
			return false;
		}

		public override bool IsFinished()
		{
			return (until <= Time.fixedTime);
		}

		public void RefreshTargetPosition()
		{
			targetPosition = startPosition + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
		}
	}

	public class HarvestTask : Task
	{
		public Baka ia;
		public Vector2 targetPosition;
		public FoodPile targetFoodPile;

		public override void Initialize(Baka ia)
		{
			type = TaskType.Harvest;
			this.ia = ia;

			var foodPile = ia.GetNearestBestFoodPile(ia.GetCurrentPosition());
			//var foodUnit = ia.GetNearestFoodUnit(ia.GetCurrentPosition());

			//if(foodUnit != null && foodPile.behaviour.GetFoodCount() < 10 && ia.IsCloser(foodUnit.GetPosition(), ia.player.GetPosition(), foodPile.GetPosition()))
			//{
			//	targetPosition = new Vector2(foodUnit.GetPosition().x, foodUnit.GetPosition().z);
			//}
			//else
			{
				targetFoodPile = foodPile;
				targetPosition = new Vector2(foodPile.GetPosition().x, foodPile.GetPosition().z);
			}
		}

		public override Vector2 GetNewPosition()
		{
			return targetPosition;
		}

		public override bool UseAction()
		{
			return false;
		}

		public override bool IsFinished()
		{
			var hasSoldiersFollowing = ia.PlayerHasGuardingSoldiers();
			var isInFood = ((ia.GetCurrentPosition() - targetPosition).magnitude < (SoldierConstants.FOOD_HARVEST_MIN_DISTANCE / 1.3f));
			var foodPileDepleted = (targetFoodPile != null ? targetFoodPile.behaviour.GetFoodCount() < 5 ? true : false : false);

			return (isInFood || !hasSoldiersFollowing || foodPileDepleted);
		}
	}

	public class BreedTask : Task
	{
		public Baka ia;
		public Lair playerLair;
		public Vector2 targetPosition;
		public float timeToWait;
		public bool startedBreeding;
		public bool startedLeaving;
		public float leavingStartTime;
		public bool finishedBreeding;

		public BreedTask(int count)
		{
			timeToWait = count + 0.3f;
			startedBreeding = false;
			startedLeaving = false;
			finishedBreeding = false;
		}

		public override void Initialize(Baka ia)
		{
			type = TaskType.Breed;
			this.ia = ia;

			playerLair = ia.game.GetPlayerLair(ia.player);

			targetPosition = new Vector2(playerLair.GetPosition().x, playerLair.GetPosition().z);
		}

		public override Vector2 GetNewPosition()
		{
			var isInLair = ((ia.GetCurrentPosition() - targetPosition).magnitude < (PlayerConstants.LAIR_EGG_CREATION_MIN_DISTANCE));

			if(!startedBreeding && isInLair)
			{
				timeToWait = Time.fixedTime + timeToWait;
				startedBreeding = true;
			}
			else if(startedBreeding && Time.fixedTime > timeToWait)
			{
				if(!startedLeaving)
				{
					startedLeaving = true;
					leavingStartTime = Time.fixedTime;
					targetPosition = targetPosition + new Vector2(Random.value > 0.5f ? -1f : 1f, Random.value > 0.5f ? -0.3f : 0.3f);
				}
				else if((!isInLair && (ia.GetCurrentPosition() - targetPosition).magnitude < 0.2f) || leavingStartTime + 2f < Time.fixedTime)
				{
					finishedBreeding = true;
				}
			}

			return targetPosition;
		}

		public override bool UseAction()
		{
			return false;
		}

		public override bool IsFinished()
		{
			return finishedBreeding;
		}
	}

	public class RecruitTask : Task
	{
		public Baka ia;
		public int amount;
		public float maxTime;
		public Soldier busySoldier;
		public bool noBusySoldiers;

		public RecruitTask(int amount, float maxTime)
		{
			this.amount = amount;
			this.maxTime = Time.fixedTime + maxTime;
		}

		public override void Initialize(Baka ia)
		{
			type = TaskType.Recruit;
			this.ia = ia;

			noBusySoldiers = false;
		}

		public override Vector2 GetNewPosition()
		{
			if(busySoldier == null || busySoldier.isDead)
			{
				busySoldier = GetBusySoldier();
			}

			if(busySoldier != null)
			{
				return new Vector2(busySoldier.GetPosition().x, busySoldier.GetPosition().z);
			}
			else
			{
				noBusySoldiers = true;
				return ia.GetCurrentPosition();
			}
		}

		public override bool IsFinished()
		{
			var currentAmount = ia.PlayerGuardingSoldiersCount();
			var timeOver = (maxTime < Time.fixedTime);

			return (timeOver || currentAmount >= amount || noBusySoldiers);
		}

		public override bool UseAction()
		{
			return (Random.value > 0.95f);
		}

		public Soldier GetBusySoldier()
		{
			foreach (var soldier in ia.game.GetPlayerSoldiers(ia.player))
			{
				if(soldier.owner == ia.player && soldier.state != SoldierState.Protect)
				{
					return soldier;
				}
			}

			return null;
		}
	}

	public class AttackTask : Task
	{
		public Baka ia;
		public Soldier enemySoldier;
		public float maxTime;
		public int tries;
		public bool noEnemies;
		public float lastTimeWithGuards;

		public override void Initialize(Baka ia)
		{
			type = TaskType.Attack;
			this.ia = ia;

			maxTime = Time.fixedTime + 15f;
			tries = 1;
			noEnemies = false;
			lastTimeWithGuards = Time.fixedTime;
			enemySoldier = ia.GetNearestEnemy(ia.GetCurrentPosition());
		}

		public override Vector2 GetNewPosition()
		{
			if(enemySoldier == null || enemySoldier.isDead)
			{
				tries++;
				enemySoldier = ia.GetNearestEnemy(ia.GetCurrentPosition());
			}

			if(ia.PlayerHasGuardingSoldiers())
			{
				lastTimeWithGuards = Time.fixedTime;
			}

			if(enemySoldier != null)
			{
				return new Vector2(enemySoldier.GetPosition().x, enemySoldier.GetPosition().z);
			}
			else
			{
				noEnemies = true;
				return ia.GetCurrentPosition();
			}
		}

		public override bool IsFinished()
		{
			var timeOver = (maxTime < Time.fixedTime);
			var withoutGuards = (lastTimeWithGuards + 2f < Time.fixedTime);

			return noEnemies || tries >= 5 || timeOver || withoutGuards;
		}

		public override bool UseAction()
		{
			return false;
		}
	}
}
