namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Linear
    {
        /// <summary>
        /// Easing equation function for a simple linear tweening, with no easing.
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
        public static float EaseNone(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return changeValue * time / duration + startValue;
        }
    }
}