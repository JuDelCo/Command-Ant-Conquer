using Rewired;
using UnityEngine;

public class RewiredInputLoader : MonoBehaviour 
{
	[SerializeField] private InputManager inputManagerPrefab;

	public void Awake()
	{
		InputManager inputManager = FindObjectOfType<InputManager>();

		if (inputManager == null)
		{
			Instantiate(inputManagerPrefab);
		}
	}
}
