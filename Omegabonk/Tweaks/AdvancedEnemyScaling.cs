using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts._Data.ShopItems;
using Il2CppAssets.Scripts.Game.Spawning.New.Summoners;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Stats;
using Il2CppAssets.Scripts.Managers;
using Il2CppAssets.Scripts.Menu.Shop;
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


internal static class AdvancedEnemyScaling {
    internal static bool Enabled => Preferences.Initialized && Preferences.EnableAdvancedEnemyScaling.Value;
    internal static int MaxNumberOfEnemiesPooled => Preferences.MaxNumberOfEnemiesPooled.Value;
    internal static int MaxNumberOfEnemies => Preferences.MaxNumberOfEnemies.Value;

    internal static bool WillTriggerAntiCheat() => Enabled;

    //void EnemyManager.Start()
    [HarmonyPatch(typeof(EnemyManager), nameof(EnemyManager.Start), new Type[] { })]
    private static class EditEnemyManagerPatch1 {
        private static void Postfix(EnemyManager __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(AdvancedEnemyScaling)}.{nameof(Postfix)}] Start");

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(EnemyManager instance) {
            yield return new WaitForEndOfFrame();


        }
    }

    ////int EnemyManager.GetNumMaxEnemies()
    //[HarmonyPatch(typeof(EnemyManager), nameof(EnemyManager.GetNumMaxEnemies), new Type[] { })]
    //private static class EditEnemyManagerPatch2 {
    //    private static void Postfix(EnemyManager __instance, ref int __result) {
    //        if (!Enabled)
    //            return;

    //        //__result += 250;
    //    }
    //}

    ////int StageSummoner.GetNumTargetEnemies()
    //[HarmonyPatch(typeof(StageSummoner), nameof(StageSummoner.GetNumTargetEnemies), new Type[] { })]
    //private static class EditStageSummonerPatch1 {
    //    private static void Postfix(StageSummoner __instance, ref int __result) {
    //        if (!Enabled)
    //            return;

    //        //__result += 250;
    //    }
    //}

    //float PlayerStatsNew.GetStat(EStat stat)
    [HarmonyPatch(typeof(PlayerStatsNew), nameof(PlayerStatsNew.GetStat), new Type[] { typeof(EStat) })]
    private static class EditPlayerStatsNewPatch1 {
        private static void Postfix(PlayerStatsNew __instance, ref float __result, EStat stat) {
            if (!Enabled || !MoreTomeAndWeaponSlots.Enabled || (MoreTomeAndWeaponSlots.AdditionalWeaponSlots == 0 && MoreTomeAndWeaponSlots.AdditionalTomeSlots == 0))
                return;

            switch (stat) {
                case EStat.EnemyAmountMultiplier:
                case EStat.EnemyHpMultiplier:
                case EStat.EnemyDamageMultiplier: {
                    var mult = GetAmountHpAndDamageMultiplier();
                    __result *= mult;
                    //MelonLogger.Msg($"[{nameof(AdvancedEnemyScaling)}.{nameof(EditPlayerStatsNewPatch1)}.{nameof(Postfix)}] Stat: {stat}, Value: {__result}");
                    break;
                }
                case EStat.EnemyScalingMultiplier: {
                    var mult = GetScalingMultiplier();
                    __result *= mult;
                    //MelonLogger.Msg($"[{nameof(AdvancedEnemyScaling)}.{nameof(EditPlayerStatsNewPatch1)}.{nameof(Postfix)}] Stat: {stat}, Value: {__result}");
                    break;
                }
                default:
                    break;
            }
        }

        private static float GetAmountHpAndDamageMultiplier() {
            var saveManager = SaveManager.Instance;
            var progression = saveManager.progression;

            var baseWeaponSlots = 2;
            var baseTomeSlots = 2;
            var shopItems = progression.shopItems;
            var currentWeaponSlots = shopItems[EShopItem.Weapons];
            var currentTomeSlots = shopItems[EShopItem.Tomes];
            var additionalWeaponSlots = currentWeaponSlots - baseWeaponSlots;
            var additionalTomeSlots = currentTomeSlots - baseTomeSlots;

            var baseMult = 0.075f;
            return 1 + ((additionalWeaponSlots * baseMult) + (additionalTomeSlots * baseMult));
        }

        private static float GetScalingMultiplier() {
            var saveManager = SaveManager.Instance;
            var progression = saveManager.progression;

            var baseWeaponSlots = 2;
            var baseTomeSlots = 2;
            var shopItems = progression.shopItems;
            var currentWeaponSlots = shopItems[EShopItem.Weapons];
            var currentTomeSlots = shopItems[EShopItem.Tomes];
            var additionalWeaponSlots = currentWeaponSlots - baseWeaponSlots;
            var additionalTomeSlots = currentTomeSlots - baseTomeSlots;

            var baseMult = 0.05f;
            return 1 + ((additionalWeaponSlots * baseMult) + (additionalTomeSlots * baseMult));
        }
    }
}
