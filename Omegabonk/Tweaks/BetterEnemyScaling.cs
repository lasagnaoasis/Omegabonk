using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts.Game.Spawning.New.Summoners;
using Il2CppAssets.Scripts.Managers;
using Il2CppAssets.Scripts.Steam;

using MelonLoader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Omegabonk.Tweaks;


internal static class BetterEnemyScaling {
    internal static bool Enabled => Preferences.Initialized && Preferences.EnableBetterEnemyScaling.Value;
    internal static int MaxNumberOfEnemiesPooled => Preferences.MaxNumberOfEnemiesPooled.Value;
    internal static int MaxNumberOfEnemies => Preferences.MaxNumberOfEnemies.Value;

    //void EnemyManager.Start()
    [HarmonyPatch(typeof(EnemyManager), nameof(EnemyManager.Start), new Type[] { })]
    private static class EditEnemyManagerPatch1 {
        private static void Postfix(EnemyManager __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(BetterEnemyScaling)}.{nameof(Postfix)}] Start");

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(EnemyManager instance) {
            yield return new WaitForEndOfFrame();


        }
    }

    //int EnemyManager.GetNumMaxEnemies()
    [HarmonyPatch(typeof(EnemyManager), nameof(EnemyManager.GetNumMaxEnemies), new Type[] { })]
    private static class EditEnemyManagerPatch2 {
        private static void Postfix(EnemyManager __instance, ref int __result) {
            if (!Enabled)
                return;

            //__result += 250;
        }
    }

    //int StageSummoner.GetNumTargetEnemies()
    [HarmonyPatch(typeof(StageSummoner), nameof(StageSummoner.GetNumTargetEnemies), new Type[] { })]
    private static class EditStageSummonerPatch1 {
        private static void Postfix(StageSummoner __instance, ref int __result) {
            if (!Enabled)
                return;

            //__result += 250;
        }
    }
}
