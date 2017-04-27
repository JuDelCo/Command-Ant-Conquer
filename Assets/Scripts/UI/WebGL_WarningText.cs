using UnityEngine;

public class WebGL_WarningText : MonoBehaviour
{
	void Start()
	{
		if(Application.platform != RuntimePlatform.WebGLPlayer)
		{
			gameObject.SetActive(false);
		}
	}
}
