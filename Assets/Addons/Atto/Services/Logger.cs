
namespace Atto.Services
{
	public abstract class Logger
	{
		public abstract void Debug(string message, params object[] args);
		public abstract void Info(string message, params object[] args);
		public abstract void Notice(string message, params object[] args);
		public abstract void Warning(string message, params object[] args);
		public abstract void Error(string message, params object[] args);
	}
}
