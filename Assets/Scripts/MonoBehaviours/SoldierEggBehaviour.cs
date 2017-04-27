using UnityEngine;
using DG.Tweening;
using Atto;
using UnityEngine.UI;

namespace AntWars
{
    public class SoldierEggBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public SoldierEgg eggData;

        public float animationTime = 2f;
        public float timeBetweenShakes = 3f;

        public float strength = 10;

        ParticleSystem ps;
        SpriteRenderer sr;

        bool hatched = false;

        public Image progressImage;

        public void Start()
        {
            transform.DOScale(0f, 0.5f).SetEase(Ease.OutBack).From();
            InvokeRepeating("Shake", 1, timeBetweenShakes);
            ps = GetComponentInChildren<ParticleSystem>();
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        void Shake()
        {
            transform.DOShakeRotation(0.5f, strength);
        }

        void Hatch()
        {
            hatched = true;
            sr.transform.DOScale(1.1f, animationTime).SetEase(Ease.OutExpo).OnComplete(() => { Splash(); });
            Destroy(progressImage.gameObject, animationTime * 0.9f);
            ps.transform.SetParent(null);
        }

        void Splash()
        {
            sr.enabled = false;
            Destroy(ps.gameObject, 0.5f);
            ps.Emit(10);
        }

        void Update()
        {
            if (progressImage)
            {
                progressImage.fillAmount = eggData.hatchProgress / SoldierEggConstants.HATCH_TIME;
            }
            if (eggData != null && eggData.hatchProgress > SoldierEggConstants.HATCH_TIME - animationTime - 0.1f && !hatched)
            {
                Hatch();
            }
        }
    }
}
