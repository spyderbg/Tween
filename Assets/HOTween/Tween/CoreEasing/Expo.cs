using System;

namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Expo
    {
        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
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
            return time == 0.0
                ? startValue
                : (float)(changeValue * Math.Pow(2.0, 10.0 * (time / (double)duration - 1.0)) + startValue - changeValue * (1.0 / 1000.0));
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing out: decelerating from zero velocity.
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
            return time == (double)duration
                ? startValue + changeValue
                : changeValue * (float)(-Math.Pow(2.0, -10.0 * time / duration) + 1.0) + startValue;
        }

        /// <summary>
        /// Easing equation function for an exponential (2^t) easing in/out: acceleration until halfway, then deceleration.
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
            if (time == 0.0) return startValue;
            if (time == (double)duration) return startValue + changeValue;

            return (time /= duration * 0.5f) < 1.0
                ? changeValue * 0.5f * (float)Math.Pow(2.0, 10.0 * (time - 1.0)) + startValue
                : (float)(changeValue * 0.5 * (-Math.Pow(2.0, -10.0 * --time) + 2.0)) + startValue;
        }
    }
}