using AIGames.HeadsUpOmaha.Platform;
namespace AIGames.HeadsUpOmaha.Checkers
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConsolePlatform.Run(new CheckersBot());
		}
	}
}
