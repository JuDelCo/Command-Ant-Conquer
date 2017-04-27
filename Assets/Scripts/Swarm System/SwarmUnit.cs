using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SwarmUnit : MonoBehaviour
{
	private NavMeshAgent agent;
	//private Vector3 target;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
	}

	public void UpdateTargetPosition(Vector3 target)
	{
		//this.target = target;
		agent.destination = target;
	}
}
