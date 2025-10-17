using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts._Data.ShopItems;
using Il2CppAssets.Scripts._Data.Tomes;
using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Items;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Stats;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Weapons;
using Il2CppAssets.Scripts.Menu.Shop;
using Il2CppAssets.Scripts.Saves___Serialization.Progression.Unlocks;
using Il2CppAssets.Scripts.Steam;
using Il2CppAssets.Scripts.UI;

using Il2CppInterop.Common;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Runtime;
using Il2CppInterop.Runtime.Runtime.VersionSpecific.MethodInfo;

using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Diagnostics;

using MelonLoader;
using MelonLoader.CoreClrUtils;
using MelonLoader.NativeUtils;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static MelonLoader.MelonLogger;

namespace Omegabonk.Tweaks;

internal static class MoreTomeAndWeaponSlots {
    private static int OriginalMaxTomeSlots { get; set; }
    private static int OriginalMaxWeaponSlots { get; set; }

    internal static bool Enabled => Preferences.Initialized && Preferences.EnableMoreTomesAndWeapons.Value;
    internal static int AdditionalTomeSlots => Preferences.AdditionalTomeSlots.Value;
    internal static int AdditionalWeaponSlots => Preferences.AdditionalWeaponSlots.Value;

    internal static bool WillTriggerAntiCheat() => Enabled && (AdditionalTomeSlots > 0 || AdditionalWeaponSlots > 0);

    private static bool _dataManagerLoaded = false;

    private unsafe delegate int GetNodeTypeDelegate(IntPtr instance, Il2CppMethodInfo* methodInfo);
    private static GetNodeTypeDelegate _getNodeTypePatchDelegate;
    private static NativeHook<GetNodeTypeDelegate> _getNodeTypeHook;
    private const int GetNodeTypeDefaultReturnValue = 4;

    internal static unsafe void OnLateInitializeMelon() {
        if (!Enabled)
            return;

        //var getNumMaxTomeSlotsMethodPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(InventoryUtility).GetMethod(nameof(InventoryUtility.GetNumMaxTomeSlots))).GetValue(null);
        //var getNumMaxWeaponSlotsMethodPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(InventoryUtility).GetMethod(nameof(InventoryUtility.GetNumMaxWeaponSlots))).GetValue(null);

        //var inventoryHudRefreshMethodPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(InventoryHud).GetMethod(nameof(InventoryHud.Refresh))).GetValue(null);
        //var upgradeInventoryUiRefreshMethodPointer = *(IntPtr*)(IntPtr)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(UpgradeInventoryUI).GetMethod(nameof(UpgradeInventoryUI.Refresh))).GetValue(null);

        //Melon<OmegabonkMod>.Logger.Msg($"{getNumMaxTomeSlotsMethodPointer} {getNumMaxWeaponSlotsMethodPointer} {inventoryHudRefreshMethodPointer} {upgradeInventoryUiRefreshMethodPointer}");

        NativeHookGetNodeType();
    }

    //https://melonwiki.xyz/#/modders/patching?id=patching-using-native-hooks
    private static unsafe void NativeHookGetNodeType() {
        IntPtr originalMethod = *(IntPtr*)(IntPtr)Il2CppInteropUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(InventoryUtility).GetMethod(nameof(InventoryUtility.GetNumMaxTomeSlots))).GetValue(null);

        _getNodeTypePatchDelegate = GetNodeType;

        IntPtr delegatePointer = Marshal.GetFunctionPointerForDelegate(_getNodeTypePatchDelegate);

        NativeHook<GetNodeTypeDelegate> hook = new NativeHook<GetNodeTypeDelegate>(originalMethod, delegatePointer);

        hook.Attach();

