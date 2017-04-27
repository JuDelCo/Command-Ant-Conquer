using UnityEngine;

public class TeamDefinition 
{
	public Rewired.Player Player { get; private set; }
	public Color Color { get; private set; }

	public TeamDefinition(Rewired.Player player, Color color)
	{
		Player = player;
		Color = color;
	}
}
