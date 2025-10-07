using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts.Camera;

using MelonLoader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static MelonLoader.MelonLogger;
using static Omegabonk.Tweaks.MoreTomeAndWeaponSlots;

namespace Omegabonk.Tweaks;

internal static class BetterMinimap {
    internal static bool Enabled => Preferences.EnableBetterMinimap.Value;
    private static float MinimapScale => Preferences.MinimapScale.Value;
    private static float MinimapZoom => Preferences.MinimapZoom.Value;
    private static bool AlwaysDisplayBossSpawnerArrow => Preferences.AlwaysDisplayBossSpawnerArrow.Value;

    //void MinimapCamera.Start()
    [HarmonyPatch(typeof(MinimapCamera), nameof(MinimapCamera.Start), new Type[] { })]
    private static class EditMinimapCameraPatch1 {
        private static void Postfix(MinimapCamera __instance) {
            if (!Enabled)
                return;

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(MinimapCamera instance) {
            for (int i = 0; i < 10; i++)
                yield return new WaitForEndOfFrame();

            ChangeZoom(instance);

            if (!AlwaysDisplayBossSpawnerArrow || instance.bossSpotted)
                yield break;

            yield return DisplayBossArrow(instance);
        }
        private static void ChangeZoom(MinimapCamera minimapCamera) {
            minimapCamera.minimapCamera.orthographicSize = MinimapZoom;
        }

        private static IEnumerator DisplayBossArrow(MinimapCamera instance) {
            instance.TryFindBossSpawner();
            if (instance.bossSpawner == null) {
                var maxWait = 150;
                while (instance.bossSpawner == null && maxWait > 0) {
                    yield return new WaitForSeconds(0.1f);

                    instance.TryFindBossSpawner();

                    maxWait--;
                }

                if (instance.bossSpawner == null) {
                    MelonLogger.Error($"[{nameof(BetterMinimap)}.{nameof(EditMinimapCameraPatch1)}.{nameof(DelayedStart)}] Couldn't find boss spawner!");
                    yield break;
                }
            }

            instance.AddArrow(instance.bossSpawner, instance.bossColor);
            instance.bossSpotted = true;
        }
    }

    //void MinimapUi.Awake()
    [HarmonyPatch(typeof(MinimapUi), nameof(MinimapUi.Awake), new Type[] { })]
    private static class EditMinimapUiPatch1 {
        private static void Postfix(MinimapUi __instance) {
            if (!Enabled)
                return;

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(MinimapUi instance) {
            for (int i = 0; i < 10; i++)
                yield return new WaitForEndOfFrame();

            UpdateScale(instance);
        }

        private static void UpdateScale(MinimapUi minimapUi) {
            minimapUi.UpdateScale(MinimapScale);
        }
    }
}
