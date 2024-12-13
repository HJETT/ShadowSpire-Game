using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Battle
{
    /// <summary>
    /// Class that handles the battle transition
    /// </summary>
    public class BattleTransition
    {
        #region Material Changes

        private static void SetTransitionTexture(Material material, Texture texture) => material.SetTexture("_TransitionTex", texture);
        private static void SetTransitionCutoff(Material material, float cutoff) => material.SetFloat("_Cutoff", cutoff);

        /// <summary>
        /// Resets the transition for another use
        /// </summary>
        public static void ResetMaterial(Material material)
        {
            SetTransitionTexture(material, null);
            SetTransitionCutoff(material, 0);
        }

        #endregion

        #region Transition

        public static IEnumerator ExecuteTransition(Material material, Texture[] textures, float fadeDuration)
        {
            yield return LoadRandomTransition(material, textures);
            yield return TransitionFadeIn(material, fadeDuration);

            var loadScene = SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);

            // Wait until loaded
            while (!loadScene.isDone)
                yield return null;

            ResetMaterial(material);

            yield return new WaitForSeconds(1f); // Dramatic effect
        }

        /// <summary>
        /// Sets a random texture to the transition material
        /// </summary>
        private static IEnumerator LoadRandomTransition(Material material, Texture[] textures)
        {
            int rdmIndex = Random.Range(0, textures.Length);
            SetTransitionTexture(material, textures[rdmIndex]);

            yield return null; // Wait for load texture
        }

        /// <summary>
        /// Processes the fade in of the transition
        /// </summary>
        private static IEnumerator TransitionFadeIn(Material material, float duration)
        {
            float time = 0;

            while (time < duration)
            {
                SetTransitionCutoff(material, Mathf.Lerp(0, 1, time / duration));

                yield return null; // Wait 1 frame

                time += Time.deltaTime;
            }

            SetTransitionCutoff(material, 1f);
            yield return null; // Wait 1 frame
        }

        #endregion
    }
}