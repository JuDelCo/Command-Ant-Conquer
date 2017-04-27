using Atto;
using UnityEngine;
using Rewired;

namespace AntWars
{
	public class GameInitializer : MonoBehaviour
	{
		[SerializeField] private Transform[] teams;
		[SerializeField] private InputManager inputManagerPrefab;

		private void Awake()
		{
			InputManager inputManager = FindObjectOfType<InputManager>();

			if (inputManager == null)
			{
				Instantiate(inputManagerPrefab);
			}

			TeamSettings settings = Core.Get<TeamSettings>();

			for (int i = 0; i < teams.Length - 1; i++)
			{
				teams[i].GetComponentInChildren<PlayerBehaviour>().playerColor = Color.clear;
			}

			if (settings.Teams.Count > 0)
			{
				for (int i = 0; i < teams.Length; i++)
				{
					if (settings.Teams.Count > i)
					{
						teams[i].gameObject.SetActive(true);
						PlayerBehaviour player = teams[i].GetComponentInChildren<PlayerBehaviour>();
						player.Player = settings.Teams[i].Player;
						player.playerColor = settings.Teams[i].Color;
					}
					else
					{
						//teams[i].gameObject.SetActive(false);

						teams[i].gameObject.SetActive(true);
						PlayerBehaviour player = teams[i].GetComponentInChildren<PlayerBehaviour>();
						player.Player = null;
						player.IsBot = true;
						player.playerColor = GetUnusedColor();
					}
				}
			}
		}

		private void Start()
		{
			GameManager manager = Core.Get<GameManager>();

			for (int i = 0; i < manager.soldiers.Count; i++)
			{
				Destroy(manager.soldiers[i].behaviour.gameObject);
			}

			manager.Reset();
		}

		protected Color GetUnusedColor()
		{
			var teamColors = Resources.Load("TeamColors") as TeamColors;
			var color = teamColors.Colors[Random.Range(0, teamColors.Colors.Length - 1)];
			bool found = false;

			while(!found)
			{
				bool isRepeated = false;

				for (int i = 0; i < teams.Length - 1; i++)
				{
					if(teams[i].GetComponentInChildren<PlayerBehaviour>().playerColor == color)
					{
						isRepeated = true;
						break;
					}
				}

				if(!isRepeated)
				{
					found = true;
				}
				else
				{
					color = teamColors.Colors[Random.Range(0, teamColors.Colors.Length - 1)];
				}
			}

			return color;
		}
	}
}
