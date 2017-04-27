using Rewired;
using UnityEngine;
using UnityEngine.AI;
using Atto;
using DG.Tweening;
using AntWars;

namespace AntWars
{
    public enum ButtonState
    {
        None,
        Pressed,
        Held
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public Player playerData;

		public Rewired.Player Player { get { return player; } set { player = value; } }
		public bool IsBot = false;
        public int PlayerId { get { return playerId; } set { playerId = value; } }
        public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; } }

        [SerializeField]
        private int playerId;
        [SerializeField]
        private float movementSpeed;

        bool isLaying = false;

        private NavMeshAgent agent;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        private Rewired.Player player;
        private Vector3 velocity;

        public Vector3 initialPosition;
        private Vector3 initialScale;

        public Color playerColor = Color.black;

        public ParticleSystem shoutParticles;
        public ParticleSystem callParticles;

        public TeamColors teamColors;

        private void Reset()
        {
            playerId = 0;
            movementSpeed = 0.5f;
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            var gameManager = Core.Get<GameManager>();
            gameManager.onPlayerAction += LayEggEffect;
        }

		private void OnDestroy()
		{
			var gameManager = Core.Get<GameManager>();
            gameManager.onPlayerAction -= LayEggEffect;
		}

		private void Start()
        {
			if (player == null)
			{
				player = ReInput.players.GetPlayer(playerId);
			}

			initialScale = transform.localScale;
            initialPosition = transform.position;

            AssignQueenColor(playerColor);
            AssignParticleColors(playerColor);
        }

        void AssignQueenColor(Color color)
        {
            spriteRenderer.color = color;
        }

        void AssignParticleColors(Color color)
        {
            var shoutParticleSystem = shoutParticles.main;
            shoutParticleSystem.startColor = (playerColor + Color.white * 0.5f);
            var callParticleSystem = callParticles.main;
            callParticleSystem.startColor = (playerColor + Color.white * 0.5f);
        }

        public void LayEggEffect(Player player, PlayerAction playerAction, Soldier relatedSoldier)
        {
            if (player.behaviour == this && playerAction == PlayerAction.SpawnEgg && !isLaying)
            {
                isLaying = true;
                Vector3 startScale = spriteRenderer.transform.localScale;
                spriteRenderer.transform.DOPunchScale(new Vector3(startScale.x, -1, 1) * 0.5f, 0.5f, 1);
                spriteRenderer.transform.DOPunchPosition(Vector3.down * 0.2f, 0.5f, 1).OnComplete(() => isLaying = false);
            }
        }

        public void ShoutEffect()
        {
            shoutParticles.Emit(5);
        }

        public void CallEffect()
        {
            callParticles.Emit(5);
        }

        /*public void Update()
        {
            Move();
            switch (playerId)
            {
                case 0: ColorUtility.TryParseHtmlString("#c51162", out playerColor); break;
                case 1: ColorUtility.TryParseHtmlString("#7f0000", out playerColor); break;
                case 2: ColorUtility.TryParseHtmlString("#F57F17", out playerColor); break;
                case 3: ColorUtility.TryParseHtmlString("#4A148C", out playerColor); break;
            }

            spriteRenderer.color = playerColor;
        }*/

        public void Move()
        {
            GetMovementInput();
            ProcessMovementInput();
        }

        public ButtonState GetActionButtonStatus()
        {
            ButtonState result = ButtonState.None;

            if (player.GetButtonDown("Action"))
            {
				CallEffect();
                result = ButtonState.Pressed;
            }
            else if (player.GetButton("Action"))
            {
                result = ButtonState.Held;
            }

            return result;
        }

        public ButtonState GetRecoverButtonStatus()
        {
            ButtonState result = ButtonState.None;

            if (player.GetButtonDown("Recover"))
            {
                CallEffect();
                result = ButtonState.Pressed;
            }
            else if (player.GetButton("Recover"))
            {
                result = ButtonState.Held;
            }

            return result;
        }

		public void MoveTo(Vector3 position, Vector2 velocity)
		{
			this.velocity.x = velocity.x;
            this.velocity.z = velocity.y;

			if (this.velocity.x != 0f || this.velocity.z != 0f)
            {
                float mirrorDirection = this.velocity.x > 0 ? 1 : -1;

                float clampedMagnitude = Mathf.Clamp01(this.velocity.magnitude);
                this.velocity = this.velocity.normalized * clampedMagnitude * movementSpeed * Time.deltaTime;

				agent.destination = position;
                //agent.Warp(transform.position - this.velocity);

                transform.localScale = new Vector3(initialScale.x * mirrorDirection, initialScale.y, initialScale.z);

                animator.speed = 1;
            }
            else
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                animator.speed = 0;
            }
		}

        private void GetMovementInput()
        {
            velocity.x = player.GetAxis("Move Horizontal");
            velocity.z = player.GetAxis("Move Depth");
        }

        private void ProcessMovementInput()
        {
            if (velocity.x != 0f || velocity.z != 0f)
            {
                float mirrorDirection = velocity.x > 0 ? 1 : -1;

                float clampedMagnitude = Mathf.Clamp01(velocity.magnitude);
                velocity = velocity.normalized * clampedMagnitude * movementSpeed * Time.deltaTime;
                Vector3 normalizedVector = Vector3.ProjectOnPlane(Camera.main.transform.up, Vector3.up).normalized * velocity.z;
                normalizedVector += Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized * velocity.x;
                agent.Warp(transform.position + normalizedVector);

                transform.localScale = new Vector3(initialScale.x * mirrorDirection, initialScale.y, initialScale.z);

                animator.speed = 1;
            }
            else
            {
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                animator.speed = 0;
            }
        }
    }
}