        _getNodeTypeHook = hook;
    }

    private unsafe static int GetNodeType(IntPtr instance, Il2CppMethodInfo* methodInfo) {
        try {
            if (methodInfo == (Il2CppMethodInfo*)IntPtr.Zero)
                return _getNodeTypeHook.Trampoline(instance, methodInfo);
        } catch {
            return GetNodeTypeDefaultReturnValue;
        }

        try {
            var il2CppMethodInfo = UnityVersionHandler.Wrap(methodInfo);
            if (il2CppMethodInfo != null && il2CppMethodInfo.MethodInfoPointer != (Il2CppMethodInfo*)IntPtr.Zero) {
                //Melon<OmegabonkMod>.Logger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(GetNodeType)}] il2cppmi {(IntPtr)methodInfo} - mip {(IntPtr)il2CppMethodInfo.MethodInfoPointer} - mp {il2CppMethodInfo.MethodPointer} - np {il2CppMethodInfo.Name} - ip {il2CppMethodInfo.InvokerMethod}");
                if (!il2CppMethodInfo.MethodPointer.IsValid())
                    return _getNodeTypeHook.Trampoline(instance, methodInfo);

                var methodNameIntPtr = il2CppMethodInfo.Name;
                if (methodNameIntPtr == (IntPtr)methodInfo)
                    return _getNodeTypeHook.Trampoline(instance, methodInfo);

                var methodName = string.Empty;
                if (methodNameIntPtr.IsValid())
                    methodName = Marshal.PtrToStringAnsi(methodNameIntPtr);

                //var il2CppClass = UnityVersionHandler.Wrap(il2CppMethodInfo.Class);
                //if (il2CppClass != null && (IntPtr)il2CppClass.ClassPointer != IntPtr.Zero) {
                //    var classNamespaceIntPtr = il2CppClass.Namespace;
                //    var classNameIntPtr = il2CppClass.Name;
                //    var classNamespace = string.Empty;
                //    var className = string.Empty;
                //    if (classNamespaceIntPtr != IntPtr.Zero)
                //        classNamespace = Marshal.PtrToStringAnsi(classNamespaceIntPtr);
                //    if (classNameIntPtr != IntPtr.Zero)
                //        className = Marshal.PtrToStringAnsi(classNameIntPtr);
                //}

                if (!string.IsNullOrWhiteSpace(methodName)) {
                    //Melon<OmegabonkMod>.Logger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(GetNodeType)}] Caller: {methodName}");

                    if (methodName.Equals(nameof(InventoryUtility.GetNumMaxTomeSlots))) {
                        return _getNodeTypeHook.Trampoline(instance, methodInfo) + AdditionalTomeSlots;
                    } else if (methodName.Equals(nameof(InventoryUtility.GetNumMaxWeaponSlots))) {
                        return _getNodeTypeHook.Trampoline(instance, methodInfo) + AdditionalWeaponSlots;
                    }
                }
            }
        } catch {
            return GetNodeTypeDefaultReturnValue;
        }

        try {
            return _getNodeTypeHook.Trampoline(instance, methodInfo);
        } catch {
            return GetNodeTypeDefaultReturnValue;
        }
    }

    //void DataManager.Load()
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Load), new Type[] { })]
    internal static class EditDataManagerPatch {
        private static void Postfix(DataManager __instance) {
            if (!Enabled || (AdditionalWeaponSlots == 0 && AdditionalWeaponSlots == 0))
                return;

            var shopItems = __instance.shopItems;
            foreach (var shopItem in shopItems) {
                var eShopItem = shopItem.Key;
                var shopItemData = shopItem.Value;
                EditShopItemData(eShopItem, shopItemData);
            }

            _dataManagerLoaded = true;

            //var unsortedShopItems = __instance.unsortedShopItems;
            //foreach (var unsortedShopItem in unsortedShopItems) {
            //    switch (unsortedShopItem.eShopItem) {
            //        case Il2CppAssets.Scripts._Data.ShopItems.EShopItem.Weapons:
            //            unsortedShopItem.maxLevel += AdditionalWeaponSlots;
            //            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditDataManagerPatch)}.{nameof(Postfix)}] Unsorted Weapon Max Level: {unsortedShopItem.maxLevel}");
            //            break;
            //        case Il2CppAssets.Scripts._Data.ShopItems.EShopItem.Tomes:
            //            unsortedShopItem.maxLevel += AdditionalTomeSlots;
            //            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditDataManagerPatch)}.{nameof(Postfix)}] Unsorted Tome Max Level: {unsortedShopItem.maxLevel}");
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private static void EditShopItemData(EShopItem eShopItem, ShopItemData shopItemData) {
            switch (eShopItem) {
                case EShopItem.Weapons: {
                    OriginalMaxWeaponSlots = shopItemData.maxLevel;
                    shopItemData.maxLevel += AdditionalWeaponSlots;
                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditDataManagerPatch)}.{nameof(EditShopItemData)}] Changed {eShopItem} maxLevel from {OriginalMaxWeaponSlots} to {shopItemData.maxLevel}");
                    break;
                }
                case EShopItem.Tomes: {
                    OriginalMaxTomeSlots = shopItemData.maxLevel;
                    shopItemData.maxLevel += AdditionalTomeSlots;
                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditDataManagerPatch)}.{nameof(EditShopItemData)}] Changed {eShopItem} maxLevel from {OriginalMaxTomeSlots} to {shopItemData.maxLevel}");
                    break;
                }
                default:
                    break;
            }
        }
    }

    ////void ShopWindow.Awake()
    //[HarmonyPatch(typeof(ShopWindow), nameof(ShopWindow.Awake), new Type[] { })]
    //internal static class EditShopWindowPatch1 {
    //    private static void Postfix(ShopWindow __instance) {
    //        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch1)}.{nameof(Postfix)}] ShopWindow Awake");
    //    }
    //}

    //void ShopWindow.Start()
    [HarmonyPatch(typeof(ShopWindow), nameof(ShopWindow.Start), new Type[] { })]
    internal static class EditShopWindowPatch2 {
        private static void Postfix(ShopWindow __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(Postfix)}] ShopWindow Start");

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(ShopWindow instance) {
            yield return new WaitForEndOfFrame();

            var shopContainers = instance.shopContainers;
            if (shopContainers is not { Count: > 0 }) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] No ShopContainers found!");
                yield break;
            }

            foreach (var shopContainer in shopContainers) {
                var shopItemData = shopContainer.data;
                if (shopItemData == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] ShopItemData null!");
                    continue;
                }

                //var shopContainerGameObject = shopContainer.gameObject;
                //MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] GO name: {shopContainerGameObject.name}");

                var levelsTransform = shopContainer.levelsParent;
                if (levelsTransform == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] LevelsTransform null!");
                    continue;
                }

                switch (shopItemData.eShopItem) {
                    case EShopItem.Weapons: {
                        var weaponCounter = 0;
                        var newMaxWeaponSlots = shopItemData.maxLevel;
                        foreach (var transformObject in levelsTransform) {
                            if (weaponCounter == newMaxWeaponSlots)
                                break;

                            var childTransform = transformObject.Cast<Transform>();
                            var childGameObject = childTransform.gameObject;
                            if (!childGameObject.activeSelf)
                                childGameObject.SetActive(true);

                            weaponCounter++;
                        }

                        shopContainer.RefreshLevel(true);

                        break;
                    }
                    case EShopItem.Tomes: {
                        var tomeCounter = 0;
                        var newMaxWeaponSlots = shopItemData.maxLevel;
                        foreach (var transformObject in levelsTransform) {
                            if (tomeCounter == newMaxWeaponSlots)
                                break;

                            var childTransform = transformObject.Cast<Transform>();
                            var childGameObject = childTransform.gameObject;
                            if (!childGameObject.activeSelf)
                                childGameObject.SetActive(true);

                            tomeCounter++;
                        }

                        shopContainer.RefreshLevel(true);

                        break;
                    }
                    default:
                        break;
                }

                //instance.RefreshPrices();
            }
        }
    }

    ////void MainMenu.GoToShop()
    //[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.GoToShop), new Type[] { })]
    //internal static class EditMainMenuPatch1 {
    //    private static void Postfix(MainMenu __instance) {
    //        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditMainMenuPatch1)}.{nameof(Postfix)}] MainMenu GoToShop");

    //    }
    //}

    ////void ShopContainer.Set(ShopItemData shopItemData)
    //[HarmonyPatch(typeof(ShopContainer), nameof(ShopContainer.Set), new Type[] { typeof(ShopItemData) })]
    //internal static class EditShopContainerPatch1 {
    //    private static void Postfix(ShopContainer __instance, ShopItemData shopItemData) {
    //        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopContainerPatch1)}.{nameof(Postfix)}] ShopContainer Set");

    //    }
    //}

    ////void InventoryHud.OnInventoryInit(PlayerInventory obj)
    //[HarmonyPatch(typeof(InventoryHud), nameof(InventoryHud.OnInventoryInit), new Type[] { typeof(PlayerInventory) })]
    //internal static class EditInventoryHudPatch1 {
    //    private static void Postfix(InventoryHud __instance, PlayerInventory obj) {
    //        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch1)}.{nameof(Postfix)}] InventoryHud OnInventoryInit");

    //    }
    //}

    //void InventoryHud.Start()
    [HarmonyPatch(typeof(InventoryHud), nameof(InventoryHud.Start), new Type[] { })]
    internal static class EditInventoryHudPatch2 {
        private static void Postfix(InventoryHud __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(Postfix)}] InventoryHud Start");

            //MelonCoroutines.Start(DelayedStart(__instance));

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(Postfix)}] Tomes | Available: {InventoryUtility.GetNumAvailableTomeSlots()}, Max: {InventoryUtility.GetNumMaxTomeSlots()}");
            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(Postfix)}] Weapons | Available: {InventoryUtility.GetNumAvailableWeaponSlots()}, Max: {InventoryUtility.GetNumMaxWeaponSlots()}");
        }

        private static IEnumerator DelayedStart(InventoryHud instance) {
            yield return new WaitForEndOfFrame();

            var weaponsParentTransform = instance.weaponParent;
            var tomesParentTransform = instance.tomeParent;
            if (weaponsParentTransform == null || tomesParentTransform == null) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] Weapon or Tome parent transform null!");
                yield break;
            }

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] {weaponsParentTransform.gameObject.name} {tomesParentTransform.gameObject.name}");

            var weaponContainers = instance.weaponContainers;
            var tomeContainers = instance.tomeContainers;

            if (weaponContainers == null || tomeContainers == null) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] Weapon or Tome containers null!");
                yield break;
            }

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] W: {weaponContainers.Count}, T: {tomeContainers.Count}");

            //var itemContainerPrefab = instance.itemContainerPrefab;
            GameObject itemContainerPrefab = null;
            foreach (var childTransformObject in weaponsParentTransform) {
                var childTransform = childTransformObject.Cast<Transform>();
                var childGameObject = childTransform.gameObject;
                var childInventoryItemPrefabUi = childGameObject.GetComponent<InventoryItemPrefabUI>();
                //var iconTransform = childTransform.Find("Icon");
                //var iconGameObject = iconTransform.gameObject;
                if (childGameObject.activeSelf && childInventoryItemPrefabUi != null && childInventoryItemPrefabUi.item == null) {
                    itemContainerPrefab = childGameObject;
                    break;
                }
            }

            if (itemContainerPrefab == null) {
                itemContainerPrefab = instance.itemContainerPrefab; //Fallback
                if (itemContainerPrefab == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] ItemContainerPrefab null!");
                    yield break;
                }
            }

            var lockedWeaponSlots = DataManager.Instance.shopItems[EShopItem.Weapons].GetMaxLevel() - SaveManager.Instance.progression.shopItems[EShopItem.Weapons];
            for (int i = 0; i < AdditionalWeaponSlots; i++) {
                var newWeaponSlot = GameObject.Instantiate(itemContainerPrefab, weaponsParentTransform);
                newWeaponSlot.SetActive(true);

                var weaponInventoryItemPrefabUi = newWeaponSlot.GetComponent<InventoryItemPrefabUI>();
                if (weaponInventoryItemPrefabUi == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] W InventoryItemPrefabUI null!");
                    yield break;
                }

                weaponContainers.Add(weaponInventoryItemPrefabUi);

                for (int x = 0; x < 2; x++) {
                    try {
                        instance.Refresh();

                        weaponInventoryItemPrefabUi.RefreshEnabled(true);
                    } catch {

                    }
                }

                if (lockedWeaponSlots > 0 && i >= (AdditionalWeaponSlots - lockedWeaponSlots)) {
                    //var lockedOverlayTransform = newWeaponSlot.transform.Find("LockedOverlay");
                    //if (lockedOverlayTransform == null) {
                    //    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] W LockedOverlayTransform null!");
                    //    continue;
                    //}

                    //lockedOverlayTransform.gameObject.SetActive(true);
                    weaponInventoryItemPrefabUi.SetUnavailable();

                    lockedWeaponSlots--;
                }
            }

            var lockedTomeSlots = DataManager.Instance.shopItems[EShopItem.Tomes].GetMaxLevel() - SaveManager.Instance.progression.shopItems[EShopItem.Tomes];
            for (int j = 0; j < AdditionalTomeSlots; j++) {
                var newTomeSlot = GameObject.Instantiate(itemContainerPrefab, tomesParentTransform);
                newTomeSlot.SetActive(true);

                var tomeInventoryItemPrefabUi = newTomeSlot.GetComponent<InventoryItemPrefabUI>();
                if (tomeInventoryItemPrefabUi == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] T InventoryItemPrefabUI null!");
                    yield break;
                }

                tomeContainers.Add(tomeInventoryItemPrefabUi);

                for (int y = 0; y < 2; y++) {
                    try {
                        instance.Refresh();

                        tomeInventoryItemPrefabUi.RefreshEnabled(true);
                    } catch {

                    }
                }

                if (lockedTomeSlots > 0 && j >= (AdditionalTomeSlots - lockedTomeSlots)) {
                    //var lockedOverlayTransform = newTomeSlot.transform.Find("LockedOverlay");
                    //if (lockedOverlayTransform == null) {
                    //    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] T LockedOverlayTransform null!");
                    //    continue;
                    //}

                    //lockedOverlayTransform.gameObject.SetActive(true);
                    tomeInventoryItemPrefabUi.SetUnavailable();

                    lockedTomeSlots--;
                }
            }
        }
    }

    //void InventoryHud.Refresh()
    [HarmonyPatch(typeof(InventoryHud), nameof(InventoryHud.Refresh), new Type[] { })]
    internal static class EditInventoryHudPatch3 {
        private static bool Prefix(InventoryHud __instance) {
            Refresh(__instance);
            //MelonCoroutines.Start(DelayedRefresh(__instance));

            return false;
        }

        //private static IEnumerator DelayedRefresh(InventoryHud instance) {
        //    yield return new WaitForEndOfFrame();

        //    try {
        //        Refresh(instance);
        //    } catch (Exception e) {
        //        Melon<OmegabonkMod>.Logger.Error(e);
        //    }
        //}

        //this aims to be a 1:1 copy of the native code
        //void InventoryHud$$Refresh(InventoryHud_o *__this,MethodInfo *method)
        private static void Refresh(InventoryHud instance) {
            var myPlayer = MyPlayer.Instance;
            if (myPlayer == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] myPlayer null");
                return;
            }

            var playerInventory = myPlayer.inventory;
            if (playerInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] playerInventory null");
                return;
            }

            var tomeInventory = playerInventory.tomeInventory;
            if (tomeInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeInventory null");
                return;
            }

            var weaponInventory = playerInventory.weaponInventory;
            if (weaponInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponInventory null");
                return;
            }

            var tomeLevels = tomeInventory.tomeLevels;
            if (tomeLevels == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevels null");
                return;
            }

            var tomeLevelsKeys = tomeLevels.Keys;
            if (tomeLevelsKeys == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeys null");
                return;
            }

            var tomeLevelsKeysAsEnumerable = tomeLevelsKeys.Cast<Il2CppSystem.Collections.Generic.IEnumerable<ETome>>();
            if (tomeLevelsKeysAsEnumerable == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeysAsEnumerable null");
                return;
            }

            var tomeLevelsKeysList = new Il2CppSystem.Collections.Generic.List<ETome>(tomeLevelsKeysAsEnumerable);
            if (tomeLevelsKeysList == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeysList null");
                return;
            }

            var weapons = weaponInventory.weapons;
            if (weapons == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weapons null");
                return;
            }

            var weaponsValues = weapons.Values;
            if (weaponsValues == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValues null");
                return;
            }

            var weaponsValuesAsEnumerable = weaponsValues.Cast<Il2CppSystem.Collections.Generic.IEnumerable<WeaponBase>>();
            if (weaponsValuesAsEnumerable == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValuesAsEnumerable null");
                return;
            }

            var weaponsValuesList = new Il2CppSystem.Collections.Generic.List<WeaponBase>(weaponsValuesAsEnumerable);
            if (weaponsValuesList == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValuesList null");
                return;
            }

            var availableTomeSlots = InventoryUtility.GetNumAvailableTomeSlots();
            var availableWeaponSlots = InventoryUtility.GetNumAvailableWeaponSlots();
            var maxTomeSlots = InventoryUtility.GetNumMaxTomeSlots();
            var maxWeaponSlots = InventoryUtility.GetNumMaxWeaponSlots();

            var tomeContainers = instance.tomeContainers;
            if (tomeContainers == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeContainers null");
                return;
            }

            var weaponContainers = instance.weaponContainers;
            if (weaponContainers == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponContainers null");
                return;
            }

            var itemContainerPrefab = instance.itemContainerPrefab;
            if (itemContainerPrefab == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemContainerPrefab null");
                return;
            }

            var tomeParent = instance.tomeParent;
            if (tomeParent == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeParent null");
                return;
            }

            var weaponParent = instance.weaponParent;
            if (weaponParent == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponParent null");
                return;
            }

            var dataManager = DataManager.Instance;
            if (dataManager == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] dataManager null");
                return;
            }

            var tomeSlots = 0;
            while (maxTomeSlots > tomeSlots) {
                if (tomeContainers.Count <= tomeSlots) {
                    var tomeItemContainerGameObject = GameObject.Instantiate(itemContainerPrefab, tomeParent);
                    var tomeInventoryItemPrefabUi = tomeItemContainerGameObject.GetComponent<InventoryItemPrefabUI>();
                    tomeContainers.Add(tomeInventoryItemPrefabUi);
                    tomeItemContainerGameObject.SetActive(true);
                }

                var currentTomeInventoryItemPrefabUi = tomeContainers[tomeSlots];
                if (currentTomeInventoryItemPrefabUi == null) {
                    Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] currentTomeInventoryItemPrefabUi null");
                    break;
                }

                if (availableTomeSlots > tomeSlots) {
                    if (tomeLevelsKeysList.Count > tomeSlots) {
                        var eTome = tomeLevelsKeysList[tomeSlots];
                        var tomeData = dataManager.GetTome(eTome);
                        currentTomeInventoryItemPrefabUi.SetItem(tomeData);
                    } else {
                        currentTomeInventoryItemPrefabUi.SetItem(null);
                    }
                } else {
                    //currentTomeInventoryItemPrefabUi.gameObject.SetActive(false);
                    currentTomeInventoryItemPrefabUi.SetUnavailable();
                }

                tomeSlots++;
            }

            var weaponSlots = 0;
            while (maxWeaponSlots > weaponSlots) {
                if (weaponContainers.Count <= weaponSlots) {
                    var weaponItemContainerGameObject = GameObject.Instantiate(itemContainerPrefab, weaponParent);
                    var weaponInventoryItemPrefabUi = weaponItemContainerGameObject.GetComponent<InventoryItemPrefabUI>();
                    weaponContainers.Add(weaponInventoryItemPrefabUi);
                    weaponItemContainerGameObject.SetActive(true);
                }

                var currentWeaponInventoryItemPrefabUi = weaponContainers[weaponSlots];
                if (currentWeaponInventoryItemPrefabUi == null) {
                    Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] currentWeaponInventoryItemPrefabUi null");
                    break;
                }

                if (availableWeaponSlots > weaponSlots) {
                    if (weaponsValuesList.Count > weaponSlots) {
                        var weaponBase = weaponsValuesList[weaponSlots];
                        var weaponData = weaponBase.weaponData;
                        currentWeaponInventoryItemPrefabUi.SetItem(weaponData);
                    } else {
                        currentWeaponInventoryItemPrefabUi.SetItem(null);
                    }
                } else {
                    //currentTomeInventoryItemPrefabUi.gameObject.SetActive(false);
                    currentWeaponInventoryItemPrefabUi.SetUnavailable();
                }

                weaponSlots++;
            }

            var transform = instance.transform;
            if (transform == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] transform null");
                return;
            }

            UiUtility.RebuildUi(transform);
        }
    }

    //void UpgradeInventoryUI.Start()
    [HarmonyPatch(typeof(UpgradeInventoryUI), nameof(UpgradeInventoryUI.OnEnable), new Type[] { })]
    internal static class EditUpgradeInventoryUiPatch1 {
        private static void Postfix(UpgradeInventoryUI __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(Postfix)}] UpgradeInventoryUI OnEnable");

            //MelonCoroutines.Start(DelayedOnEnable(__instance));
        }

        private static IEnumerator DelayedOnEnable(UpgradeInventoryUI instance) {
            yield return new WaitForEndOfFrame();

            var weaponContainers = instance.weaponContainers;
            if (weaponContainers is not { Count: > 0 }) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] UpgradeInventoryUI WeaponContainers null or empty!");
                yield break;
            }

            if (weaponContainers.Count >= (4 + AdditionalWeaponSlots))
                yield break;

            var weaponsParentTransform = instance.weaponParent;
            if (weaponsParentTransform == null) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] UpgradeInventoryUI WeaponsParentTransform null!");
                yield break;
            }

            GameObject itemContainerPrefab = null;
            foreach (var childTransformObject in weaponsParentTransform) {
                var childTransform = childTransformObject.Cast<Transform>();
                var childGameObject = childTransform.gameObject;
                var childInventoryItemPrefabUi = childGameObject.GetComponent<InventoryItemPrefabUI>();
                if (childGameObject.activeSelf && childInventoryItemPrefabUi != null && childInventoryItemPrefabUi.item == null) {
                    itemContainerPrefab = childGameObject;
                    break;
                }
            }

            if (itemContainerPrefab == null) {
                itemContainerPrefab = instance.itemContainerPrefab; //Fallback
                if (itemContainerPrefab == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] ItemContainerPrefab null!");
                    yield break;
                }
            }

            var lockedWeaponSlots = DataManager.Instance.shopItems[EShopItem.Weapons].GetMaxLevel() - SaveManager.Instance.progression.shopItems[EShopItem.Weapons];
            for (int i = 0; i < AdditionalWeaponSlots; i++) {
                var newWeaponSlot = GameObject.Instantiate(itemContainerPrefab, weaponsParentTransform);
                newWeaponSlot.SetActive(true);

                var weaponInventoryItemPrefabUi = newWeaponSlot.GetComponent<InventoryItemPrefabUI>();
                if (weaponInventoryItemPrefabUi == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] W InventoryItemPrefabUI null!");
                    yield break;
                }

                weaponContainers.Add(weaponInventoryItemPrefabUi);

                for (int x = 0; x < 2; x++) {
                    try {
                        instance.Refresh();

                        weaponInventoryItemPrefabUi.RefreshEnabled(true);
                    } catch {

                    }
                }

                if (lockedWeaponSlots > 0 && i >= (AdditionalWeaponSlots - lockedWeaponSlots)) {
                    weaponInventoryItemPrefabUi.SetUnavailable();

                    lockedWeaponSlots--;
                }
            }

            var tomeContainers = instance.tomeContainers;
            if (tomeContainers is not { Count: > 0 }) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] UpgradeInventoryUI TomeContainers null or empty!");
                yield break;
            }

            var tomesParentTransform = instance.tomeParent;
            if (tomesParentTransform == null) {
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] UpgradeInventoryUI TomesParentTransform null!");
                yield break;
            }

            var lockedTomeSlots = DataManager.Instance.shopItems[EShopItem.Tomes].GetMaxLevel() - SaveManager.Instance.progression.shopItems[EShopItem.Tomes];
            for (int j = 0; j < AdditionalTomeSlots; j++) {
                var newTomeSlot = GameObject.Instantiate(itemContainerPrefab, tomesParentTransform);
                newTomeSlot.SetActive(true);

                var tomeInventoryItemPrefabUi = newTomeSlot.GetComponent<InventoryItemPrefabUI>();
                if (tomeInventoryItemPrefabUi == null) {
                    MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditUpgradeInventoryUiPatch1)}.{nameof(DelayedOnEnable)}] T InventoryItemPrefabUI null!");
                    yield break;
                }

                tomeContainers.Add(tomeInventoryItemPrefabUi);

                for (int y = 0; y < 2; y++) {
                    try {
                        instance.Refresh();

                        tomeInventoryItemPrefabUi.RefreshEnabled(true);
                    } catch {

                    }
                }

                if (lockedTomeSlots > 0 && j >= (AdditionalTomeSlots - lockedTomeSlots)) {
                    tomeInventoryItemPrefabUi.SetUnavailable();

                    lockedTomeSlots--;
                }
            }
        }
    }

    //void UpgradeInventoryUI.Refresh()
    [HarmonyPatch(typeof(UpgradeInventoryUI), nameof(UpgradeInventoryUI.Refresh), new Type[] { })]
    internal static class EditUpgradeInventoryUiPatch2 {
        private static bool Prefix(UpgradeInventoryUI __instance) {
            Refresh(__instance);

            return false;
        }

        private static void Refresh(UpgradeInventoryUI instance) {
            var myPlayer = MyPlayer.Instance;
            if (myPlayer == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] myPlayer null");
                return;
            }

            var playerInventory = myPlayer.inventory;
            if (playerInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] playerInventory null");
                return;
            }

            var tomeInventory = playerInventory.tomeInventory;
            if (tomeInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeInventory null");
                return;
            }

            var tomeLevels = tomeInventory.tomeLevels;
            if (tomeLevels == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevels null");
                return;
            }

            var tomeLevelsKeys = tomeLevels.Keys;
            if (tomeLevelsKeys == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeys null");
                return;
            }

            var tomeLevelsKeysAsEnumerable = tomeLevelsKeys.Cast<Il2CppSystem.Collections.Generic.IEnumerable<ETome>>();
            if (tomeLevelsKeysAsEnumerable == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeysAsEnumerable null");
                return;
            }

            var tomeLevelsKeysList = new Il2CppSystem.Collections.Generic.List<ETome>(tomeLevelsKeysAsEnumerable);
            if (tomeLevelsKeysList == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeLevelsKeysList null");
                return;
            }

            var weaponInventory = playerInventory.weaponInventory;
            if (weaponInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponInventory null");
                return;
            }

            var weapons = weaponInventory.weapons;
            if (weapons == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weapons null");
                return;
            }

            var weaponsValues = weapons.Values;
            if (weaponsValues == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValues null");
                return;
            }

            var weaponsValuesAsEnumerable = weaponsValues.Cast<Il2CppSystem.Collections.Generic.IEnumerable<WeaponBase>>();
            if (weaponsValuesAsEnumerable == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValuesAsEnumerable null");
                return;
            }

            var weaponsValuesList = new Il2CppSystem.Collections.Generic.List<WeaponBase>(weaponsValuesAsEnumerable);
            if (weaponsValuesList == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponsValuesList null");
                return;
            }

            var itemInventory = playerInventory.itemInventory;
            if (itemInventory == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemInventory null");
                return;
            }

            var items = itemInventory.items;
            if (items == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] items null");
                return;
            }

            var itemsKeys = items.Keys;
            if (itemsKeys == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemsKeys null");
                return;
            }

            var itemsKeysAsEnumerable = itemsKeys.Cast<Il2CppSystem.Collections.Generic.IEnumerable<EItem>>();
            if (itemsKeysAsEnumerable == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemsKeysAsEnumerable null");
                return;
            }

            var itemsKeysList = new Il2CppSystem.Collections.Generic.List<EItem>(itemsKeysAsEnumerable);
            if (itemsKeysList == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemsKeysList null");
                return;
            }

            var availableTomeSlots = InventoryUtility.GetNumAvailableTomeSlots();
            var availableWeaponSlots = InventoryUtility.GetNumAvailableWeaponSlots();
            var maxTomeSlots = InventoryUtility.GetNumMaxTomeSlots();
            var maxWeaponSlots = InventoryUtility.GetNumMaxWeaponSlots();

            var tomeContainers = instance.tomeContainers;
            if (tomeContainers == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeContainers null");
                return;
            }

            var weaponContainers = instance.weaponContainers;
            if (weaponContainers == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponContainers null");
                return;
            }

            var itemContainers = instance.itemContainers;
            if (itemContainers == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemContainers null");
                return;
            }

            var itemContainerPrefab = instance.itemContainerPrefab;
            if (itemContainerPrefab == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemContainerPrefab null");
                return;
            }

            var tomeParent = instance.tomeParent;
            if (tomeParent == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] tomeParent null");
                return;
            }

            var weaponParent = instance.weaponParent;
            if (weaponParent == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] weaponParent null");
                return;
            }

            var itemParent = instance.itemParent;
            if (itemParent == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] itemParent null");
                return;
            }

            var dataManager = DataManager.Instance;
            if (dataManager == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] dataManager null");
                return;
            }

            var tomeSlots = 0;
            while (maxTomeSlots > tomeSlots) {
                if (tomeContainers.Count <= tomeSlots) {
                    var tomeItemContainerGameObject = GameObject.Instantiate(itemContainerPrefab, tomeParent);
                    var tomeInventoryItemPrefabUi = tomeItemContainerGameObject.GetComponent<InventoryItemPrefabUI>();
                    tomeContainers.Add(tomeInventoryItemPrefabUi);
                    tomeItemContainerGameObject.SetActive(true);
                }

                var currentTomeInventoryItemPrefabUi = tomeContainers[tomeSlots];
                if (currentTomeInventoryItemPrefabUi == null) {
                    Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] currentTomeInventoryItemPrefabUi null");
                    break;
                }

                var currentTomeInventoryItemPrefabUiGameObject = currentTomeInventoryItemPrefabUi.gameObject;
                currentTomeInventoryItemPrefabUiGameObject.SetActive(true);

                if (availableTomeSlots > tomeSlots) {
                    if (tomeLevelsKeysList.Count > tomeSlots) {
                        var eTome = tomeLevelsKeysList[tomeSlots];
                        var tomeData = dataManager.GetTome(eTome);
                        currentTomeInventoryItemPrefabUi.SetItem(tomeData);
                    } else {
                        currentTomeInventoryItemPrefabUi.SetItem(null);
                    }
                } else {
                    //currentTomeInventoryItemPrefabUi.gameObject.SetActive(false);
                    currentTomeInventoryItemPrefabUi.SetUnavailable();
                }

                tomeSlots++;
            }

            var weaponSlots = 0;
            while (maxWeaponSlots > weaponSlots) {
                if (weaponContainers.Count <= weaponSlots) {
                    var weaponItemContainerGameObject = GameObject.Instantiate(itemContainerPrefab, weaponParent);
                    var weaponInventoryItemPrefabUi = weaponItemContainerGameObject.GetComponent<InventoryItemPrefabUI>();
                    weaponContainers.Add(weaponInventoryItemPrefabUi);
                    weaponItemContainerGameObject.SetActive(true);
                }

                var currentWeaponInventoryItemPrefabUi = weaponContainers[weaponSlots];
                if (currentWeaponInventoryItemPrefabUi == null) {
                    Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] currentWeaponInventoryItemPrefabUi null");
                    break;
                }

                var currentWeaponInventoryItemPrefabUiGameObject = currentWeaponInventoryItemPrefabUi.gameObject;
                currentWeaponInventoryItemPrefabUiGameObject.SetActive(true);

                if (availableWeaponSlots > weaponSlots) {
                    if (weaponsValuesList.Count > weaponSlots) {
                        var weaponBase = weaponsValuesList[weaponSlots];
                        var weaponData = weaponBase.weaponData;
                        currentWeaponInventoryItemPrefabUi.SetItem(weaponData);
                    } else {
                        currentWeaponInventoryItemPrefabUi.SetItem(null);
                    }
                } else {
                    //currentTomeInventoryItemPrefabUi.gameObject.SetActive(false);
                    currentWeaponInventoryItemPrefabUi.SetUnavailable();
                }

                weaponSlots++;
            }

            foreach (var itemInventoryItemPrefabUi in itemContainers) {
                var itemInventoryItemPrefabUiGameObject = itemInventoryItemPrefabUi.gameObject;
                itemInventoryItemPrefabUiGameObject.SetActive(false);
            }

            var itemSlots = 0;
            while (itemsKeysList.Count > itemSlots) {
                if (itemContainers.Count <= itemSlots) {
                    var itemItemContainerGameObject = GameObject.Instantiate(itemContainerPrefab, itemParent);
                    var itemInventoryItemPrefabUi = itemItemContainerGameObject.GetComponent<InventoryItemPrefabUI>();
                    itemContainers.Add(itemInventoryItemPrefabUi);
                    itemItemContainerGameObject.SetActive(true);
                }

                var currentItemInventoryItemPrefabUi = itemContainers[itemSlots];
                if (currentItemInventoryItemPrefabUi == null) {
                    Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] currentItemInventoryItemPrefabUi null");
                    break;
                }

                var eItem = itemsKeysList[itemSlots];
                currentItemInventoryItemPrefabUi.SetItem(eItem);
                var currentItemInventoryItemPrefabUiGameObject = currentItemInventoryItemPrefabUi.gameObject;
                currentItemInventoryItemPrefabUiGameObject.SetActive(true);

                itemSlots++;
            }

            var transform = instance.transform;
            if (transform == null) {
                Melon<OmegabonkMod>.Logger.Error($"[{nameof(Refresh)}] transform null");
                return;
            }

            UiUtility.RebuildUi(transform);
        }
    }

    //int ShopItemData.GetMaxLevel()
    [HarmonyPatch(typeof(ShopItemData), nameof(ShopItemData.GetMaxLevel), new Type[] { })]
    internal static class EditShopItemDataPatch1 {
        private static void Postfix(ShopItemData __instance, ref int __result) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopItemDataPatch1)}.{nameof(Postfix)}] ShopItemData GetMaxLevel");

            var eShopItem = __instance.eShopItem;
            switch (eShopItem) {
                case EShopItem.Weapons: {
                    var firstSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots");
                    var secondSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots2");
                    if (firstSlotUnlocked && secondSlotUnlocked)
                        __result += AdditionalWeaponSlots;
                    break;
                }
                case EShopItem.Tomes: {
                    var firstSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_tomeSlots");
                    var secondSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_tomeSlots2");
                    if (firstSlotUnlocked && secondSlotUnlocked)
                        __result += AdditionalTomeSlots;
                    break;
                }
                default:
                    break;
            }
        }
    }

    //bool ShopItemData.IsMaxLevel()
    [HarmonyPatch(typeof(ShopItemData), nameof(ShopItemData.IsMaxLevel), new Type[] { })]
    internal static class EditShopItemDataPatch2 {
        private static void Postfix(ShopItemData __instance, ref bool __result) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopItemDataPatch1)}.{nameof(Postfix)}] ShopItemData GetMaxLevel");

            var saveManager = SaveManager.Instance;
            var progression = saveManager.progression;
            var shopItemLevels = progression.shopItems;

            var eShopItem = __instance.eShopItem;
            switch (eShopItem) {
                case EShopItem.Weapons: {
                    var firstSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots");
                    var secondSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots2");
                    var newSlots = 2 + AdditionalWeaponSlots;
                    __result = firstSlotUnlocked && secondSlotUnlocked && (newSlots <= shopItemLevels[EShopItem.Weapons]);
                    break;
                }
                case EShopItem.Tomes: {
                    var firstSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_tomeSlots");
                    var secondSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_tomeSlots2");
                    var newSlots = 2 + AdditionalTomeSlots;
                    __result = firstSlotUnlocked && secondSlotUnlocked && (newSlots <= shopItemLevels[EShopItem.Tomes]);
                    break;
                }
                default:
                    break;
            }
        }
    }

    ////int InventoryUtility.GetNumMaxWeaponSlots()
    //[HarmonyPatch(typeof(InventoryUtility), nameof(InventoryUtility.GetNumMaxWeaponSlots), new Type[] { })]
    //private static class EditInventoryUtilityPatch1 {
    //    private static void Postfix(ref int __result) {
    //        if (!Enabled || !_dataManagerLoaded)
    //            return;

    //        //var slots = 0;
    //        //var firstSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots");
    //        //if (firstSlotUnlocked)
    //        //    slots++;
    //        //var secondSlotUnlocked = MyAchievements.IsUnlockedInternalNameAch("a_weaponSlots2");
    //        //if (secondSlotUnlocked)
    //        //    slots++;
    //        //slots += AdditionalWeaponSlots;
    //        //__result = slots;
    //    }
    //}
}
