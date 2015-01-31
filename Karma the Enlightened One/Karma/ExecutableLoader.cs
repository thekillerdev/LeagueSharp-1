using LeagueSharp;
using LeagueSharp.Common;

namespace Karma
{
    /// <summary>
    ///     ExecutableLoader Class, accepts executable program call.
    /// </summary>
    internal class ExecutableLoader
    {
        /// <summary>
        ///     Executable Caller
        /// </summary>
        /// <param name="args">Arguments</param>
        private static void Main(string[] args)
        {
            // .Loader sends empty args
            if (args != null)
            {
                // On Game Load
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    // Construct Karma
                    var exec = new Karma(ObjectManager.Player);

                    // Construct Menu
                    Instances.Menu = exec.Menu();

                    // Add Menu to Main Menu
                    Instances.Menu.AddToMainMenu();
                };
            }
        }
    }
}