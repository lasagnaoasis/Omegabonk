using HarmonyLib;

using Il2CppAssets.Scripts.Saves___Serialization.Progression.Achievements;
using Il2CppAssets.Scripts.Saves___Serialization.Progression.Unlocks;
using Il2CppAssets.Scripts.Saves___Serialization.SaveFiles;
using Il2CppAssets.Scripts.Steam;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Omegabonk.Tweaks.MoreTomeAndWeaponSlots;

namespace Omegabonk.Tweaks;

////bool MyAchievements.TryUnlock(string unlockName)
//[HarmonyPatch(typeof(MyAchievements), nameof(MyAchievements.TryUnlock), new Type[] { typeof(string) })]
//internal static class DisableInternalAchievements2 {
//    private static bool Prefix(ref bool __result, string unlockName) {
//        MelonLoader.MelonLogger.Msg($"[{nameof(DisableInternalAchievements2)}.{nameof(Prefix)}] MA prevented internal achievement unlock! {unlockName}");
//        __result = false;
//        return false;
//    }
//}

////bool ProgressionSaveFile.CompleteAchievement(MyAchievement achievement)
//[HarmonyPatch(typeof(ProgressionSaveFile), nameof(ProgressionSaveFile.CompleteAchievement), new Type[] { typeof(MyAchievement) })]
//internal static class DisableInternalAchievements3 {
//    private static bool Prefix(ProgressionSaveFile __instance, MyAchievement achievement) {
//        MelonLoader.MelonLogger.Msg($"[{nameof(DisableInternalAchievements3)}.{nameof(Prefix)}] PSF prevented internal achievement unlock! {achievement.internalName}");
//        return false;
//    }
//}
