using UnityEngine.Events;

public static class UnityEventExtensions 
{
	public static void InvokeIfNotNull(this UnityEvent unityEvent)
	{
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
	}
}
