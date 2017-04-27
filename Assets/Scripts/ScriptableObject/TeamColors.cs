using UnityEngine;

[CreateAssetMenu(menuName = "AntWars/TeamColors", fileName = "TeamColors", order = 1000)]
public class TeamColors : ScriptableObject 
{
	public Color[] Colors { get { return colors; } }

	[SerializeField] private Color[] colors;
}
