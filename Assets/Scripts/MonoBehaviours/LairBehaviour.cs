using UnityEngine;
using UnityEngine.UI;

namespace AntWars
{
    public class LairBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public Lair lairData;

        public PlayerBehaviour owner;

        public SpriteRenderer flagRenderer;
        public Text scoreDisplay;

        void Start()
        {
            SetFlagColor(owner.playerColor);
        }

        public void SetFlagColor(Color color)
        {
            flagRenderer.color = color;
            scoreDisplay.color = color;
        }
    }
}
