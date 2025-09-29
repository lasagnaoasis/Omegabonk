using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts._Data.ShopItems;
using Il2CppAssets.Scripts.Inventory__Items__Pickups;
using Il2CppAssets.Scripts.Saves___Serialization.Progression.Unlocks;
using Il2CppAssets.Scripts.Steam;

using MelonLoader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Omegabonk.Tweaks;

//int InventoryUtility.GetNumMaxTomeSlots()
//[HarmonyPatch(typeof(InventoryUtility), nameof(InventoryUtility.GetNumMaxTomeSlots), new Type[] { })]
internal static class MoreTomeAndWeaponSlots {
    private static int OriginalMaxTomeSlots { get; set; }
    private static int OriginalMaxWeaponSlots { get; set; }

    private static int AdditionalTomeSlots => Preferences.AdditionalTomeSlots.Value;
    private static int AdditionalWeaponSlots => Preferences.AdditionalWeaponSlots.Value;

    //private static void Postfix(ref int __result) {
    //    __result = _maxTomeSlots;
    //}

    internal static void Enable() {
        
    }

    internal static void Disable() {

    }

    internal static void OnMainMenuInitialized() {
        
    }

    internal static void OnLevelInitialized() {
        MelonCoroutines.Start(DelayedEditGameUi());
    }

    private static IEnumerator DelayedEditGameUi() {
        UiManager uiManager = null;
        var maxWait = 150;
        while (maxWait > 0) {
            uiManager = GameObject.FindFirstObjectByType<UiManager>();
            if (uiManager != null)
                break;

            yield return new WaitForSeconds(0.1f);

            maxWait--;
        }

        if (uiManager == null) {
            MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(DelayedEditGameUi)}] Failed to find UiManager!");
            yield break;
        }

        var gameUiGameObject = uiManager.gameObject;
        var hudGameObject = uiManager.hud;

        if (hudGameObject == null) {
            MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(DelayedEditGameUi)}] Failed to find HUD!");
            yield break;
        }

        var inventoryHud = hudGameObject.GetComponentInChildren<InventoryHud>(true);
        if (inventoryHud == null) {
            MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(DelayedEditGameUi)}] Failed to find InventoryHud!");
            yield break;
        }


    }

    //void DataManager.Load()
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Load), new Type[] { })]
    internal static class EditDataManagerPatch {
        private static void Postfix(DataManager __instance) {
            var shopItems = __instance.shopItems;
            foreach (var shopItem in shopItems) {
                var eShopItem = shopItem.Key;
                var shopItemData = shopItem.Value;
                switch (eShopItem) {
                    case EShopItem.Weapons:
                        OriginalMaxWeaponSlots = shopItemData.maxLevel;
                        shopItemData.maxLevel += AdditionalWeaponSlots;
                        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditDataManagerPatch)}.{nameof(Postfix)}] Weapon Max Level: {shopItemData.maxLevel}");
                        break;
                    case EShopItem.Tomes:
                        OriginalMaxTomeSlots = shopItemData.maxLevel;
                        shopItemData.maxLevel += AdditionalTomeSlots;
                        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditDataManagerPatch)}.{nameof(Postfix)}] Tome Max Level: {shopItemData.maxLevel}");
                        break;
                    default:
                        break;
                }
            }

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

                        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] TL: {DataManager.Instance.shopItems[EShopItem.Tomes].maxLevel} ({DataManager.Instance.shopItems[EShopItem.Tomes].GetMaxLevel()}) ({DataManager.Instance.shopItems[EShopItem.Tomes].IsMaxLevel()}) ({SaveManager.Instance.progression.shopItems[EShopItem.Tomes]})");
                        shopContainer.RefreshLevel(true);
                        MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditShopWindowPatch2)}.{nameof(DelayedStart)}] TL: {DataManager.Instance.shopItems[EShopItem.Tomes].maxLevel} ({DataManager.Instance.shopItems[EShopItem.Tomes].GetMaxLevel()}) ({DataManager.Instance.shopItems[EShopItem.Tomes].IsMaxLevel()}) ({SaveManager.Instance.progression.shopItems[EShopItem.Tomes]})");

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
            MelonLogger.Msg($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(Postfix)}] InventoryHud Start");

            MelonCoroutines.Start(DelayedStart(__instance));
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
                MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] ItemContainerPrefab null!");
                yield break;
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

                instance.Refresh();
                weaponInventoryItemPrefabUi.RefreshEnabled(true);
                instance.Refresh();
                weaponInventoryItemPrefabUi.RefreshEnabled(true);

                if (lockedWeaponSlots > 0) {
                    var lockedOverlayTransform = newWeaponSlot.transform.Find("LockedOverlay");
                    if (lockedOverlayTransform == null) {
                        MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] W LockedOverlayTransform null!");
                        continue;
                    }

                    lockedOverlayTransform.gameObject.SetActive(true);

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

                instance.Refresh();
                tomeInventoryItemPrefabUi.RefreshEnabled(true);
                instance.Refresh();
                tomeInventoryItemPrefabUi.RefreshEnabled(true);

                if (lockedTomeSlots > 0) {
                    var lockedOverlayTransform = newTomeSlot.transform.Find("LockedOverlay");
                    if (lockedOverlayTransform == null) {
                        MelonLogger.Error($"[{nameof(MoreTomeAndWeaponSlots)}.{nameof(EditInventoryHudPatch2)}.{nameof(DelayedStart)}] T LockedOverlayTransform null!");
                        continue;
                    }

                    lockedOverlayTransform.gameObject.SetActive(true);

                    lockedTomeSlots--;
                }
            }
        }
    }

    //int ShopItemData.GetMaxLevel()
    [HarmonyPatch(typeof(ShopItemData), nameof(ShopItemData.GetMaxLevel), new Type[] { })]
    internal static class EditShopItemDataPatch1 {
        private static void Postfix(ShopItemData __instance, ref int __result) {
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
}
