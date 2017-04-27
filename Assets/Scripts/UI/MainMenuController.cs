using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace AntWars
{
	public class MainMenuController : MonoBehaviour
	{
		public string SelectionMenuName;
		public RectTransform Instructions;

		private bool instructionsEntering;
		private bool instructionsLeaving;

		private void Start()
		{
			Instructions.gameObject.SetActive(false);			
		}

		public void OnPlaySelected()
		{
			SceneManager.LoadScene(SelectionMenuName);
		}

		public void OnInstructionsSelected()
		{
			if (instructionsEntering == false && instructionsLeaving == false)
			{
				instructionsEntering = true;

				Instructions.gameObject.SetActive(true);
				Instructions.anchoredPosition = new Vector2(0, 720);
				Instructions.DOAnchorPos(Vector2.zero, 0.5f)
					.SetEase(Ease.InOutExpo)
					.OnComplete(() => instructionsEntering = false);
			}
		}

		public void OnQuitSelected()
		{
			Application.Quit();
		}

		public void OnRemoveInstructions()
		{
			if (instructionsEntering == false && instructionsLeaving == false)
			{
				instructionsLeaving = true;

				Instructions.DOAnchorPos(new Vector2(0, 720), 0.5f)
					.SetEase(Ease.InOutExpo)
					.OnComplete(() =>
						{
							instructionsLeaving = false;
							Instructions.gameObject.SetActive(false);
						});
			}
		}
	}
}
