using ExtensionsModule;
using UnityEngine;

namespace ControllerModule.Controllers
{
    public class LookController : MonoBehaviour
    {
        #region Parameters

        [Header("Parameters")]
        [SerializeField, Min(0), Tooltip("Determines how fast the camera rotates")]
        private float sensitivity = 5.0f;

        #endregion

        #region Angles Clamp

        [Header("Angles Clamp")]
        [SerializeField, Tooltip("Max angles that this controller can rotate")]
        private Vector3 maxAngles;

        [SerializeField, Tooltip("Axis on which the angles are clamped (0 or 1)")]
        private Vector3Int clampAxis;

        #endregion

        #region Object Rotations

        [Header("Object Rotations")]
        [SerializeField, Tooltip("Determines with which offset the anchor rotates")]
        private Transform parentController;

        [SerializeField, Tooltip("Root of the object to turn horizontally")]
        private Transform self;

        [Tooltip("Object that will turn the camera")]
        public Transform cameraAnchor;

        #endregion

        #region Update

        /// <summary>
        /// Current rotation
        /// </summary>
        private Vector3 camRotation = Vector3.zero;

        /// <summary>
        /// Updates the rotation of the target
        /// </summary>
        /// <param name="direction">Direction of the rotation</param>
        /// <param name="elapsed">Time since the last frame</param>
        public void UpdateRotation(Vector2 direction, float elapsed)
        {
            // Update rotation
            this.camRotation = this.camRotation.ClampRotation(
                elapsed * this.sensitivity * new Vector3(-direction.y, direction.x, 0),
                this.maxAngles,
                this.clampAxis
            );

            Vector3 copy = this.camRotation;

            // Add offset
            if (this.parentController != null)
                copy += this.parentController.eulerAngles;
            
            // Rotate correct Transform
            if (this.self != null)
            {
                this.self.rotation = Quaternion.Euler(0, copy.y, 0);
                
                copy.y = 0;
            }

            if (this.cameraAnchor != null)
            {
                this.cameraAnchor.localRotation = Quaternion.Euler(copy);
            }
        }

        #endregion

        #region MonoBehaviour

        /// <inheritdoc cref="Start" />
        private void Start() => this.camRotation = this.cameraAnchor.localEulerAngles;

#if UNITY_EDITOR
        /// <inheritdoc cref="OnValidate" />
        private void OnValidate()
        {
            // Limit angles between [0; 180]
            this.maxAngles.Set(
                Mathf.Clamp(this.maxAngles.x, 0, 180),
                Mathf.Clamp(this.maxAngles.y, 0, 180),
                Mathf.Clamp(this.maxAngles.z, 0, 180)
            );

            // Make sure the axis are only 0 or 1
            this.clampAxis.Set(
                this.clampAxis.x <= 0 ? 0 : 1,
                this.clampAxis.y <= 0 ? 0 : 1,
                this.clampAxis.z <= 0 ? 0 : 1
            );
        }

        /// <inheritdoc cref="OnDrawGizmosSelected" />
        private void OnDrawGizmosSelected()
        {
            // Only in inspector
            if (Application.isPlaying)
                return;

            Vector3 center = this.cameraAnchor?.position ?? Vector3.zero;
            Vector3 angles = this.maxAngles * Mathf.Deg2Rad;

            // X
            if (this.clampAxis.x == 1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(0, Mathf.Sin(angles.x), Mathf.Cos(angles.x))
                );
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(0, Mathf.Sin(-angles.x), Mathf.Cos(-angles.x))
                );
            }

            // Y
            if (this.clampAxis.y == 1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(Mathf.Sin(angles.y), 0, Mathf.Cos(angles.y))
                );
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(Mathf.Sin(-angles.y), 0, Mathf.Cos(-angles.y))
                );
            }

            // Z
            if (this.clampAxis.z == 1)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(Mathf.Cos(angles.z), Mathf.Sin(angles.z), 0)
                );
                Gizmos.DrawLine(
                    center,
                    center + new Vector3(Mathf.Cos(-angles.z), Mathf.Sin(-angles.z), 0)
                );
            }
        }

#endif

        #endregion
    }
}