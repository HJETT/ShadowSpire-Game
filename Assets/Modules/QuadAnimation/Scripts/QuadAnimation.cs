using UnityEngine;

namespace QuadAnimation
{
    [RequireComponent(typeof(MeshFilter))]
    public class QuadAnimation : MonoBehaviour
    {
        #region Parameters

        [Header("Wave Parameters")]
        [SerializeField] private float amplitude = 0.1f;
        [SerializeField] private float period = 2f;
        [SerializeField] private float waveSpeed = 1f;
        [SerializeField] private Vector2 waveDirection = new Vector2(1, 0);

        #endregion

        private MeshFilter meshFilter;
        private Mesh currentMesh;
        private Vector3[] verticesReference;

        private void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            currentMesh = meshFilter.mesh;
            verticesReference = currentMesh.vertices;

            // Normaliser la direction de l'onde
            if (waveDirection.magnitude > 0)
                waveDirection.Normalize();
        }

        private void Update()
        {
            // Si le maillage a chang�, mettre � jour les r�f�rences
            if (meshFilter.mesh != currentMesh)
            {
                currentMesh = meshFilter.mesh;
                verticesReference = currentMesh.vertices;
            }

            Vector3[] copy = new Vector3[verticesReference.Length];
            float time = Time.time * waveSpeed;

            for (int i = 0; i < copy.Length; i++)
            {
                Vector3 pos = verticesReference[i];
                float wavePhase = waveDirection.x * pos.x + waveDirection.y * pos.z;
                pos.y += amplitude * Mathf.Sin((2 * Mathf.PI / period) * wavePhase - time) * (pos.x - verticesReference[0].x) / 2;
                copy[i] = pos;
            }

            // Mettre � jour le maillage
            meshFilter.mesh.SetVertices(copy);
            meshFilter.mesh.RecalculateNormals();
        }
    }
}