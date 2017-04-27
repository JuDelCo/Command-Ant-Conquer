using AntWars.Bot;
using Atto;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AntWars
{
    public static class GameManagerExtension
    {
        public static Player winner;

        public static void Initialize(this GameManager gameManager)
        {
            gameManager.players = new List<Player>();
            gameManager.soldiers = new List<Soldier>();
            gameManager.eggs = new List<SoldierEgg>();
            gameManager.lairs = new List<Lair>();
            gameManager.foodPiles = new List<FoodPile>();
            gameManager.foodUnits = new List<FoodUnit>();

            gameManager.prefabs = Resources.Load("Prefabs") as Prefabs;

            gameManager.onGameFinish += (Player player) =>
            {
                if (SceneManager.GetActiveScene().name == "Game" && SceneManager.sceneCount == 1)
                {
                    winner = player;
                    SceneManager.LoadScene("Game Over", LoadSceneMode.Additive);
                }
            };
        }

        public static void Reset(this GameManager gameManager)
        {
            gameManager.players.Clear();
            gameManager.soldiers.Clear();
            gameManager.eggs.Clear();
            gameManager.lairs.Clear();
            gameManager.foodPiles.Clear();
            gameManager.foodUnits.Clear();

            var players = Object.FindObjectsOfType(typeof(PlayerBehaviour)) as PlayerBehaviour[];
            var lairs = Object.FindObjectsOfType(typeof(LairBehaviour)) as LairBehaviour[];
            var foodPiles = Object.FindObjectsOfType(typeof(FoodPileBehaviour)) as FoodPileBehaviour[];
			var ias = 0;

			foreach (var player in players)
			{
				if(player.IsBot)
				{
					ias++;
				}
			}

            Core.Log.Debug("Found: {0} Players ({1} Bots), {2} Lairs, {3} FoodPiles", players.Length, ias, lairs.Length, foodPiles.Length);

            foreach (var playerBehaviour in players)
            {
                var player = new Player();
                player.lastEggTime = Time.fixedTime;
                player.behaviour = playerBehaviour;

				if(player.behaviour.IsBot)
				{
					player.ia = Baka.Instantiate(player);
				}

                gameManager.players.Add(player);

                playerBehaviour.playerData = player;
            }

            foreach (var player in gameManager.players)
            {
                for (int i = 0; i < GameConstants.START_SOLDIER_COUNT; ++i)
                {
                    SoldierExtension.Instantiate(player, player.GetPosition());
                }

                foreach (var soldier in gameManager.soldiers)
                {
                    soldier.state = SoldierState.Protect;
                    soldier.behaviour.transform.position = soldier.GetPlayerProtectPosition();
                }
            }

            foreach (var lairBehaviour in lairs)
            {
                var lair = new Lair();
                lair.behaviour = lairBehaviour;
                lair.foodCount = LairConstants.START_FOOD_COUNT;

                foreach (var player in gameManager.players)
                {
                    if (player.behaviour == lairBehaviour.owner)
                    {
                        lair.owner = player;
                        break;
                    }
                }

                gameManager.lairs.Add(lair);

                lairBehaviour.lairData = lair;
            }

            foreach (var foodPileBehaviour in foodPiles)
            {
                var foodPile = new FoodPile();
                foodPile.behaviour = foodPileBehaviour;

                gameManager.foodPiles.Add(foodPile);

                foodPileBehaviour.foodPileData = foodPile;
            }
        }

        public static void Update(this GameManager gameManager)
        {
            for (int i = (gameManager.players.Count - 1); i >= 0; --i)
            {
                gameManager.players[i].Update();
            }

            for (int i = (gameManager.soldiers.Count - 1); i >= 0; --i)
            {
                gameManager.soldiers[i].Update();
            }

            for (int i = (gameManager.soldiers.Count - 1); i >= 0; --i)
            {
                if (gameManager.soldiers[i].isDead)
                {
                    gameManager.soldiers[i].Kill();
                }
            }

            for (int i = (gameManager.eggs.Count - 1); i >= 0; --i)
            {
                gameManager.eggs[i].Update();
            }

            for (int i = (gameManager.lairs.Count - 1); i >= 0; --i)
            {
                if (gameManager.lairs[i].foodCount >= GameConstants.TARGET_FOOD_POINTS)
                {
                    if (gameManager.onGameFinish != null)
                    {
                        gameManager.onGameFinish(gameManager.lairs[i].owner);
                    }

                    break;
                }
            }

			if(Input.GetKey(KeyCode.Escape))
			{
				SceneManager.LoadScene("Main Menu");
			}

            // ----------------------------------------------

            var playersDefeated = 0;
            Player playerWinner = null;

            for (int i = (gameManager.players.Count - 1); i >= 0; --i)
            {
                var player = gameManager.players[i];

                if (gameManager.GetPlayerSoldiers(player).Count + gameManager.GetPlayerEggs(player).Count == 0 && gameManager.GetPlayerLair(player).foodCount < PlayerConstants.FOOD_SOLDIER_COST)
                {
                    playersDefeated++;
                }
                else
                {
                    playerWinner = player;
                }
            }

            if (playersDefeated >= gameManager.players.Count - 1)
            {
                if (gameManager.onGameFinish != null)
                {
                    gameManager.onGameFinish(playerWinner);
                }
            }
        }

        public static Lair GetPlayerLair(this GameManager gameManager, Player player)
        {
            Lair playerLair = null;

            foreach (var lair in gameManager.lairs)
            {
                if (lair.owner == player)
                {
                    playerLair = lair;
                    break;
                }
            }

            return playerLair;
        }

        public static List<Soldier> GetPlayerSoldiers(this GameManager gameManager, Player player)
        {
            var result = new List<Soldier>();

            foreach (var soldier in gameManager.soldiers)
            {
                if (soldier.owner == player)
                {
                    result.Add(soldier);
                }
            }

            return result;
        }

        public static List<SoldierEgg> GetPlayerEggs(this GameManager gameManager, Player player)
        {
            var result = new List<SoldierEgg>();

            foreach (var egg in gameManager.eggs)
            {
                if (Vector3.Distance(egg.GetPosition(), gameManager.GetPlayerLair(player).GetPosition()) < SoldierEggConstants.HATCH_MIN_DISTANCE_LAIR)
                {
                    result.Add(egg);
                }
            }

            return result;
        }
    }
}
