using UnityEngine;
using Rewired;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Atto;
using UnityEngine.SceneManagement;

namespace AntWars
{
	public class SelectionManager : MonoBehaviour
	{
		private class PlayerEntry
		{
			public PlayerSelectionSlot Slot { get; private set; }
			public Color Color { get; private set; }

			public PlayerEntry(PlayerSelectionSlot slot, Color color)
			{
				Slot = slot;
				Color = color;
			}
		}

		[SerializeField] private string gameSceneName;
		[SerializeField] private Button startButton;
		[SerializeField] private PlayerSelectionSlot[] slots;
		[SerializeField] private TeamColors initialAvailableColors;

		private List<PlayerEntry> players;
		private List<Color> reservedColors;
		private int joysticksCount;
		private int keyboardsCount;

		private void Awake()
		{
			players = new List<PlayerEntry>();
			reservedColors = new List<Color>();
			startButton.interactable = false;
		}

		private void Start()
		{
			var players = ReInput.players.Players;

			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].Initialize(this);
			}

			joysticksCount = 0;

			for (int i = 0; i < players.Count; i++)
			{
				Rewired.Player player = players[i];

				if (player.controllers.joystickCount > 0)
				{
					PlayerSelectionSlot slot = Array.Find(slots, x => x.Player == null);

					if (slot != null)
					{
						slot.AssignPlayer(players[i]);
					}
				}

				joysticksCount += player.controllers.joystickCount;
			}

			if (joysticksCount <= 0)
			{
				SelectKeyboard(slots[0]);
				SelectKeyboard(slots[1]);
			}
		}

		public void Update()
		{
			if(Input.GetKey(KeyCode.Escape))
			{
				SceneManager.LoadScene("Main Menu");
			}
		}

		public Color GetNextAvailableColor(Color lastColor)
		{
			Color result = default(Color);
			Color[] availableColors = initialAvailableColors.Colors;
			int lastColorIndex = Array.IndexOf(availableColors, lastColor);

			if (lastColorIndex >= availableColors.Length - 1)
			{
				lastColorIndex = -1;
			}

			bool found = false;
			int i = lastColorIndex + 1;
			int count = 0;

			while (!found && i != lastColorIndex && count < availableColors.Length)
			{
				if (!reservedColors.Contains(availableColors[i]))
				{
					result = availableColors[i];
					found = true;
				}

				if (i == availableColors.Length - 1)
				{
					i = 0;
				}
				else
				{
					i++;
				}

				count++;
			}

			if (found)
			{
				reservedColors.Remove(lastColor);
				reservedColors.Add(result);
			}

			return result;
		}

		public Color GetPreviousAvailableColor(Color lastColor)
		{
			Color result = default(Color);
			Color[] availableColors = initialAvailableColors.Colors;
			int lastColorIndex = Array.IndexOf(availableColors, lastColor);

			if (lastColorIndex <= 0)
			{
				lastColorIndex = availableColors.Length;
			}

			bool found = false;
			int i = lastColorIndex - 1;
			int count = 0;

			while (!found && i != lastColorIndex && count < availableColors.Length)
			{
				if (!reservedColors.Contains(availableColors[i]))
				{
					result = availableColors[i];
					found = true;
				}

				if (i == 0)
				{
					i = availableColors.Length - 1;
				}
				else
				{
					i--;
				}

				count++;
			}

			if (found)
			{
				reservedColors.Remove(lastColor);
				reservedColors.Add(result);
			}

			return result;
		}

		public void SelectKeyboard(PlayerSelectionSlot slot)
		{
			Rewired.Player joystickPlayerToRecycle = slot.Player;

			if (joystickPlayerToRecycle != null && joystickPlayerToRecycle.controllers.joystickCount <= 0)
			{
				joystickPlayerToRecycle = null;
			}

			Rewired.Player player = FindNextAvailableKeyboardPlayer();

			if (player != null)
			{
				slot.AssignPlayer(player);

				if (joystickPlayerToRecycle != null)
				{
					PlayerSelectionSlot nextAvailableSlot = FindNextDisabledSlot(false);

					if (nextAvailableSlot != null)
					{
						nextAvailableSlot.AssignPlayer(joystickPlayerToRecycle);
					}
				}

				if (FindNextAvailableKeyboardPlayer() == null)
				{
					for (int i = 0; i < slots.Length; i++)
					{
						slots[i].IsKeyBoardAvailable(false);
					}
				}

				keyboardsCount++;
			}
		}

		public void SlotReady(PlayerSelectionSlot slot, Color color)
		{
			players.Add(new PlayerEntry(slot, color));

			if (players.Count >= 1)
			{
				startButton.interactable = true;
			}

			if (joysticksCount <= 0 && keyboardsCount >= 2 && players.Count >= 2)
			{
				OnGameStart();
			}
		}

		public void OnGameStart()
		{
			TeamSettings settings = Core.Get<TeamSettings>();

			settings.Clear();

			for (int i = 0; i < players.Count; i++)
			{
				settings.AddTeam(new TeamDefinition(players[i].Slot.Player, players[i].Color));
			}

			SceneManager.LoadScene(gameSceneName);
		}

		private PlayerSelectionSlot FindNextDisabledSlot(bool includeSlotsWithJoysticks = true)
		{
			PlayerSelectionSlot result = null;

			bool found = false;
			int i = 0;

			while (!found && i < slots.Length)
			{
				if (!slots[i].HasJoined)
				{
					if (includeSlotsWithJoysticks
						|| (!includeSlotsWithJoysticks
							&& (slots[i].Player == null
								|| slots[i].Player.controllers.joystickCount == 0)))
					{
						result = slots[i];
						found = true;
					}
				}

				i++;
			}

			return result;
		}

		private Rewired.Player FindNextAvailableKeyboardPlayer()
		{
			Rewired.Player result = null;

			var players = ReInput.players.Players;

			for (int i = 0; i < players.Count; i++)
			{
				if (players[i].controllers.hasKeyboard)
				{
					if (Array.Find(slots, x => x.Player == players[i]) == null)
					{
						result = players[i];
					}
				}
			}

			return result;
		}
	}
}
