using UnityEngine;

namespace QuadAnimation
{
    [RequireComponent(typeof(MeshFilter))]
    public class ParameteredRectangle : MonoBehaviour
    {
        #region Parameters

        [Header("Parameters")]
        [SerializeField, Min(0.001f)]
        private float height;

        [SerializeField, Min(0.001f)]
        private float width;

        public Vector2Int resolution;

        #endregion Parameters

        private MeshFilter meshFilter;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = Generate(new Vector2(width, height), resolution);
        }

        private static Mesh Generate(Vector2 size, Vector2Int resolution)
        {
            // Check values
            size.x = Mathf.Max(0, size.x);
            size.y = Mathf.Max(0, size.y);
            resolution.x = Mathf.Max(0, resolution.x);
            resolution.y = Mathf.Max(0, resolution.y);

            Vector2 singleSize = size;
            singleSize.x /= resolution.x + 1;
            singleSize.y /= resolution.y + 1;

            Mesh mesh = new();
            mesh.Clear();

            // 1) Define the co-ordinates of each corner
            int width = 2 + resolution.x;
            int height = 2 + resolution.y;

            Vector3[] c = new Vector3[width * height];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = new(
                    Mathf.FloorToInt(i / height) * singleSize.x,
                    0,
                    i % height * singleSize.y
                );
            }

            // 2) Define the vertices (duplicated for both sides)
            Vector3[] vertices = new Vector3[c.Length * 2];
            c.CopyTo(vertices, 0); // Front side
            c.CopyTo(vertices, c.Length); // Back side

            // 3) Define each vertex's Normal (front = up, back = down)
            Vector3[] normals = new Vector3[vertices.Length];
            for (int i = 0; i < c.Length; i++)
            {
                normals[i] = Vector3.up; // Front face
                normals[i + c.Length] = Vector3.down; // Back face
            }

            // 4) Define each vertex's UV coordinates (duplicated for both sides)
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int i = 0; i < c.Length; i++)
            {
                uvs[i] = new(
                    vertices[i].x / size.x,
                    vertices[i].z / size.y
                );
                uvs[i + c.Length] = uvs[i];
            }

            // 5) Define the triangles (two sets for front and back faces)
            int triangleCount = (width - 1) * (height - 1) * 2;
            int[] triangles = new int[triangleCount * 3 * 2]; // Two sides

            // Front face triangles
            for (int i = 0; i < triangleCount * 3; i += 6)
            {
                int current = i / 6;
                current += current / (height - 1);

                triangles[i] = current;
                triangles[i + 1] = current + 1;
                triangles[i + 2] = current + height;

                triangles[i + 3] = current + 1;
                triangles[i + 4] = current + height + 1;
                triangles[i + 5] = current + height;
            }

            // Back face triangles (in reverse order)
            for (int i = 0; i < triangleCount * 3; i += 6)
            {
                int current = i / 6;
                current += current / (height - 1);

                int offset = c.Length;

                triangles[triangleCount * 3 + i] = current + offset;
                triangles[triangleCount * 3 + i + 1] = current + height + offset;
                triangles[triangleCount * 3 + i + 2] = current + 1 + offset;

                triangles[triangleCount * 3 + i + 3] = current + height + offset;
                triangles[triangleCount * 3 + i + 4] = current + height + 1 + offset;
                triangles[triangleCount * 3 + i + 5] = current + 1 + offset;
            }

            // 6) Build the Mesh
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.Optimize();
            mesh.name = $"Rectangle (Generated)";

            return mesh;
        }
    }
}

