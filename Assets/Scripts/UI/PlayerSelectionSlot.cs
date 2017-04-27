using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace AntWars
{
	public class PlayerSelectionSlot : MonoBehaviour
	{
		public Rewired.Player Player { get { return player; } }
		public bool HasJoined { get { return hasJoined; } }

		[SerializeField] private Transform buttonHint;
		[SerializeField] private RectTransform upArrow;
		[SerializeField] private RectTransform downArrow;
		[SerializeField] private Image avatar;
		[SerializeField] private Image keyboard;
		[SerializeField] private KeyboardHintController keyboardHinter;
		[SerializeField] private Transform readyContainer;

		private SelectionManager manager;
		private Rewired.Player player;
		private bool hasJoined;
		private bool isReady;
		private Color lastColor;

		private bool selectingUp;
		private bool selectingDown;

		private bool upInput;
		private bool downInput;
		private bool acceptInput;

		private Vector3 upArrowOriginalPosition;
		private Vector3 downArrowOriginalPosition;

		private void Awake()
		{
			hasJoined = false;
			isReady = false;
			lastColor = default(Color);
			buttonHint.gameObject.SetActive(false);
			upArrow.gameObject.SetActive(false);
			downArrow.gameObject.SetActive(false);
			avatar.gameObject.SetActive(false);
			keyboard.gameObject.SetActive(true);
			keyboardHinter.gameObject.SetActive(false);
			readyContainer.gameObject.SetActive(false);
			upArrowOriginalPosition = upArrow.position;
			downArrowOriginalPosition = downArrow.position;
		}

		private void Update()
		{
			if (!isReady)
			{
				if (!hasJoined && player != null && player.GetAnyButton())
				{
					Join();
				}
				else if (player != null)
				{
					GetInput();
					ProcessInput();
				}
			}
		}

		public void Initialize(SelectionManager manager)
		{
			this.manager = manager;
		}

		public void AssignPlayer(Rewired.Player player)
		{
			this.player = player;

			if (player.controllers.joystickCount > 0)
			{
				buttonHint.gameObject.SetActive(true);
			}
			else if (player.controllers.hasKeyboard)
			{
				Join();
				keyboard.gameObject.SetActive(false);
				keyboardHinter.gameObject.SetActive(true);
				keyboardHinter.Setup(player);
			}
		}

		public void IsKeyBoardAvailable(bool value)
		{
			if (value == false)
			{
				keyboard.gameObject.SetActive(false);
			}
			else
			{
				if (!hasJoined)
				{
					keyboard.gameObject.SetActive(true);
				}
			}
		}

		public void OnSelectKeyboard()
		{
			manager.SelectKeyboard(this);
		}

		public void AllColorsTaken(bool value)
		{
			if (value == true)
			{
				SelectQueen();
			}
		}

		private void Join()
		{
			buttonHint.gameObject.SetActive(false);
			upArrow.gameObject.SetActive(true);
			downArrow.gameObject.SetActive(true);
			avatar.gameObject.SetActive(true);
			PreviewQueen(manager.GetNextAvailableColor(lastColor));

			if (!player.controllers.hasKeyboard)
			{
				keyboard.gameObject.SetActive(false);
			}

			hasJoined = true;
		}

		private void GetInput()
		{
			upInput = false;
			downInput = false;
			acceptInput = false;

			if (player.GetAxis("Move Depth") == 0)
			{
				selectingUp = false;
				selectingDown = false;
			}

			if (player.GetAxis("Move Depth") > 0 && !selectingUp)
			{
				upInput = true;
				selectingUp = true;
			}

			if (player.GetAxis("Move Depth") < 0 && !selectingDown)
			{
				downInput = true;
				selectingDown = true;
			}

			if (player.GetButtonDown("Action"))
			{
				acceptInput = true;
			}
		}

		private void ProcessInput()
		{
			if (upInput)
			{
				PreviewQueen(manager.GetPreviousAvailableColor(lastColor));

				DOTween.Kill(upArrow);
				upArrow.position = upArrowOriginalPosition;
				upArrow.DOJump(upArrow.position, 10, 1, 0.3f);
			}
			else if (downInput)
			{
				PreviewQueen(manager.GetNextAvailableColor(lastColor));

				DOTween.Kill(downArrow);
				downArrow.position = downArrowOriginalPosition;
				downArrow.DOJump(downArrow.position, -10, 1, 0.3f);
			}

			if (acceptInput)
			{
				SelectQueen();
			}
		}

		private void PreviewQueen(Color color)
		{
			if (color != default(Color))
			{
				avatar.color = color;
				lastColor = color;
			}
		}

		private void SelectQueen()
		{
			manager.SlotReady(this, lastColor);
			upArrow.gameObject.SetActive(false);
			downArrow.gameObject.SetActive(false);
			readyContainer.gameObject.SetActive(true);
			isReady = true;
		}
	}
}
