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
    private const float MaxMinimapZoom = 270f;

    private const int MinAdditionalWeaponSlots = 0;
    private const int MaxAdditionalWeaponSlots = 3;
    private const int MinAdditionalTomeSlots = 0;
    private const int MaxAdditionalTomeSlots = 3;
    
    private const float DefaultMaxCameraDistance = 5f;
    private const float NewMaxCameraDistance = 15f;

    internal static bool Initialized { get; private set; }

    internal static MelonPreferences_Category OmegabonkCategory;

    internal static MelonPreferences_Entry<bool> EnableBetterMinimap;
    internal static MelonPreferences_Entry<float> MinimapScale;
    internal static MelonPreferences_Entry<float> MinimapZoom;
    internal static MelonPreferences_Entry<bool> AlwaysDisplayBossSpawnerArrow;

    internal static MelonPreferences_Entry<bool> EnableMoreTomesAndWeapons;
    internal static MelonPreferences_Entry<int> AdditionalWeaponSlots;
    internal static MelonPreferences_Entry<int> AdditionalTomeSlots;

    internal static MelonPreferences_Entry<bool> EnableMoreRefreshesSkipsAndBanishes;
    internal static MelonPreferences_Entry<int> AdditionalRefreshes;
    internal static MelonPreferences_Entry<int> AdditionalSkips;
    internal static MelonPreferences_Entry<int> AdditionalBanishes;

    internal static MelonPreferences_Entry<bool> EnableCustomSteamRichPresence;
    internal static MelonPreferences_Entry<List<string>> CustomSteamRichPresenceStatus;

    internal static MelonPreferences_Entry<bool> DisableSteamLeaderboardUpload;

    internal static MelonPreferences_Entry<bool> EnableBetterEnemyScaling;
    internal static MelonPreferences_Entry<int> MaxNumberOfEnemiesPooled;
    internal static MelonPreferences_Entry<int> MaxNumberOfEnemies;

    internal static MelonPreferences_Entry<bool> EnableBetterCamera;
    internal static MelonPreferences_Entry<float> MaxCameraDistance;
    internal static MelonPreferences_Entry<bool> ScaleFOVBasedOnCameraDistance;

    internal static void Setup() {
        OmegabonkCategory = MelonPreferences.CreateCategory("Omegabonk");

        EnableBetterMinimap = OmegabonkCategory.CreateEntry("EnableBetterMinimap", true, description: "Enables a bigger & more zoomed out minimap for better visibility");
        MinimapScale = OmegabonkCategory.CreateEntry("MinimapScale", 1.8f, description: "Minimap scale as unity units (1f - 2f)", validator: new ValueRange<float>(MinMinimapScale, MaxMinimapScale));
        MinimapZoom = OmegabonkCategory.CreateEntry("MinimapZoom", 160f, description: "Minimap zoom as FOV (60f - 270f)", validator: new ValueRange<float>(MinMinimapZoom, MaxMinimapZoom));
        AlwaysDisplayBossSpawnerArrow = OmegabonkCategory.CreateEntry("AlwaysDisplayBossSpawnerArrow", true, description: "Always displays the boss spawner arrow on the minimap");

        EnableMoreTomesAndWeapons = OmegabonkCategory.CreateEntry("EnableMoreTomesAndWeapons", true, description: "Enables additional tome and weapon slots that you can purchase in the shop");
        AdditionalWeaponSlots = OmegabonkCategory.CreateEntry("AdditionalWeaponSlots", 1, description: "Additional weapon slots (0 - 3)", validator: new ValueRange<int>(MinAdditionalWeaponSlots, MaxAdditionalWeaponSlots));
        AdditionalTomeSlots = OmegabonkCategory.CreateEntry("AdditionalTomeSlots", 1, description: "Additional tome slots (0 - 3)", validator: new ValueRange<int>(MinAdditionalTomeSlots, MaxAdditionalTomeSlots));

        EnableMoreRefreshesSkipsAndBanishes = OmegabonkCategory.CreateEntry("EnableMoreRefreshesSkipsAndBanishes", true, description: "Enables additioanl refreshes, skips and banishes that you can purchase in the shop");
        AdditionalRefreshes = OmegabonkCategory.CreateEntry("AdditionalRefreshes", 4, description: "Additional refreshes (0 - 14)", validator: new ValueRange<int>(0, 14));
        AdditionalSkips = OmegabonkCategory.CreateEntry("AdditionalSkips", 4, description: "Additional skips (0 - 14)", validator: new ValueRange<int>(0, 14));
        AdditionalBanishes = OmegabonkCategory.CreateEntry("AdditionalBanishes", 4, description: "Additional banishes (0 - 14)", validator: new ValueRange<int>(0, 14));

        EnableCustomSteamRichPresence = OmegabonkCategory.CreateEntry("EnableCustomSteamRichPresence", false, description: "Enables custom Steam Rich Presence status");
        CustomSteamRichPresenceStatus = OmegabonkCategory.CreateEntry("CustomSteamRichPresenceStatus", new List<string> { "bonk bonk bonk", "I'M MEGABONKING" }, description: "Custom Steam Rich Presence status (at least 1 required)");

        DisableSteamLeaderboardUpload = OmegabonkCategory.CreateEntry("DisableSteamLeaderboardUpload", true, description: "Disables uploading scores to the Steam Leaderboards (anti-cheat)");

        EnableBetterEnemyScaling = OmegabonkCategory.CreateEntry("EnableBetterEnemyScaling", true, description: "Enables better enemy scaling");
        MaxNumberOfEnemiesPooled = OmegabonkCategory.CreateEntry("MaxNumberOfEnemiesPooled", 2500, description: "Maximum number of enemies pooled (500 - 2500)", validator: new ValueRange<int>(500, 2500));
        MaxNumberOfEnemies = OmegabonkCategory.CreateEntry("MaxNumberOfEnemies", 1250, description: "Maximum number of enemies (250 - 1000)", validator: new ValueRange<int>(250, 1250));

        EnableBetterCamera = OmegabonkCategory.CreateEntry("EnableBetterCamera", true, description: "Enables you to zoom in & out with your mousewheel");
        MaxCameraDistance = OmegabonkCategory.CreateEntry("MaxCameraDistance", 10f, description: $"Maximum camera distance ({DefaultMaxCameraDistance}f - {NewMaxCameraDistance}f)", validator: new ValueRange<float>(DefaultMaxCameraDistance, NewMaxCameraDistance));
        ScaleFOVBasedOnCameraDistance = OmegabonkCategory.CreateEntry("ScaleFOVBasedOnCameraDistance", true, description: "Scales FOV based on camera distance (max camera distance will slightly increase FOV)");

        Initialized = true;
    }
}
