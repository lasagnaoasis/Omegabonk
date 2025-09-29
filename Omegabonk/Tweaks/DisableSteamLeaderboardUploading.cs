using HarmonyLib;

using Il2CppAssets.Scripts.Menu.Shop.Leaderboards;
using Il2CppAssets.Scripts.Steam;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using MelonLoader;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk.Tweaks;

//void SteamLeaderboardsManager.UploadLeaderboardScore(string leaderboardName, int score, Il2CppStructArray<int> details, ELeaderboardCategory category)
[HarmonyPatch(typeof(SteamLeaderboardsManager), nameof(SteamLeaderboardsManager.UploadLeaderboardScore), new Type[] { typeof(string), typeof(int), typeof(Il2CppStructArray<int>), typeof(ELeaderboardCategory) })]
internal static class DisableSteamLeaderboardUploading {
    private static bool Prefix(string leaderboardName, int score, Il2CppStructArray<int> details, ELeaderboardCategory category) {
        MelonLogger.Msg($"Prevented leaderboard upload! (LBN: {leaderboardName}, Score: {score}, Details: [{string.Join(", ", details)}], LBC: {category})");
        return false;
    }
}
