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

namespace Omegabonk.Tweaks;

internal static class BetterMinimap {
    internal static bool Enabled => Preferences.EnableBetterMinimap.Value;
    private static float MinimapScale => Preferences.MinimapScale.Value;
    private static float MinimapZoom => Preferences.MinimapZoom.Value;
    private static bool AlwaysDisplayBossSpawnerArrow => Preferences.AlwaysDisplayBossSpawnerArrow.Value;

    internal static void OnLevelInitialized() {
        if (!Enabled)
            return;

        MelonCoroutines.Start(DelayedEditMinimapUi());
        MelonCoroutines.Start(DelayedEditMinimapCamera());
    }

    private static IEnumerator DelayedEditMinimapCamera() {
        var maxWait = 150;
        while (maxWait > 0) {
            var minimapCamera = GameObject.FindFirstObjectByType<MinimapCamera>();
            if (minimapCamera != null && minimapCamera.minimapCamera != null) {
                ChangeZoom(minimapCamera);
                DisplayBossArrow(minimapCamera);
                break;
            }

            yield return new WaitForSeconds(0.1f);

            maxWait--;
        }
    }

    private static IEnumerator DelayedEditMinimapUi() {
        var maxWait = 150;
        while (maxWait > 0) {
            var minimapUi = GameObject.FindFirstObjectByType<MinimapUi>();
            if (minimapUi != null) {
                UpdateScale(minimapUi);
                break;
            }

            yield return new WaitForSeconds(0.1f);

            maxWait--;
        }
    }

    private static void UpdateScale(MinimapUi minimapUi) {
        minimapUi.UpdateScale(MinimapScale);
    }

    private static void ChangeZoom(MinimapCamera minimapCamera) {
        minimapCamera.minimapCamera.orthographicSize = MinimapZoom;
    }

    private static void DisplayBossArrow(MinimapCamera minimapCamera) {
        if (!AlwaysDisplayBossSpawnerArrow || minimapCamera.bossSpotted)
            return;

        minimapCamera.TryFindBossSpawner();
        if (minimapCamera.bossSpawner == null)
            return;

        minimapCamera.AddArrow(minimapCamera.bossSpawner, minimapCamera.bossColor);
        minimapCamera.bossSpotted = true;
    }
}
