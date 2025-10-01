using HarmonyLib;

using Il2Cpp;

using Il2CppAssets.Scripts._Data.ShopItems;

using MelonLoader;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using static Omegabonk.Tweaks.MoreRefreshesSkipsAndBanishes;

namespace Omegabonk.Tweaks;

internal static class MoreRefreshesSkipsAndBanishes {
    private static int OriginalRefreshes { get; set; }
    private static int OriginalSkips { get; set; }
    private static int OriginalBanishes { get; set; }

    internal static bool Enabled => Preferences.Initialized && Preferences.EnableMoreRefreshesSkipsAndBanishes.Value;
    internal static int AdditionalRefreshes => Preferences.AdditionalRefreshes.Value;
    internal static int AdditionalSkips => Preferences.AdditionalSkips.Value;
    internal static int AdditionalBanishes => Preferences.AdditionalBanishes.Value;

    internal static bool WillTriggerAntiCheat() => Enabled && (AdditionalRefreshes > 0 || AdditionalSkips > 0 || AdditionalBanishes > 0);

    //void DataManager.Load()
    [HarmonyPatch(typeof(DataManager), nameof(DataManager.Load), new Type[] { })]
    internal static class EditDataManagerPatch {
        private static void Postfix(DataManager __instance) {
            if (!Enabled)
                return;

            var shopItems = __instance.shopItems;
            foreach (var shopItem in shopItems) {
                var eShopItem = shopItem.Key;
                var shopItemData = shopItem.Value;
                EditShopItemData(eShopItem, shopItemData);
            }
        }

        private static void EditShopItemData(EShopItem eShopItem, ShopItemData shopItemData) {
            switch (eShopItem) {
                case EShopItem.Refresh: {
                    OriginalRefreshes = shopItemData.maxLevel;
                    shopItemData.maxLevel += AdditionalRefreshes;
                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditDataManagerPatch)}.{nameof(EditShopItemData)}] Changed {eShopItem} maxLevel from {OriginalRefreshes} to {shopItemData.maxLevel}");
                    break;
                }
                case EShopItem.Skip: {
                    OriginalSkips = shopItemData.maxLevel;
                    shopItemData.maxLevel += AdditionalSkips;
                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditDataManagerPatch)}.{nameof(EditShopItemData)}] Changed {eShopItem} maxLevel from {OriginalSkips} to {shopItemData.maxLevel}");
                    break;
                }
                case EShopItem.Banish: {
                    OriginalBanishes = shopItemData.maxLevel;
                    shopItemData.maxLevel += AdditionalBanishes;
                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditDataManagerPatch)}.{nameof(EditShopItemData)}] Changed {eShopItem} maxLevel from {OriginalBanishes} to {shopItemData.maxLevel}");
                    break;
                }
                default:
                    break;
            }
        }
    }

    //void ShopWindow.Start()
    [HarmonyPatch(typeof(ShopWindow), nameof(ShopWindow.Start), new Type[] { })]
    internal static class EditShopWindowPatch {
        private static void Postfix(ShopWindow __instance) {
            if (!Enabled)
                return;

            MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditShopWindowPatch)}.{nameof(Postfix)}] ShopWindow Start");

            MelonCoroutines.Start(DelayedStart(__instance));
        }

        private static IEnumerator DelayedStart(ShopWindow instance) {
            yield return new WaitForEndOfFrame();

            var shopContainers = instance.shopContainers;
            if (shopContainers is not { Count: > 0 }) {
                MelonLogger.Error($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditShopWindowPatch)}.{nameof(DelayedStart)}] No ShopContainers found!");
                yield break;
            }

            foreach (var shopContainer in shopContainers) {
                HandleShopContainer(shopContainer);
            }
        }

        private static void HandleShopContainer(ShopContainer shopContainer) {
            var shopItemData = shopContainer.data;
            if (shopItemData == null) {
                MelonLogger.Error($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditShopWindowPatch)}.{nameof(DelayedStart)}] ShopItemData null!");
                return;
            }

            var levelsTransform = shopContainer.levelsParent;
            if (levelsTransform == null) {
                MelonLogger.Error($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditShopWindowPatch)}.{nameof(DelayedStart)}] LevelsTransform null!");
                return;
            }

            var eShopItem = shopItemData.eShopItem;
            AddAdditionalShopSlots(eShopItem, shopItemData, levelsTransform, shopContainer);
        }

        private static void AddAdditionalShopSlots(EShopItem eShopItem, ShopItemData shopItemData, Transform levelsTransform, ShopContainer shopContainer) {
            switch (eShopItem) {
                case EShopItem.Refresh:
                case EShopItem.Skip:
                case EShopItem.Banish: {
                    var counter = 0;
                    var newSlots = shopItemData.maxLevel;

                    MelonLogger.Msg($"[{nameof(MoreRefreshesSkipsAndBanishes)}.{nameof(EditShopWindowPatch)}.{nameof(AddAdditionalShopSlots)}] {eShopItem} - {newSlots}");

                    foreach (var transformObject in levelsTransform) {
                        if (counter == newSlots)
                            break;

                        var childTransform = transformObject.Cast<Transform>();
                        var childGameObject = childTransform.gameObject;
                        if (!childGameObject.activeSelf)
                            childGameObject.SetActive(true);

                        counter++;
                    }

                    shopContainer.RefreshLevel(true);

                    break;
                }
                default:
                    break;
            }
        }
    }
}
