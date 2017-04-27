using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Manager
{
	private static string ManagersName = "Managers";

	private class DummyMonoBehaviour : MonoBehaviour
	{
		public UnityEvent OnUpdate = new UnityEvent();

		private void Update()
		{
			OnUpdate.InvokeIfNotNull();
		}
	}

	private DummyMonoBehaviour monoBehaviourHelper;
	private UnityAction cachedCallback;

	public Manager()
	{
		AddManagerToScene();
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}

	private void AddManagerToScene()
	{
		GameObject managersObject = GameObject.Find(ManagersName);

		if (managersObject == null)
		{
			managersObject = new GameObject(ManagersName);
		}

		GameObject myGameObject = new GameObject(GetType().Name);
		monoBehaviourHelper = myGameObject.AddComponent<DummyMonoBehaviour>();
	}

	private void OnActiveSceneChanged(Scene lastScene, Scene newScene)
	{
		if (monoBehaviourHelper == null)
		{
			AddManagerToScene();

			if (cachedCallback != null)
			{
				SubscribeToUpdate(cachedCallback);
			}
		}
	}

	protected void SubscribeToUpdate(UnityAction callback)
	{
		cachedCallback = callback;
		monoBehaviourHelper.OnUpdate.AddListener(callback);
	}

	protected void UnsubscribeToUpdate(UnityAction callback)
	{
		cachedCallback = null;
		monoBehaviourHelper.OnUpdate.RemoveListener(callback);
	}
}
