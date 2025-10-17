using MelonLoader;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk;

internal static class MemoryUtils {
    private static readonly IntPtr _minPointerWindows = new IntPtr(0x140000000); //5368709120
    private static readonly IntPtr _minPointerUnix = new IntPtr(0x1000); //4096

    private static readonly IntPtr _maxPointerWindows = new IntPtr(0x7FFFFFFFFFFF); //140737488355327
    private static readonly IntPtr _maxPointerUnix = new IntPtr(0x7FFFFFFFFFFF); //140737488355327

    private static bool _isEmulatedByWine;

    static MemoryUtils() {
        CheckIfRunningUnderWine();
    }

    private static void CheckIfRunningUnderWine() {
        try {
            var ntdllHandle = System.Runtime.InteropServices.NativeLibrary.Load("ntdll.dll");
            if (ntdllHandle == IntPtr.Zero) {
                Melon<OmegabonkMod>.Logger.Msg($"[{nameof(MemoryUtils)}.{nameof(CheckIfRunningUnderWine)}] Not running on Windows.");
                return;
            }

            var wineGetVersionAddress = System.Runtime.InteropServices.NativeLibrary.GetExport(ntdllHandle, "wine_get_version");
            if (wineGetVersionAddress == IntPtr.Zero) {
                Melon<OmegabonkMod>.Logger.Msg($"[{nameof(MemoryUtils)}.{nameof(CheckIfRunningUnderWine)}] Not running under Wine.");
                return;
            }

            _isEmulatedByWine = true;
            Melon<OmegabonkMod>.Logger.Msg($"[{nameof(MemoryUtils)}.{nameof(CheckIfRunningUnderWine)}] Wine emulation detected.");
        } catch (Exception e) {
            Melon<OmegabonkMod>.Logger.Error(e);
        }
    }

    internal static bool IsValid(this IntPtr intPtr) {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !_isEmulatedByWine) {
            return IsValidWindows(intPtr);
        }

        return IsValidUnix(intPtr);
    }

    private static bool IsValidWindows(this IntPtr intPtr) {
        return intPtr.ToInt64() >= _minPointerWindows.ToInt64() && intPtr.ToInt64() <= _maxPointerWindows.ToInt64();
    }

    private static bool IsValidUnix(this IntPtr intPtr) {
        return intPtr.ToInt64() >= _minPointerUnix.ToInt64() && intPtr.ToInt64() <= _maxPointerUnix.ToInt64();
    }
}
