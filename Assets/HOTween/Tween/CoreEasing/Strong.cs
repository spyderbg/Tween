namespace Holoville.HOTween.Core.Easing
{
    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    public static class Strong
    {
        /// <summary>Tween.</summary>
        /// <param name="time">Time.</param>
        /// <param name="startValue">Begin value.</param>
        /// <param name="changeValue">Change value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>
        /// A <see cref="T:System.Single" />
        /// </returns>
        public static float EaseIn(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return changeValue * (time /= duration) * time * time * time * time + startValue;
        }

        /// <summary>Tween.</summary>
        /// <param name="time">Time.</param>
        /// <param name="startValue">Begin value.</param>
        /// <param name="changeValue">Change value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>
        /// A <see cref="T:System.Single" />
        /// </returns>
        public static float EaseOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return changeValue * (float)((time = (float)(time / (double)duration - 1.0)) * (double)time * time * time * time + 1.0) + startValue;
        }

        /// <summary>Tween.</summary>
        /// <param name="time">Time.</param>
        /// <param name="startValue">Begin value.</param>
        /// <param name="changeValue">Change value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="unusedOvershootOrAmplitude">Unused: here to keep same delegate for all ease types.</param>
        /// <param name="unusedPeriod">Unused: here to keep same delegate for all ease types.</param>
        /// <returns>
        /// A <see cref="T:System.Single" />
        /// </returns>
        public static float EaseInOut(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershootOrAmplitude,
            float unusedPeriod)
        {
            return (time /= duration * 0.5f) < 1.0
                ? changeValue * 0.5f * time * time * time * time * time + startValue
                : (float)(changeValue * 0.5 * ((time -= 2f) * (double)time * time * time * time + 2.0)) + startValue;
        }
    }
}