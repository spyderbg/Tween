using System;
using UnityEngine;

namespace Holoville.HOTween.Core
{
    /// <summary>Various utils used by HOTween.</summary>
    internal static class Utils
    {
        /// <summary>
        /// Converts the given Matrix4x4 to a Quaternion and returns it.
        /// </summary>
        /// <param name="m">The matrix to convert.</param>
        /// <returns>
        /// The resulting <see cref="T:UnityEngine.Quaternion" />.
        /// </returns>
        internal static Quaternion MatrixToQuaternion(Matrix4x4 m)
        {
            var quaternion = new Quaternion();
            var num1 = 1f + m[0, 0] + m[1, 1] + m[2, 2];
            if (num1 < 0.0)
                num1 = 0.0f;
            quaternion.w = (float)Math.Sqrt(num1) * 0.5f;
            var num2 = 1f + m[0, 0] - m[1, 1] - m[2, 2];
            if (num2 < 0.0)
                num2 = 0.0f;
            quaternion.x = (float)Math.Sqrt(num2) * 0.5f;
            var num3 = 1f - m[0, 0] + m[1, 1] - m[2, 2];
            if (num3 < 0.0)
                num3 = 0.0f;
            quaternion.y = (float)Math.Sqrt(num3) * 0.5f;
            var num4 = 1f - m[0, 0] - m[1, 1] + m[2, 2];
            if (num4 < 0.0)
                num4 = 0.0f;
            quaternion.z = (float)Math.Sqrt(num4) * 0.5f;
            quaternion.x *= Mathf.Sign(quaternion.x * (m[2, 1] - m[1, 2]));
            quaternion.y *= Mathf.Sign(quaternion.y * (m[0, 2] - m[2, 0]));
            quaternion.z *= Mathf.Sign(quaternion.z * (m[1, 0] - m[0, 1]));
            return quaternion;
        }

        /// <summary>
        /// Returns a string representing the given Type without the packages
        /// (like Single instead than System.Single).
        /// </summary>
        internal static string SimpleClassName(Type p_class)
        {
            var str = p_class.ToString();
            return str.Substring(str.LastIndexOf('.') + 1);
        }
    }
}