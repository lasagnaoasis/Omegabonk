using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts.Menu.Shop.Leaderboards;
using Il2CppAssets.Scripts.Steam;
using Il2CppAssets.Scripts.Steam.LeaderboardsNew;
using Il2CppAssets.Scripts.Tools;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

using MelonLoader;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk.Tweaks;

//void SteamLeaderboardsManagerNew.UploadLeaderboardScore(string leaderboardName, int score, Il2CppStructArray<int> details, bool isFriendsLb)
[HarmonyPatch(typeof(SteamLeaderboardsManagerNew), nameof(SteamLeaderboardsManagerNew.UploadLeaderboardScore), new Type[] { typeof(string), typeof(int), typeof(Il2CppStructArray<int>), typeof(bool) })]
internal static class DisableSteamLeaderboardUploading {
    internal static bool Enabled => Preferences.Initialized && Preferences.DisableSteamLeaderboardUpload.Value;

    private static bool Prefix(string leaderboardName, int score, Il2CppStructArray<int> details, bool isFriendsLb) {
        if (!Enabled) {
            if (MoreTomeAndWeaponSlots.WillTriggerAntiCheat() || MoreRefreshesSkipsAndBanishes.WillTriggerAntiCheat() || AdvancedEnemyScaling.WillTriggerAntiCheat()) {
                MelonLogger.Warning($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(Prefix)}] You disabled the anti-cheat patch, but you are playing with tweaks that are considered cheating. Please keep it fair for others by reenabling it.");
                MelonLogger.Warning($"[{nameof(DisableSteamLeaderboardUploading)}.{nameof(Prefix)}] \"Cheats\": {nameof(MoreTomeAndWeaponSlots)}, {nameof(MoreRefreshesSkipsAndBanishes)}, {nameof(AdvancedEnemyScaling)}");
            }
            
            return true;
        }

        MelonLogger.Msg($"Prevented leaderboard upload! (LBN: {leaderboardName}, Score: {score}, Details: [{string.Join(", ", details)}], IFLB: {isFriendsLb})");
        return false;
    }

    //void Potato.MarkPotato(EPotatoFlags flag, string message)
    [HarmonyPatch(typeof(Potato), nameof(Potato.MarkPotato), new Type[] { typeof(EPotatoFlags), typeof(string) })]
    private static class EditPotatoPatch1 {
        private static void Postfix(EPotatoFlags flag, string message) {
            MelonLogger.Warning($"The Megabonk anti-cheat has marked you as a cheater! {flag} - {message}");
        }
    }

    //void Potato.Update()
    [HarmonyPatch(typeof(Potato), nameof(Potato.Update), new Type[] { })]
    private static class EditPotatoPatch2 {
        private static bool _onlyOnce = false;

        private static void Postfix() {
            if (Potato.flags != EPotatoFlags.None && !_onlyOnce) {
                MelonLogger.Warning($"Cheater flags updated: {Potato.flags}");
                _onlyOnce = true;
            }
        }
    }

    //bool Sus.Check()
    [HarmonyPatch(typeof(Sus), nameof(Sus.Check), new Type[] { })]
    private static class EditSusPatch1 {
        private static bool _onlyOnce = false;

        private static void Postfix(ref bool __result) {
            if (__result)
                Melon<OmegabonkMod>.Logger.Warning("The internal anti-cheat considers you sus");
            else
                Melon<OmegabonkMod>.Logger.Msg("The internal anti-cheat hasn't found anything sus");
        }
    }

    //bool Sus.CheckMods(out string reason)
    [HarmonyPatch(typeof(Sus), nameof(Sus.CheckMods))]
    private static class EditSusPatch2 {
        private static bool _onlyOnce = false;

        private static void Postfix(ref bool __result, ref string reason) {
            if (__result)
                Melon<OmegabonkMod>.Logger.Warning($"The internal anti-cheat considers you sus, reason: {reason}");
            else
                Melon<OmegabonkMod>.Logger.Msg("The internal anti-cheat hasn't found anything sus");
        }
    }
}
