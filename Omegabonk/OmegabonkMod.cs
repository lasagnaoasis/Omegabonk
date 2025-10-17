using Il2Cpp;

using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Inventory__Items__Pickups.Stats;
using Il2CppAssets.Scripts.Managers;
using Il2CppAssets.Scripts.Menu.Shop;
using Il2CppAssets.Scripts.Steam.LeaderboardsNew;

using Il2CppRewired;

using MelonLoader;

using Omegabonk;
using Omegabonk.Tweaks;

using System.Collections;
using System.Collections.Concurrent;

using UnityEngine;

[assembly: MelonInfo(typeof(OmegabonkMod), "Omegabonk", "0.1.6", "skatinglasagna")]
[assembly: MelonGame("Ved", "Megabonk")]

namespace Omegabonk {
    public class OmegabonkMod : MelonMod {
        internal static OmegabonkMod Instance;

        private ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();

        private SceneState _sceneState;

        public override void OnEarlyInitializeMelon() {
            Instance = this;

            Preferences.Setup();

            LoggerInstance.Msg($"{nameof(OmegabonkMod)} is built for Megabonk v1.0.12");
        }

        //public override void OnInitializeMelon() {

        //}

        public override void OnLateInitializeMelon() {
            if (MoreTomeAndWeaponSlots.Enabled)
                MoreTomeAndWeaponSlots.OnLateInitializeMelon();
        }

        //public override void OnSceneWasLoaded(int buildIndex, string sceneName) {

        //}

        public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
            switch (buildIndex) {
                case -1: //None
                    _sceneState = SceneState.None;
                    break;
                case 0: //Boot
                    _sceneState = SceneState.Boot;
                    OnBootInitialized();
                    break;
                case 1: //Main menu
                    _sceneState = SceneState.MainMenu;
                    OnMainMenuInitialized();
                    break;
                case 2: //Level
                    _sceneState = SceneState.Level;
                    OnLevelInitialized();
                    break;
                case 3:
                    _sceneState = SceneState.LoadingScreen;
                    OnLoadingScreenInitialized();
                    break;
            }
        }

        private void OnBootInitialized() {

        }

        private void OnMainMenuInitialized() {
            //DisableSteamAchievements.Enable();

            //if (MoreTomeAndWeaponSlots.Enabled)
            //    MoreTomeAndWeaponSlots.OnMainMenuInitialized();

            //if (BetterEnemyScaling.Enabled) {
            //    EnemyManager.maxNumEnemiesPooled = BetterEnemyScaling.MaxNumberOfEnemiesPooled;
            //}

            var sus = Sus.Check();
            if (sus) {
                LoggerInstance.Warning("=== ATTENTION ===");
                LoggerInstance.Warning("[Sus.Check, Sus.CheckMods] Megabonks internal anti-cheat found out you are sus!");
                LoggerInstance.Warning("[Sus.Check, Sus.CheckMods] Be aware that there is a risk that you will end up being banned from the leaderboards!");
                LoggerInstance.Warning("=================");
            }
        }

        private void OnLevelInitialized() {
            //if (BetterMinimap.Enabled)
            //    BetterMinimap.OnLevelInitialized();

            //if (MoreTomeAndWeaponSlots.Enabled)
            //    MoreTomeAndWeaponSlots.OnLevelInitialized();

            //MelonCoroutines.Start(DelayedOnLevelInitialized());
        }

        private void OnLoadingScreenInitialized() {

        }

        private IEnumerator DelayedOnLevelInitialized() {
            yield return new WaitForSeconds(5f);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) {

        }

        public override void OnUpdate() {
            if (_actionQueue.Count > 0) {
                try {
                    if (_actionQueue.TryDequeue(out var action))
                        action();
                } catch {

                }
            }

            //var inputPlayerHelper = ReInput.players;
            //if (inputPlayerHelper == null)
            //    return;
            ////var systemInputPlayer = inputPlayerHelper.AllPlayers[0];
            //var megabonkInputPlayer = inputPlayerHelper.AllPlayers[1];
            //if (megabonkInputPlayer == null)
            //    return;
            //var controllerHelper = megabonkInputPlayer.controllers;
            //if (controllerHelper == null)
            //    return;

            //var mouse = controllerHelper.Mouse;
            //var keyboard = controllerHelper.Keyboard;

            //if (mouse.GetButtonDownById((int)MouseInputElement.Button0)) {

            //}

            //if (keyboard.GetKeyDown(KeyCode.A)) {

            //}
        }

        public override void OnDeinitializeMelon() {

        }

        internal void EnqueueAction(Action action) {
            _actionQueue.Enqueue(action);
        }

        private enum SceneState {
            None,
            Boot,
            MainMenu,
            Level,
            LoadingScreen
        }
    }
}
