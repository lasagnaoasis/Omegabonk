using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts.Managers;

using Il2CppRewired;

using Il2CppSystem.Linq;

using MelonLoader;

using System;
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
            if (newCameraDistance == MaxCameraDistance || newCameraDistance == MinCameraDistance)
                return;

            __instance.OnSettingUpdated("camera_distance", oldCameraDistance, newCameraDistance);
        }
    }
}
