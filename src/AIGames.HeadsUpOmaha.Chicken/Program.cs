using AIGames.HeadsUpOmaha.Platform;

namespace AIGames.HeadsUpOmaha.Chicken
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ConsolePlatform.Run(new ChickenBot());
		}
	}
}
