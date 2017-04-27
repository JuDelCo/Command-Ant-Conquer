using System.Collections.Generic;

namespace AntWars
{
	public class TeamSettings
	{
		public List<TeamDefinition> Teams { get; private set; }

		public TeamSettings()
		{
			Teams = new List<TeamDefinition>();
		}

		public void Clear()
		{
			Teams.Clear();
		}

		public void AddTeam(TeamDefinition team)
		{
			Teams.Add(team);
		}
	}
}
