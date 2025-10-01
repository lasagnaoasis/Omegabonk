using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts.Managers;

using Il2CppRewired;

using Il2CppSystem.Linq;

using MelonLoader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Omegabonk.Tweaks;

internal static class BetterCamera {
    internal static bool Enabled => Preferences.Initialized && Preferences.EnableBetterCamera.Value;
    private static float MinCameraDistance => 0f;
    private static float MaxCameraDistance => Preferences.MaxCameraDistance.Value;
    private static bool ScaleFovBasedOnCameraDistance => Preferences.ScaleFOVBasedOnCameraDistance.Value;

    private const string CameraDistancePreferenceKey = "camera_distance";
    private const string FovPreferenceKey = "fov";
    private const float MaxFov = 100f;
    private const float MaxAdditionalFov = 10f;
    private const float MaxScaledFov = MaxFov + MaxAdditionalFov;
    private static float _zoomRatio = 0.2f;

    //void PlayerCamera.Update()
    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Update), new Type[] { })]
    private static class EditPlayerCameraPatch1 {
        private static void Postfix(PlayerCamera __instance) {
            if (!Enabled)
                return;

            var players = ReInput.players;
            var inputPlayer = players.AllPlayers[1];
            var inputPlayerControllers = inputPlayer.controllers;
            var mouse = inputPlayerControllers.Mouse;
            var mouseWheel = mouse.axes[2];

            if (mouseWheel.timeActive <= 0d)
                return;


            var oldCameraDistance = SaveManager.Instance.config.cfVideoSettings.camera_distance;
            var newCameraDistance = oldCameraDistance;
            newCameraDistance += (mouseWheel.value * -1) * _zoomRatio;
            newCameraDistance = Mathf.Clamp(newCameraDistance, MinCameraDistance, MaxCameraDistance);
            SaveManager.Instance.config.cfVideoSettings.camera_distance = newCameraDistance;
            if (newCameraDistance != MinCameraDistance && newCameraDistance != MaxCameraDistance) {
                __instance.OnSettingUpdated(CameraDistancePreferenceKey, oldCameraDistance, newCameraDistance);

                //__instance.currentZ = newCameraDistance + __instance.defaultZ;
                //var offset = __instance.offset3rdPerson;
                //__instance.offset3rdPerson = new Vector3(offset.x, offset.y, __instance.currentZ + __instance.defaultZ);
                //MelonLogger.Msg($"[{nameof(BetterCamera)}.{nameof(Postfix)}] Changed camera distance from {oldCameraDistance} to {newCameraDistance}");
            }

            if (!ScaleFovBasedOnCameraDistance)
                return;

            var currentFov = SaveManager.Instance.config.cfVideoSettings.fov;
            //var currentFov = __instance.camera.fieldOfView;
            var maxFov = currentFov + MaxAdditionalFov;
            if (currentFov > MaxFov || maxFov > MaxScaledFov) {
                //MelonLogger.Error($"[{nameof(BetterCamera)}.{nameof(Postfix)}] Current FOV {currentFov} or max FOV {maxFov} too high, skipping FOV adjustment");
                return;
            }

            var newFov = currentFov;
            newFov = Mathf.Lerp(newFov, maxFov, (newCameraDistance / MaxCameraDistance));
            if (__instance.camera.fieldOfView != newFov) {
                __instance.camera.fieldOfView = newFov;
                //MelonLogger.Msg($"[{nameof(BetterCamera)}.{nameof(Postfix)}] Changed FOV from {currentFov} to {newFov}");
            }
        }
    }

    //void PlayerCamera.Start()
    [HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.Start), new Type[] { })]
    private static class EditPlayerCameraPatch2 {
        private static void Postfix(PlayerCamera __instance) {
            if (!Enabled)
                return;

            MelonCoroutines.Start(DelayedStart());
        }

        private static IEnumerator DelayedStart() {
            yield return new WaitForEndOfFrame();


        }
    }

    ////void PlayerCamera.CameraInput(Vector3 playerRotation)
    //[HarmonyPatch(typeof(PlayerCamera), nameof(PlayerCamera.CameraInput), new Type[] { typeof(Vector3) })]
    //private static class EditPlayerCameraPatch3 {
    //    private static void Prefix(PlayerCamera __instance, Vector3 playerRotation) {
    //        if (!Enabled)
    //            return;

            
    //    }
    //}
}
