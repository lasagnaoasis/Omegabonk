using HarmonyLib;

using Il2CppAssets.Scripts.Menu.Shop.Leaderboards;
using Il2CppAssets.Scripts.Steam;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk.Tweaks;

//string SteamRichPresenceManager.GetRandomMenuStatus()
[HarmonyPatch(typeof(SteamRichPresenceManager), nameof(SteamRichPresenceManager.GetRandomMenuStatus), new Type[] { })]
internal static class CustomizeSteamRichPresence {
    private static List<string> _customStatuses = new List<string> {
            "Skating like a pro!",
            "Grinding through levels!",
            "Flipping out in Megabonk!",
            "Landing tricks and making bonks!",
            "Shredding the virtual skatepark!",
            "Bonking with style!",
            "Skate, bonk, repeat!",
            "Nailing those combos!",
            "Cruising through Megabonk!",
            "Skateboarding to victory!"
        };

    //private static bool Prefix(ref string __result) {
    //    __result = _customStatuses[Random.Shared.Next(_customStatuses.Count)];
    //    return false; // Skip original method
    //}

    private static void Postfix(ref string __result) {
        __result = _customStatuses[Random.Shared.Next(_customStatuses.Count)];
    }
}
