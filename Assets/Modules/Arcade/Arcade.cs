using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UtilsModule;

namespace Arcade
{
    /// <summary>
    /// Script for the interaction between the player and the arcade
    /// </summary>
    public class Arcade : Singleton<Arcade>, IInteractable
    {
        [SerializeField] CinemachineVirtualCamera ArcadeCamera;
        [SerializeField] PlayerController player;
        [SerializeField] AudioLowPassFilter AudioFilter;
        bool isGameLoaded;

        public static bool QuitUnload;

        /// <inheritdoc/>
        public void OnClick() => Start2DGame();

        public void Start2DGame()
        {
            player.enabled = false;

            var camPlayer = player.GetComponentInChildren<CinemachineVirtualCamera>();
            ArcadeCamera.Priority = camPlayer.Priority + 1;

            AudioFilter.enabled = true;

            if (!isGameLoaded)
                StartCoroutine(StartGame());
        }

        private IEnumerator StartGame()
        {
            isGameLoaded = true;

            var loadScene = SceneManager.LoadSceneAsync("TitleScreen", LoadSceneMode.Additive);

            while (!loadScene.isDone)
                yield return null;

            SceneManager.sceneUnloaded += On2DGameUnloaded;
        }

        private void On2DGameUnloaded(Scene scene)
        {
            if (scene.name != "TitleScreen" || !QuitUnload)
                return;

            SceneManager.sceneUnloaded -= On2DGameUnloaded;

            player.enabled = true;

            var camPlayer = player.GetComponentInChildren<CinemachineVirtualCamera>();
            ArcadeCamera.Priority = camPlayer.Priority - 1;

            AudioFilter.enabled = false;

            isGameLoaded = false;
            QuitUnload = false;
        }

        #region Singleton

        /// <inheritdoc/>
        protected override bool DestroyOnLoad => true;

        #endregion
    }
}
