using System;

namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Circ
    {
        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseIn(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return (float)(-(double)changeValue * (Math.Sqrt(1.0 - (time /= duration) * (double)time) - 1.0)) +
                   startValue;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return changeValue *
                (float)Math.Sqrt(1.0 - (time = (float)(time / (double)duration - 1.0)) * (double)time) + startValue;
        }

        /// <summary>
        /// Easing equation function for a circular (sqrt(1-t^2)) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseInOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return (time /= duration * 0.5f) < 1.0
                ? (float)(-(double)changeValue * 0.5 * (Math.Sqrt(1.0 - time * (double)time) - 1.0)) + startValue
                : (float)(changeValue * 0.5 * (Math.Sqrt(1.0 - (time -= 2f) * (double)time) + 1.0)) + startValue;
        }
    }
}