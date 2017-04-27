using AntWars;
using Atto;
using Atto.Services;

public class Bootstrap : UnityEngine.MonoBehaviour
{
	protected static bool initializated = false;

	private void Awake()
	{
		if(initializated)
		{
			return;
		}

		Core.Provide<Logger>(() =>
		{
			var logger = new ConsoleLogger(LogLevel.Debug);

			Core.Container.OnClassReplaced += (type, id) =>
			{
				Core.Get<Logger>().Warning("Provided an existing class of type '{0}' with ID '{1}'. The previous class has been unloaded.", type.ToString(), id);
			};

			Core.Container.OnFactoryReplaced += (type, id) =>
			{
				Core.Get<Logger>().Warning("Provided an existing factory of type '{0}' with ID '{1}'. The previous factory has been unloaded.", type.ToString(), id);
			};

			return logger;
		});

		Core.Get<Logger>(); // Force instance of Logger (to suscribe Container events)

		Core.Provide<GameManager>(() =>
		{
			var gameManager = new GameManager();
			gameManager.Initialize();

			return gameManager;
		});

		Core.Provide<TeamSettings>(() => 
		{
			return new TeamSettings();
		});

		initializated = true;
	}

	private void Start()
	{
		Core.Get<GameManager>().Reset();
	}

	private void Update()
	{
		Core.Get<GameManager>().Update();
	}
}
