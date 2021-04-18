using Holoville.HOTween.Core;
using System.Collections;

namespace Holoville.HOTween {

// ReSharper disable once InconsistentNaming
/// <summary>
/// Public interface shared by <see cref="T:Holoville.HOTween.Tweener" /> and <see cref="T:Holoville.HOTween.Sequence" />.
/// </summary>
public interface IHOTweenComponent
{
    /// <summary>
    /// Eventual ID of this tween
    /// (more than one tween can share the same ID, thus allowing for grouped operations).
    /// You can also use <c>intId</c> instead of <c>id</c> for faster operations.
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Default is <c>-1</c>.
    /// Eventual int ID of this tween
    /// (more than one tween can share the same intId, thus allowing for grouped operations).
    /// The main difference from <c>id</c> is that while <c>id</c> is more legible, <c>intId</c> allows for faster operations.
    /// </summary>
    int IntId { get; set; }

    /// <summary>
    /// Default is <c>true</c>.
    /// If <c>false</c> doesn't remove this Tweener/Sequence from HOTween's list when it is completed
    /// (useful if you want to be able to control it independently with GoTo, instead than letting it run),
    /// and you will need to call an <c>HOTween.Kill</c> to remove this Tweener/Sequence.
    /// </summary>
    bool AutoKillOnComplete { get; set; }

    /// <summary>
    /// Default is <c>true</c>.
    /// If set to <c>false</c>, this Tweener/Sequence will not be updated,
    /// and any use of animation methods (Play/Pause/Rewind/etc) will be ignored
    /// (both if called directly via this instance, than if using HOTween.Play/Pause/Rewind/etc.).
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Time scale that will be used by this Tweener/Sequence.
    /// </summary>
    float TimeScale { get; set; }

    /// <summary>
    /// Number of times the Tweener/Sequence will run (<c>-1</c> means the tween has infinite loops, <c>1</c> means the tween will run only once).
    /// </summary>
    int Loops { get; set; }

    /// <summary>
    /// Type of loop for this Tweener/Sequence, in case <see cref="P:Holoville.HOTween.IHOTweenComponent.loops" /> is greater than 1 (or infinite).
    /// </summary>
    LoopType LoopType { get; set; }

    /// <summary>
    /// Gets and sets the time position of the Tweener/Sequence (loops are included when not infinite, delay is not).
    /// </summary>
    float Position { get; set; }

    /// <summary>
    /// Duration of this Tweener/Sequence, loops and tween delay excluded.
    /// </summary>
    float Duration { get; }

    /// <summary>
    /// Full duration of this Tweener/Sequence, loops included (when not infinite) but tween delay excluded.
    /// </summary>
    float FullDuration { get; }

    /// <summary>
    /// Elapsed time within the current loop (tween delay excluded).
    /// Note that <c>elapsed</c> will be equal to <c>duration</c> only when all the loops are completed,
    /// otherwise each time a loop is completed, <c>completedLoops</c> is augmented by 1 and <c>elapsed</c> is reset to <c>0</c>.
    /// </summary>
    float Elapsed { get; }

    /// <summary>
    /// Full elapsed time including loops (but without considering tween delay).
    /// </summary>
    float FullElapsed { get; }

    /// <summary>The update type for this Tweener/Sequence.</summary>
    UpdateType UpdateType { get; }

    /// <summary>Number of loops that have been executed.</summary>
    int CompletedLoops { get; }

