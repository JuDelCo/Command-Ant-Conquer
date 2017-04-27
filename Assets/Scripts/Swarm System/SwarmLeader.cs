using System.Collections.Generic;
using UnityEngine;

public class SwarmLeader : MonoBehaviour 
{
	private static readonly Vector3[] swarmOffsets = new Vector3[]
	{
		Vector3.forward * 2,
		Vector3.right * 2,
		Vector3.forward * -2,
		Vector3.left * 2
	};

	[SerializeField] private List<SwarmUnit> swarm;
	private Vector3 lastPosition;

	private void Update()
	{
		if (transform.position != lastPosition)
		{
			for (int i = 0; i < swarm.Count; i++)
			{
				swarm[i].UpdateTargetPosition(transform.position + swarmOffsets[i]);
			}

			lastPosition = transform.position;
		}
	}
}
