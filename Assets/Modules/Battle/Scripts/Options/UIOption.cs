using System;
using UnityEngine;

namespace Battle.Options
{
    public class UIOptionData
    {
        public Action OnEnter;
        public Action OnEscape;
    }

    public abstract class UIOption<T> : MonoBehaviour where T : UIOptionData
    {
        #region Parent

        protected UIOptions parent;

        public void SetParent(UIOptions parent) => this.parent = parent;

        #endregion

        #region Load

        protected T loadedOption;
        public T GetOption() => loadedOption;

        public void LoadOption(T option)
        {
            loadedOption = option;
            OnLoadOption(option);
        }

        protected virtual void OnLoadOption(T option) { }

        #endregion

        #region Selection

        public abstract void Select();
        public abstract void Deselect();

        #endregion

        #region Input

        public void Enter() => loadedOption.OnEnter?.Invoke();
        public void Escape() => loadedOption.OnEscape?.Invoke();

        #endregion
    }
}