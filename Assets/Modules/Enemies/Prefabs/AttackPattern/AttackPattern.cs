using UnityEngine;

namespace Enemies.Attacks
{
    public abstract class AttackPattern : MonoBehaviour
    {
        public abstract void StartAttack();
        public abstract void StopAttack();
        public abstract void ClearAttack();
    }
}
