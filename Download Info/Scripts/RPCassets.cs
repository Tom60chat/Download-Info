using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Download_Info
{
    class RPCassets
    {
        const string DefaultImageKey = "downloadrp";

        public enum Game {
            CallofDuty,
            Overwatch,
            Apex,
            Fortnite,
            GrandTheftAutoV,
            Hearthstone,
            LeagueofLegends,
            Minecraft,
            CounterStrikeGlobalOffensive,
            Fallout76,
        }

        public static Dictionary<Game, string> Assets = new Dictionary<Game, string>()
        {
            {Game.CallofDuty, "cod"},
            {Game.Overwatch, "overwatch"},
            {Game.Apex, "apex"},
            {Game.Fortnite, "fortnite"},
            {Game.GrandTheftAutoV, "gtav"},
            {Game.Hearthstone, "hearthstone"},
            {Game.LeagueofLegends, "lol"},
            {Game.Minecraft, "minecraft"},
            {Game.CounterStrikeGlobalOffensive, "csgo"},
            {Game.Fallout76, "fallout76"},
        };

        public static string GetImageKey(Game game)
        {
            if (Assets.TryGetValue(game, out string value))
                return value;
            else
                return DefaultImageKey; // to show somethings
        }


        public static string GetImageKey(string Game)
        {
            string game = Game.ToLower().Replace(" ", ""); // A Game! => a game! => agame!
            game = Regex.Replace(game, @"[^\w\d]", ""); // agame! => agame

            foreach (Game suit in (Game[])Enum.GetValues(typeof(Game)))
            {
                string EnumName = Enum.GetName(typeof(Game), suit).ToLower();

                if (game.Contains(EnumName))
                    return GetImageKey(suit);
            }

            Console.WriteLine($"Can't find {Game} value: {game}");

            return DefaultImageKey; // to show somethings
        }
    }
}
