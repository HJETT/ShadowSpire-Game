using System.Collections;
using Managers;
using UnityEngine;

namespace Weapons
{
    public class WeaponUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject title;

        public IEnumerator ShowWeapons()
        {
            hasSelected = false;

            LoadWeapons(GameManager.Instance.player.Weapon);
            title.SetActive(true);

            yield return null; // Wait for weapons to load

            // Enable inputs
            AddInputs();

            // Wait until weapon selected
            while (!hasSelected)
                yield return null;

            title.SetActive(false);
            options.DestroyOptions();
        }

        private void SelectWeapon()
        {
            // If already selected, skip
            if (hasSelected)
                return;

            hasSelected = true;

            // Disable inputs
            RemoveInputs();

            var selectedOption = options.GetSelection().GetOption();
            GameManager.Instance.player.Weapon = selectedOption.WeaponInstance;
        }

        #region Options

        [SerializeField]
        private WeaponOptions options;

        private bool hasSelected = false;

        private void LoadWeapons(WeaponInstance currentWeapon)
        {
            var weapons = new WeaponOptionData[3] {
                new() {
                    WeaponInstance = currentWeapon,
                    Subtext = "Keep",
                    OnEnter = SelectWeapon
                },
                new() {
                    WeaponInstance = WeaponInstance.CreateRandom(GameManager.Instance.Level.Index),
                    Subtext = "Replace",
                    OnEnter = SelectWeapon
                },
                new() {
                    WeaponInstance = WeaponInstance.CreateRandom(GameManager.Instance.Level.Index),
                    Subtext = "Replace",
                    OnEnter = SelectWeapon
                }
            };

            options.LoadOptions(weapons);
            options.ShowSelection();
        }

        #endregion

        #region Inputs

        private void Move(Vector2 dir) => options.Move(dir);
        private void Enter() => options.Enter();

        /// <summary>
        /// Adds the inputs for this object
        /// </summary>
        private void AddInputs()
        {
            InputManager.Instance.OnMoveUI.AddListener(Move);
            InputManager.Instance.OnEnterUI.AddListener(Enter);
        }

        /// <summary>
        /// Removes the inputs for this object
        /// </summary>
        private void RemoveInputs()
        {
            InputManager.Instance.OnMoveUI.RemoveListener(Move);
            InputManager.Instance.OnEnterUI.RemoveListener(Enter);
        }

        #endregion
    }
}