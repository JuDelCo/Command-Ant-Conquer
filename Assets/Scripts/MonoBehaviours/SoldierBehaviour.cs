using UnityEngine;
using UnityEngine.AI;

namespace AntWars
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SoldierBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public Soldier soldierData;

        private NavMeshAgent agent;
		private AntVisuals antVisuals;
		private float speed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            antVisuals = GetComponent<AntVisuals>();
			speed = agent.speed;
        }

        public void MoveTo(Vector3 position)
        {
            agent.destination = position;

			if(soldierData.state == SoldierState.Attack)
			{
				agent.speed = speed * 1.2f;
			}
			else
			{
				agent.speed = speed;
			}
        }

		public void CarryEgg()
		{
			antVisuals.CarryLarva();
		}

		public void CarryFood()
		{
			antVisuals.CarryFood();
		}

		public void CarryNone()
		{
			antVisuals.CarryNone();
		}
    }
}