    /// <summary>
    /// Returns a value of <c>true</c> if this Tweener/Sequence contains no tweens
    /// (if this is a Tweener, it means that no valid property to tween was set;
    /// if this is a Sequence, it means no valid <see cref="T:Holoville.HOTween.Tweener" /> was added).
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Returns a value of <c>true</c> if this Tweener/Sequence is set to go backwards (because of a call to <c>Reverse</c>.
    /// </summary>
    bool IsReversed { get; }

    /// <summary>
    /// Returns a value of <c>true</c> when this Tweener/Sequence is in the "going backwards" part of a Yoyo loop.
    /// </summary>
    bool IsLoopingBack { get; }

    /// <summary>
    /// Returns a value of <c>true</c> if this Tweener/Sequence is paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Returns a value of <c>true</c> after this Tweener/Sequence was started the first time,
    /// or if a call to <c>GoTo</c> or <c>GoToAndPlay</c> was executed.
    /// </summary>
    bool HasStarted { get; }

    /// <summary>
    /// Returns a value of <c>true</c> when this Tweener/Sequence is complete.
    /// </summary>
    bool IsComplete { get; }

    /// <summary>
    /// Returns a value of <c>true</c> if this Tweener/Sequence was added to a Sequence.
    /// </summary>
    bool IsSequenced { get; }

    /// <summary>Kills this Tweener/Sequence.</summary>
    void Kill();

    /// <summary>Resumes this Tweener/Sequence (tween delay included).</summary>
    void Play();

    /// <summary>
    /// Resumes this Tweener/Sequence (tween delay included) and plays it forward.
    /// </summary>
    void PlayForward();

    /// <summary>Resumes this Tweener/Sequence and plays it backwards.</summary>
    void PlayBackwards();

    /// <summary>Pauses this Tweener/Sequence.</summary>
    void Pause();

    /// <summary>
    /// Rewinds this Tweener/Sequence (loops and tween delay included), and pauses it.
    /// </summary>
    void Rewind();

    /// <summary>
    /// Restarts this Tweener/Sequence from the beginning (loops and tween delay included).
    /// </summary>
    void Restart();

    /// <summary>
    /// Reverses this Tweener/Sequence,
    /// animating it backwards from its current position.
    /// </summary>
    /// <param name="forcePlay">
    /// If TRUE, the tween will also start playing in case it was paused,
    /// otherwise it will maintain its current play/pause state (default).
    /// </param>
    void Reverse(bool forcePlay = false);

    /// <summary>
    /// Completes this Tweener/Sequence.
    /// Where a loop was involved and not infinite, the Tweener/Sequence completes at the position where it would actually be after the set number of loops.
    /// If there were infinite loops, this method will have no effect.
    /// </summary>
    void Complete();

    /// <summary>
    /// Sends the Tweener/Sequence to the given time (taking also loops into account).
    /// If the time is bigger than the total Tweener/Sequence duration, it goes to the end.
    /// </summary>
    /// <param name="time">
    /// The time where the Tweener/Sequence should be sent.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the Tweener/Sequence reached its end and was completed.
    /// </returns>
    bool GoTo(float time);

    /// <summary>
    /// Sends the Tweener/Sequence to the given time (taking also loops into account) and plays it.
    /// If the time is bigger than the total Tweener/Sequence duration, it goes to the end.
    /// </summary>
    /// <param name="time">
    /// The time where the Tweener/Sequence should be sent.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if the Tweener/Sequence reached its end and was completed.
    /// </returns>
    bool GoToAndPlay(float time);

    /// <summary>
    /// A coroutine that waits until the Tweener/Sequence is complete (delays and loops included).
    /// You can use it inside a coroutin as a yield. Ex:
    /// yield return StartCoroutine( myTweenComponent.WaitForCompletion() );
    /// </summary>
    IEnumerator WaitForCompletion();

    /// <summary>
    /// A coroutine that waits until the Tweener/Sequence is rewinded (loops included).
    /// You can use it inside a coroutine as a yield. Ex:
    /// yield return StartCoroutine( myTweenComponent.WaitForRewind() );
    /// </summary>
    IEnumerator WaitForRewind();

    /// <summary>
    /// Assigns the given callback to this Tweener/Sequence,
    /// overwriting any existing callbacks of the same type.
    /// </summary>
    /// <param name="callbackType">The type of callback to apply</param>
    /// <param name="callback">The function to call, who must return <c>void</c> and accept no parameters</param>
    void ApplyCallback(CallbackType callbackType, TweenDelegate.TweenCallback callback);

    /// <summary>
    /// Assigns the given callback to this Tweener/Sequence,
    /// overwriting any existing callbacks of the same type.
    /// </summary>
    /// <param name="callbackType">The type of callback to apply</param>
    /// <param name="callback">The function to call, who must return <c>void</c> and accept no parameters.
    /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" /></param>
    /// <param name="callbackParms">Additional comma separated parameters to pass to the function</param>
    void ApplyCallback(CallbackType callbackType, TweenDelegate.TweenCallbackWParms callback, params object[] callbackParms);

    /// <summary>
    /// Returns <c>true</c> if the given target is currently involved in this Tweener/Sequence (taking into account also nested tweens).
    /// Returns <c>false</c> both if the given target is not inside this Tweener/Sequence, than if the relative Tweener/Sequence is paused.
    /// To simply check if the target is attached to this Tweener/Sequence, use <c>IsLinkedTo( target )</c> instead.
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns>
    /// A value of <c>true</c> if the given target is currently involved in a running tween or sequence.
    /// </returns>
    bool IsTweening(object target);

    /// <summary>
    /// Returns <c>true</c> if the given target is linked to this Tweener/Sequence (running or not, taking into account also nested tweens).
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <returns>
    /// A value of <c>true</c> if the given target is linked to this Tweener/Sequence (running or not, taking into account also nested tweens).
    /// </returns>
    bool IsLinkedTo(object target);
}

}