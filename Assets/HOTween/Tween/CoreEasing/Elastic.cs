using System;

namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Elastic
    {
        private const float TwoPi = 6.283185f;

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>The eased value.</returns>
        public static float EaseIn(float time, float startValue, float changeValue, float duration) =>
            EaseIn(time, startValue, changeValue, duration, 0.0f, 0.0f);

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="period">Period.</param>
        /// <returns>The eased value.</returns>
        public static float EaseIn(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float amplitude,
            float period)
        {
            if (time == 0.0)
                return startValue;
            if ((time /= duration) == 1.0)
                return startValue + changeValue;
            if (period == 0.0)
                period = duration * 0.3f;
            float num;
            if (amplitude == 0.0 || changeValue > 0.0 && amplitude < (double)changeValue ||
                changeValue < 0.0 && amplitude < -(double)changeValue)
            {
                amplitude = changeValue;
                num = period / 4f;
            }
            else
                num = period / 6.283185f * (float)Math.Asin(changeValue / (double)amplitude);

            return (float)-(amplitude * Math.Pow(2.0, 10.0 * --time) *
                            Math.Sin((time * (double)duration - num) * 6.28318548202515 / period)) + startValue;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>The eased value.</returns>
        public static float EaseOut(float time, float startValue, float changeValue, float duration) =>
            EaseOut(time, startValue, changeValue, duration, 0.0f, 0.0f);

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="period">Period.</param>
        /// <returns>The eased value.</returns>
        public static float EaseOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float amplitude,
            float period)
        {
            if (time == 0.0)
                return startValue;
            if ((time /= duration) == 1.0)
                return startValue + changeValue;
            if (period == 0.0)
                period = duration * 0.3f;
            float num;
            if (amplitude == 0.0 || changeValue > 0.0 && amplitude < (double)changeValue ||
                changeValue < 0.0 && amplitude < -(double)changeValue)
            {
                amplitude = changeValue;
                num = period / 4f;
            }
            else
                num = period / 6.283185f * (float)Math.Asin(changeValue / (double)amplitude);

            return (float)(amplitude * Math.Pow(2.0, -10.0 * time) *
                           Math.Sin((time * (double)duration - num) * 6.28318548202515 / period)) + changeValue +
                   startValue;
        }

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>The eased value.</returns>
        public static float EaseInOut(float time, float startValue, float changeValue, float duration) =>
            EaseInOut(time, startValue, changeValue, duration, 0.0f, 0.0f);

        /// <summary>
        /// Easing equation function for an elastic (exponentially decaying sine wave) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="amplitude">Amplitude.</param>
        /// <param name="period">Period.</param>
        /// <returns>The eased value.</returns>
        public static float EaseInOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float amplitude,
            float period)
        {
            if (time == 0.0)
                return startValue;
            if ((time /= duration * 0.5f) == 2.0)
                return startValue + changeValue;
            if (period == 0.0)
                period = duration * 0.45f;
            float num;
            if (amplitude == 0.0 || changeValue > 0.0 && amplitude < (double)changeValue || changeValue < 0.0 && amplitude < -(double)changeValue)
            {
                amplitude = changeValue;
                num = period / 4f;
            }
            else
                num = period / 6.283185f * (float)Math.Asin(changeValue / (double)amplitude);

            return time < 1.0
                ? (float)(-0.5 * (amplitude * Math.Pow(2.0, 10.0 * --time) *
                                  Math.Sin((time * (double)duration - num) * 6.28318548202515 / period))) + startValue
                : (float)(amplitude * Math.Pow(2.0, -10.0 * --time) *
                          Math.Sin((time * (double)duration - num) * 6.28318548202515 / period) * 0.5) + changeValue +
                  startValue;
        }
    }
}