using UnityEngine;

public class TweeningGoButton : MonoBehaviour
{
	void Update ()
	{
		transform.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, Mathf.Sin(Time.fixedTime * 2f + 1f) * 0.1f));
	}
}
