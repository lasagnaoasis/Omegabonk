using MelonLoader;
using MelonLoader.Preferences;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk;

internal static class Preferences {
    internal static MelonPreferences_Category OmegabonkCategory;

    internal static MelonPreferences_Entry<bool> EnableBetterMinimap;
    internal static MelonPreferences_Entry<float> MinimapScale;
    internal static MelonPreferences_Entry<float> MinimapZoom;

    internal static MelonPreferences_Entry<bool> EnableMoreTomesAndWeapons;
    internal static MelonPreferences_Entry<int> AdditionalWeaponSlots;
    internal static MelonPreferences_Entry<int> AdditionalTomeSlots;
    

    internal static void Setup() {
        OmegabonkCategory = MelonPreferences.CreateCategory("Omegabonk");

        EnableBetterMinimap = OmegabonkCategory.CreateEntry("EnableBetterMinimap", true, description: "Enables a bigger & more zoomed out minimap for better visibility");
        MinimapScale = OmegabonkCategory.CreateEntry("MinimapScale", 2f, description: "Minimap scale as unity units (1f - 2f)", validator: new ValueRange<float>(1f, maxValue: 2f));
        MinimapZoom = OmegabonkCategory.CreateEntry("MinimapZoom", 150f, description: "Minimap zoom as FOV (50f - 200f)", validator: new ValueRange<float>(50f, 200f));

        EnableMoreTomesAndWeapons = OmegabonkCategory.CreateEntry("EnableMoreTomesAndWeapons", true, description: "Enables more tome and weapon slots in the inventory");
        AdditionalWeaponSlots = OmegabonkCategory.CreateEntry("AdditionalWeaponSlots", 1, description: "Additional weapon slots (0 - 2)", validator: new ValueRange<int>(0, 2));
        AdditionalTomeSlots = OmegabonkCategory.CreateEntry("AdditionalTomeSlots", 1, description: "Additional tome slots (0 - 2)", validator: new ValueRange<int>(0, 2));


    }
}
