using UnityEngine;
using UnityEngine.UI;
using Rewired;
using System;

namespace AntWars
{
	public class KeyboardHintController : MonoBehaviour
	{
		private static readonly string UpArrow = "▲";
		private static readonly string RightArrow = "▶";
		private static readonly string DownArrow = "▼";
		private static readonly string LeftArrow = "◀";
		private static readonly string Comma = ",";

		public Text upKey;
		public Text rightKey;
		public Text downKey;
		public Text leftKey;
		public Text regroupKey;

		public void Setup(Rewired.Player player)
		{
			var iterator = player.controllers.maps.GetAllMaps(ControllerType.Keyboard).GetEnumerator();
			iterator.MoveNext();
			ControllerMap map = iterator.Current;
			ActionElementMap[] buttonMaps = new ActionElementMap[map.buttonMapCount];
			map.ButtonMaps.CopyTo(buttonMaps, 0);
			ActionElementMap up = Array.Find(buttonMaps, x => x.actionId == 0 && x.axisContribution == Pole.Positive);
			ActionElementMap right = Array.Find(buttonMaps, x => x.actionId == 1 && x.axisContribution == Pole.Positive);
			ActionElementMap down = Array.Find(buttonMaps, x => x.actionId == 0 && x.axisContribution == Pole.Negative);
			ActionElementMap left = Array.Find(buttonMaps, x => x.actionId == 1 && x.axisContribution == Pole.Negative);
			ActionElementMap action = Array.Find(buttonMaps, x => x.actionId == 2);

			SetText(up.keyCode, upKey);
			SetText(right.keyCode, rightKey);
			SetText(down.keyCode, downKey);
			SetText(left.keyCode, leftKey);
			SetText(action.keyCode, regroupKey);
		}

		private void SetText(KeyCode code, Text text)
		{
			string stringText = string.Empty;

			switch (code)
			{
				case KeyCode.UpArrow: stringText = UpArrow; break;
				case KeyCode.RightArrow: stringText = RightArrow; break;
				case KeyCode.DownArrow: stringText = DownArrow; break;
				case KeyCode.LeftArrow: stringText = LeftArrow; break;
				case KeyCode.Comma: stringText = Comma; break;
				default: stringText = code.ToString(); break;
			}

			text.text = stringText;
		}
	}
}
