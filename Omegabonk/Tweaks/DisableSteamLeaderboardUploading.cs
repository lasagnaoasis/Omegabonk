using HarmonyLib;

using Il2CppAssets.Scripts.Menu.Shop.Leaderboards;
using Il2CppAssets.Scripts.Steam;
using Il2CppAssets.Scripts.Tools;

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
    internal static bool Enabled => Preferences.Initialized && Preferences.DisableSteamLeaderboardUpload.Value;

    private static bool Prefix(string leaderboardName, int score, Il2CppStructArray<int> details, ELeaderboardCategory category) {
        if (!Enabled) {
            if (MoreTomeAndWeaponSlots.WillTriggerAntiCheat() || MoreRefreshesSkipsAndBanishes.WillTriggerAntiCheat() || AdvancedEnemyScaling.WillTriggerAntiCheat()) {
                MelonLogger.Warning($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(Prefix)}] You disabled the anti-cheat patch, but you are playing with tweaks that are considered cheating. Please keep it fair for others by reenabling it.");
                MelonLogger.Warning($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(Prefix)}] \"Cheats\": {nameof(MoreTomeAndWeaponSlots)}, {nameof(MoreRefreshesSkipsAndBanishes)}, {nameof(AdvancedEnemyScaling)}");
            }
            
            return true;
        }

        MelonLogger.Msg($"Prevented leaderboard upload! (LBN: {leaderboardName}, Score: {score}, Details: [{string.Join(", ", details)}], LBC: {category})");
        return false;
    }

    //void Potato.MarkPotato(EPotatoFlags flag, string message)
    [HarmonyPatch(typeof(Potato), nameof(Potato.MarkPotato), new Type[] { typeof(EPotatoFlags), typeof(string) })]
    private static class EditPotatoPatch1 {
        private static void Postfix(EPotatoFlags flag, string message) {
            MelonLogger.Error($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(EditPotatoPatch1)}.{nameof(Postfix)}] Marked as cheater! {flag} - {message}");
        }
    }

    //void Potato.Update()
    [HarmonyPatch(typeof(Potato), nameof(Potato.Update), new Type[] { })]
    private static class EditPotatoPatch2 {
        private static bool _onlyOnce = false;

        private static void Postfix() {
            if (Potato.flags != EPotatoFlags.None && !_onlyOnce) {
                MelonLogger.Error($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(EditPotatoPatch2)}.{nameof(Postfix)}] Cheater flags updated: {Potato.flags}");
                _onlyOnce = true;
            }
        }
    }

    ////void Leaderboards.UploadScore(int score, ELeaderboardCategory category)
    //[HarmonyPatch(typeof(Leaderboards), nameof(Leaderboards.UploadScore), new Type[] { typeof(int), typeof(ELeaderboardCategory) })]
    //private static class EditLeaderboardsPatch1 {
    //    private static void Postfix(int score, ELeaderboardCategory category) {
    //        MelonLogger.Msg($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(EditLeaderboardsPatch1)}.{nameof(Postfix)}] UploadScore {score} - {category}");
    //    }
    //}
}
