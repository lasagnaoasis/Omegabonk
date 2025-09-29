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
    private static float MinimapScale => Preferences.MinimapScale.Value;
    private static float MinimapZoom => Preferences.MinimapZoom.Value;

    internal static void OnLevelInitialized() {
        MelonCoroutines.Start(DelayedEditMinimapUi());
        MelonCoroutines.Start(DelayedEditMinimapCamera());
    }

    private static IEnumerator DelayedEditMinimapCamera() {
        int maxWait = 150;
        while (maxWait > 0) {
            var minimapCamera = GameObject.FindFirstObjectByType<MinimapCamera>();
            if (minimapCamera != null && minimapCamera.minimapCamera != null) {
                ChangeZoom(minimapCamera);
                break;
            }

            yield return new WaitForSeconds(0.1f);

            maxWait--;
        }
    }

    private static IEnumerator DelayedEditMinimapUi() {
        int maxWait = 150;
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
}
