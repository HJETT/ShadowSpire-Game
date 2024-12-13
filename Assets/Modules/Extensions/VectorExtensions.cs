using UnityEngine;

namespace ExtensionsModule
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Clamps the vector component-wise.
        /// </summary>
        /// <param name="original">Vector to clamp</param>
        /// <param name="maxAngles">Max angles on each axis</param>
        /// <param name="clampAxis">Axis to clamp</param>
        /// <returns>Clamped vector</returns>
        public static Vector3 ClampAll(this Vector3 original, Vector3 maxAngles, Vector3 clampAxis)
        {
            Vector3 copy = original;

            if (clampAxis.x != 0)
                copy.x = Mathf.Clamp(copy.x, -maxAngles.x, maxAngles.x);
            
            if (clampAxis.y != 0)
                copy.y = Mathf.Clamp(copy.y, -maxAngles.y, maxAngles.y);

            if (clampAxis.z != 0)
                copy.z = Mathf.Clamp(copy.z, -maxAngles.z, maxAngles.z);

            return copy;
        }

        /// <summary>
        /// Clamps the given rotation.
        /// </summary>
        /// <param name="rotation">Current rotation of the target in degrees</param>
        /// <param name="angles">Angles in degrees to try adding to the target</param>
        /// <param name="maxAngles">Maximum angles in degrees on all axis</param>
        /// <param name="clampAxis">Axis to clamp the rotation</param>
        /// <returns>Clamped rotation</returns>
        public static Vector3 ClampRotation(this Vector3 rotation, Vector3 angles, Vector3 maxAngles, Vector3 clampAxis)
        {
            rotation += angles;
            rotation = rotation.ClampAll(maxAngles, clampAxis);

            return rotation;
        }
        
        /// <summary>
        /// Lerps the vector component-wise
        /// </summary>
        public static Vector3 LerpAll(this Vector3 original, Vector3 destination, float duration)
        {
            Vector3 copy = original;

            copy.x = Mathf.Lerp(copy.x, destination.x, duration);
            copy.y = Mathf.Lerp(copy.y, destination.y, duration);
            copy.z = Mathf.Lerp(copy.z, destination.z, duration);

            return copy;
        }

        /// <summary>
        /// Lerps the vector component-wise using LerpAngle
        /// </summary>
        public static Vector3 LerpAngleAll(this Vector3 original, Vector3 destination, float duration)
        {
            Vector3 copy = original;

            copy.x = Mathf.LerpAngle(copy.x, destination.x, duration);
            copy.y = Mathf.LerpAngle(copy.y, destination.y, duration);
            copy.z = Mathf.LerpAngle(copy.z, destination.z, duration);

            return copy;
        }
    }
}