namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Bounce
    {
        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
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
            return changeValue - EaseOut(duration - time, 0.0f, changeValue, duration, -1f, -1f) + startValue;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
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
            if ((time /= duration) < 0.363636374473572)
                return changeValue * (121f / 16f * time * time) + startValue;
            if (time < 0.727272748947144)
                return changeValue * (float)(121.0 / 16.0 * (time -= 0.5454546f) * time + 0.75) + startValue;
            return time < 0.909090936183929
                ? changeValue * (float)(121.0 / 16.0 * (time -= 0.8181818f) * time + 15.0 / 16.0) + startValue
                : changeValue * (float)(121.0 / 16.0 * (time -= 0.9545454f) * time + 63.0 / 64.0) + startValue;
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
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
            return time < duration * 0.5
                ? EaseIn(time * 2f, 0.0f, changeValue, duration, -1f, -1f) * 0.5f + startValue
                : (float)(EaseOut(time * 2f - duration, 0.0f, changeValue, duration, -1f, -1f) * 0.5 +
                          changeValue * 0.5) + startValue;
        }
    }
}