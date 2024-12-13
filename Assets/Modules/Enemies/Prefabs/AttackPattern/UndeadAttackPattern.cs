using UnityEngine;

namespace Enemies.Attacks
{
    public class UndeadAttackPattern : AttackPattern
    {
        float initialYPosition;
        float duration = 15;
        float elapsed = 0;

        void Start()
        {
            initialYPosition = transform.localPosition.y;
        }

        // Update is called once per frame
        void Update()
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x, 
                Mathf.Lerp(initialYPosition, 0, elapsed / duration), 
                transform.localPosition.z
            );
            elapsed += Time.deltaTime;
        }

        public override void ClearAttack()
        {
            throw new System.NotImplementedException();
        }

        public override void StartAttack()
        {
            elapsed = 0;
            enabled = true;
        }

        public override void StopAttack()
        {
            throw new System.NotImplementedException();
        }

    }

}
