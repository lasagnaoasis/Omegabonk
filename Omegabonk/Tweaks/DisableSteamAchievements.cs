using Il2CppAssets.Scripts.Steam;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk.Tweaks;

internal static class DisableSteamAchievements {
    internal static void Enable() {
        SteamAchievementsManager.ENABLED = false;
    }

    internal static void Disable() {
        SteamAchievementsManager.ENABLED = true;
    }
}
