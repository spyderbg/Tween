using UnityEngine;

namespace Holoville.HOTween {

/// <summary>
/// Tween component, created by HOTween for each separate tween.
/// <para>Author: Daniele Giardini (http://www.holoville.com)</para>
/// </summary>
public interface ITweener
{
    /// <summary>Indicates whether this is a FROM or a TO tween.</summary>
    bool IsFrom { get; }

    /// <summary>
    /// Ease type of this tweener
    /// (consider that the plugins you have set might have different ease types).
    /// Setting it will change the ease of all the plugins used by this tweener.
    /// </summary>
    EaseType EaseType { get; set; }

    /// <summary>
    /// Ease type of this tweener
    /// (consider that the plugins you have set might have different ease types).
    /// Setting it will change the ease of all the plugins used by this tweener.
    /// </summary>
    AnimationCurve EaseAnimationCurve { get; set; }

    /// <summary>Eventual overshoot to use with Back easeTypes.</summary>
    float EaseOvershootOrAmplitude { get; set; }

    /// <summary>Eventual period to use with Elastic easeTypes.</summary>
    float EasePeriod { get; set; }

    /// <summary>Target of this tween.</summary>
    object Target { get; }

    /// <summary>
    /// <c>true</c> if this tween is animated via integers values only.
    /// </summary>
    bool PixelPerfect { get; }

    /// <summary>
    /// <c>true</c> if this tween is animated by speed instead than by duration.
    /// </summary>
    bool SpeedBased { get; }

    /// <summary>The delay that was set for this tween.</summary>
    float Delay { get; }

    /// <summary>The currently elapsed delay time.</summary>
    public float ElapsedDelay { get; }

    /// <summary>Resumes this Tweener.</summary>
    /// <param name="skipDelay">
    /// If <c>true</c> skips any initial delay.
    /// </param>
    void Play(bool skipDelay);

    /// <summary>Resumes this Tweener and plays it forward.</summary>
    /// <param name="skipDelay">
    /// If <c>true</c> skips any initial delay.
    /// </param>
    void PlayForward(bool skipDelay);
}

}