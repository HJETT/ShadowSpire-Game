using UnityEngine;
using UnityEngine.AI;

namespace NightClub
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class MiniBot : MonoBehaviour
    {
        private NavMeshAgent navMeshAgent;

        public Transform target;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (target == null)
                return;

            navMeshAgent.SetDestination(target.position);
        }
    }
}


