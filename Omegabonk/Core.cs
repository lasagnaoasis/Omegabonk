using Il2Cpp;

using Il2CppAssets.Scripts.Actors.Player;
using Il2CppAssets.Scripts.Managers;

using MelonLoader;

using Omegabonk.Tweaks;

using System.Collections;
using System.Collections.Concurrent;

using UnityEngine;

[assembly: MelonInfo(typeof(Omegabonk.Core), "Omegabonk", "0.1.3", "skatinglasagna", null)]
[assembly: MelonGame("Ved", "Megabonk")]

namespace Omegabonk {
    public class Core : MelonMod {
        internal static Core Instance;

        private ConcurrentQueue<Action> _actionQueue = new ConcurrentQueue<Action>();

        public override void OnEarlyInitializeMelon() {
            Instance = this;

            Preferences.Setup();
        }

        //public override void OnInitializeMelon() {

        //}

        //public override void OnLateInitializeMelon() {

        //}

        //public override void OnSceneWasLoaded(int buildIndex, string sceneName) {

        //}

        public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
            switch (buildIndex) {
                case -1: //None
                    break;
                case 0: //Boot
                    OnBootInitialized();
                    break;
                case 1: //Main menu
                    OnMainMenuInitialized();
                    break;
                case 2: //Level
                    OnLevelInitialized();
                    break;
            }
        }

        private void OnBootInitialized() {

        }

        private void OnMainMenuInitialized() {
            //DisableSteamAchievements.Enable();

            if (MoreTomeAndWeaponSlots.Enabled)
                MoreTomeAndWeaponSlots.OnMainMenuInitialized();

            //if (BetterEnemyScaling.Enabled) {
            //    EnemyManager.maxNumEnemiesPooled = BetterEnemyScaling.MaxNumberOfEnemiesPooled;
            //}
        }

        private void OnLevelInitialized() {
            if (BetterMinimap.Enabled)
                BetterMinimap.OnLevelInitialized();

            if (MoreTomeAndWeaponSlots.Enabled)
                MoreTomeAndWeaponSlots.OnLevelInitialized();

            //MelonCoroutines.Start(DelayedOnLevelInitialized());
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
        }

        public override void OnDeinitializeMelon() {

        }

        internal void EnqueueAction(Action action) {
            _actionQueue.Enqueue(action);
        }
    }
}
