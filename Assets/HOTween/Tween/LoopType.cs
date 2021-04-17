namespace Holoville.HOTween
{
    /// <summary>Enumeration of types of loops to apply.</summary>
    public enum LoopType
    {
        /// <summary>
        /// When a tween completes, rewinds the animation and restarts (X to Y, repeat).
        /// </summary>
        Restart,

        /// <summary>
        /// Tweens to the end values then back to the original ones and so on (X to Y, Y to X, repeat).
        /// </summary>
        Yoyo,

        /// <summary>
        /// Like <see cref="F:Holoville.HOTween.LoopType.Yoyo" />, but also inverts the easing (meaning if it was <c>easeInSomething</c>, it will become <c>easeOutSomething</c>, and viceversa).
        /// </summary>
        YoyoInverse,

        /// <summary>
        /// Continuously increments the tween (X to Y, Y to Y+(Y-X), and so on),
        /// thus always moving "onward".
        /// </summary>
        Incremental,
    }
}