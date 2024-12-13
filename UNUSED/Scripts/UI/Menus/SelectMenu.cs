using UnityEngine;

namespace Battle.UI
{
    public abstract class SelectMenu : MonoBehaviour
    {
        public bool IsSelected;

        public abstract void Clear();
        public abstract void Move(Vector2 dir);
    }
}