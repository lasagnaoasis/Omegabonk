using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omegabonk;

internal static class MemoryUtils {
    private static readonly IntPtr _minPointer = new IntPtr(0x140000000); //5368709120
    private static readonly IntPtr _maxPointer = new IntPtr(0x7FFFFFFFFFFF); //140737488355327

    //internal static bool IsValidPointer(IntPtr intPtr) {
    //    return intPtr.ToInt64() >= _minPointer.ToInt64() && intPtr.ToInt64() <= _maxPointer.ToInt64();
    //}

    internal static bool IsValid(this IntPtr intPtr) {
        return intPtr.ToInt64() >= _minPointer.ToInt64() && intPtr.ToInt64() <= _maxPointer.ToInt64();
    }
}
