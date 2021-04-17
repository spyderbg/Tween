namespace Holoville.HOTween
{
    /// <summary>
    /// Only used with <see cref="T:Holoville.HOTween.Core.ABSTweenComponent" /> ApplyCallback method.
    /// </summary>
    public enum CallbackType
    {
        /// <summary>Called when the tween is starting</summary>
        OnStart,

        /// <summary>Called each time the tween is updated</summary>
        OnUpdate,

        /// <summary>Called each time a single loop is completed</summary>
        OnStepComplete,

        /// <summary>
        /// Called when the whole tween (loops included) is complete
        /// </summary>
        OnComplete,

        /// <summary>Called when the tween is paused</summary>
        OnPause,

        /// <summary>Called when the tween is played</summary>
        OnPlay,

        /// <summary>Called when the tween is rewinded</summary>
        OnRewinded,

        /// <summary>
        /// Works only with Tweeners, and not with Sequences.
        /// Called when a plugin of the Tweens is overwritten by the OverwriteManager.
        /// </summary>
        OnPluginOverwritten,
    }
}