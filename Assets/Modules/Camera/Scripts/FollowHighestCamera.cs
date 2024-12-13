using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CameraUtils
{
    [RequireComponent(typeof(Canvas))]
    public class FollowHighestCamera : MonoBehaviour
    {
        private Canvas canvas;

        private Stack<Camera> loadedCameras = new(); 

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var roots = scene.GetRootGameObjects();
            canvas.worldCamera = roots.First(r => r.activeInHierarchy && r.CompareTag("MainCamera")).GetComponent<Camera>();
            loadedCameras.Push(canvas.worldCamera);
        }

        void OnSceneUnloaded(Scene scene)
        {
            loadedCameras.Pop();

            if (loadedCameras.Count == 0)
                canvas.worldCamera = Camera.main;
            else
                canvas.worldCamera = loadedCameras.Peek();
        }
    }
}
