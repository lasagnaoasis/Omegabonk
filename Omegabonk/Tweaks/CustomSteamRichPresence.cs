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
internal static class CustomSteamRichPresence {
    internal static bool Enabled => Preferences.Initialized && Preferences.EnableCustomSteamRichPresence.Value;

    private static List<string> _fallbackStatus = new List<string> {
            "bonk bonk bonk",
            "I'M MEGABONKING"
        };

    private static void Postfix(ref string __result) {
        if (!Enabled)
            return;

        var customStatus = Preferences.CustomSteamRichPresenceStatus.Value;
        if (customStatus is { Count: > 0})
            __result = customStatus[Random.Shared.Next(customStatus.Count)];
        else
            __result = _fallbackStatus[Random.Shared.Next(_fallbackStatus.Count)];
    }
}
