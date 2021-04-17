namespace Holoville.HOTween
{
    /// <summary>
    /// Enumeration of types of update that can be applied to a tween.
    /// </summary>
    public enum UpdateType
    {
        /// <summary>Normal update.</summary>
        Update,

        /// <summary>Late update.</summary>
        LateUpdate,

        /// <summary>Fixed update (useful for rigidBodies).</summary>
        FixedUpdate,

        /// <summary>
        /// Timescale independent update.
        /// Contrary to the other types, this one is not subject to changes in Time.timeScale,
        /// thus it's the best way for tweens that happen inside GUI methods
        /// (so that even if the game is paused, the GUI will still have animated tweens).
        /// </summary>
        TimeScaleIndependentUpdate,
    }
}