using MelonLoader;
using MelonLoader.Preferences;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk;

internal static class Preferences {
    private const float MinMinimapScale = 1f;
    private const float MaxMinimapScale = 2f;

    private const float MinMinimapZoom = 60f;
    private const float MaxMinimapZoom = 180f;

    private const int MaxAdditionalWeaponSlots = 3;
    private const int MaxAdditionalTomeSlots = 3;

    private const float MaxCameraDistanceVal = 15f;

    internal static bool Initialized { get; private set; }

    internal static MelonPreferences_Category OmegabonkCategory;

    internal static MelonPreferences_Entry<bool> EnableBetterMinimap;
    internal static MelonPreferences_Entry<float> MinimapScale;
    internal static MelonPreferences_Entry<float> MinimapZoom;

    internal static MelonPreferences_Entry<bool> EnableMoreTomesAndWeapons;
    internal static MelonPreferences_Entry<int> AdditionalWeaponSlots;
    internal static MelonPreferences_Entry<int> AdditionalTomeSlots;

    internal static MelonPreferences_Entry<bool> EnableCustomSteamRichPresence;
    internal static MelonPreferences_Entry<List<string>> CustomSteamRichPresenceStatus;

    internal static MelonPreferences_Entry<bool> DisableSteamLeaderboardUpload;

    internal static MelonPreferences_Entry<bool> EnableBetterEnemyScaling;
    internal static MelonPreferences_Entry<int> MaxNumberOfEnemiesPooled;
    internal static MelonPreferences_Entry<int> MaxNumberOfEnemies;

    internal static MelonPreferences_Entry<bool> EnableBetterCamera;
    internal static MelonPreferences_Entry<float> MaxCameraDistance;

    internal static void Setup() {
        OmegabonkCategory = MelonPreferences.CreateCategory("Omegabonk");

        EnableBetterMinimap = OmegabonkCategory.CreateEntry("EnableBetterMinimap", true, description: "Enables a bigger & more zoomed out minimap for better visibility");
        MinimapScale = OmegabonkCategory.CreateEntry("MinimapScale", 1.8f, description: "Minimap scale as unity units (1f - 2f)", validator: new ValueRange<float>(MinMinimapScale, MaxMinimapScale));
        MinimapZoom = OmegabonkCategory.CreateEntry("MinimapZoom", 140f, description: "Minimap zoom as FOV (60f - 180f)", validator: new ValueRange<float>(MinMinimapZoom, MaxMinimapZoom));

        EnableMoreTomesAndWeapons = OmegabonkCategory.CreateEntry("EnableMoreTomesAndWeapons", true, description: "Enables more tome and weapon slots in the inventory");
        AdditionalWeaponSlots = OmegabonkCategory.CreateEntry("AdditionalWeaponSlots", 1, description: "Additional weapon slots (0 - 3)", validator: new ValueRange<int>(0, MaxAdditionalWeaponSlots));
        AdditionalTomeSlots = OmegabonkCategory.CreateEntry("AdditionalTomeSlots", 1, description: "Additional tome slots (0 - 3)", validator: new ValueRange<int>(0, MaxAdditionalTomeSlots));

        EnableCustomSteamRichPresence = OmegabonkCategory.CreateEntry("EnableCustomSteamRichPresence", false, description: "Enables custom Steam Rich Presence status");
        CustomSteamRichPresenceStatus = OmegabonkCategory.CreateEntry("CustomSteamRichPresenceStatus", new List<string> { "bonk bonk bonk", "I'M MEGABONKING" }, description: "Custom Steam Rich Presence status (at least 1 required)");

        DisableSteamLeaderboardUpload = OmegabonkCategory.CreateEntry("DisableSteamLeaderboardUpload", true, description: "Disables uploading scores to the Steam Leaderboards");

        EnableBetterEnemyScaling = OmegabonkCategory.CreateEntry("EnableBetterEnemyScaling", true, description: "Enables better enemy scaling");
        MaxNumberOfEnemiesPooled = OmegabonkCategory.CreateEntry("MaxNumberOfEnemiesPooled", 2500, description: "Maximum number of enemies pooled (500 - 2500)", validator: new ValueRange<int>(500, 2500));
        MaxNumberOfEnemies = OmegabonkCategory.CreateEntry("MaxNumberOfEnemies", 1250, description: "Maximum number of enemies (250 - 1000)", validator: new ValueRange<int>(250, 1250));

        EnableBetterCamera = OmegabonkCategory.CreateEntry("EnableBetterCamera", true, description: "Enables you to zoom in & out with your mousewheel");
        MaxCameraDistance = OmegabonkCategory.CreateEntry("MaxCameraDistance", 10f, description: $"Maximum camera distance (5f - {MaxCameraDistanceVal}f)", validator: new ValueRange<float>(5f, MaxCameraDistanceVal));

        Initialized = true;
    }
}
