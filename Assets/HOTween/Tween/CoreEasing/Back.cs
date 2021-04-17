namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Back
    {
        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="overshoot">
        /// Overshoot ammount: higher means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
        /// </param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseIn(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float overshoot,
            float unusedPeriod)
        {
            return (float)(changeValue * (double)(time /= duration) * time * ((overshoot + 1.0) * time - overshoot)) +
                   startValue;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="overshoot">
        /// Overshoot ammount: higher means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
        /// </param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float overshoot,
            float unusedPeriod)
        {
            return changeValue * (float)((time = (float)(time / (double)duration - 1.0)) * (double)time *
                ((overshoot + 1.0) * time + overshoot) + 1.0) + startValue;
        }

        /// <summary>
        /// Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">Current time (in frames or seconds).</param>
        /// <param name="startValue">Starting value.</param>
        /// <param name="changeValue">Change needed in value.</param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <param name="overshoot">
        /// Overshoot ammount: higher means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
        /// </param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>The eased value.</returns>
        public static float EaseInOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float overshoot,
            float unusedPeriod)
        {
            return (time /= duration * 0.5f) < 1.0
                ? (float)(changeValue * 0.5 *
                          (time * (double)time * (((overshoot *= 1.525f) + 1.0) * time - overshoot))) + startValue
                : (float)(changeValue / 2.0 *
                          ((time -= 2f) * (double)time * (((overshoot *= 1.525f) + 1.0) * time + overshoot) + 2.0)) +
                  startValue;
        }
    }
}