using UnityEngine;

namespace UtilsModule
{
    /// <summary>
    /// Class that allows any class to be accessed from anywhere
    /// </summary>
    /// <typeparam name="T">Type of the class</typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Unique instance of <typeparamref name="T"/>
        /// </summary>
        public static T Instance;

        #region MonoBehaviour

        /// <inheritdoc cref="Awake" />
        private void Awake()
        {
            // Keep only one
            if (Instance != null)
            {
                if (!CanCoexist)
                {
                    Debug.LogWarning($"Another instance of {this.GetType().Name} has been found.");
                    Destroy(this.gameObject);
                }
                return;
            }

            Instance = this.gameObject.GetComponent<T>();

            if (!this.DestroyOnLoad)
            {
                this.transform.SetParent(null);
                DontDestroyOnLoad(this.gameObject);
            }

            this.OnAwake();
        }

        #endregion MonoBehaviour

        #region Virtual

        /// <summary>
        /// Defines if the singleton should be destroyed when loading a new scene
        /// </summary>
        protected virtual bool DestroyOnLoad => false;

        /// <summary>
        /// Defines if multiple instances of the singleton can coexist
        /// </summary>
        protected virtual bool CanCoexist => false;

        /// <summary>
        /// Called when <see cref="Awake"/> is called
        /// </summary>
        protected virtual void OnAwake() { }

        #endregion Virtual
    }
}