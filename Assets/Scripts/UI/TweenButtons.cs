using UnityEngine;
using DG.Tweening;

namespace AntWars
{
	public class TweenButtons : MonoBehaviour
	{
		public float HighlightScale = 1.1f;

		public void Highlight()
		{
			DOTween.Kill(transform);
			transform.DOScale(HighlightScale, 0.3f)
				.SetEase(Ease.InOutSine);
		}

		public void BackToNormal()
		{
			DOTween.Kill(transform);
			transform.DOScale(1, 0.3f)
				.SetEase(Ease.InOutSine);
		}
	}
}
