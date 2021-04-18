using Holoville.HOTween.Core;
using UnityEngine;

namespace Holoville.HOTween {

/// <summary>
/// Sequence component. Manager for sequence of Tweeners or other nested Sequences.
/// <para>Author: Daniele Giardini (http://www.holoville.com)</para>
/// </summary>
public interface ISequence
{
    /// <summary>Appends the given callback to this Sequence.</summary>
    /// <param name="callback">The function to call, who must return <c>void</c> and accept no parameters</param>
    void AppendCallback(TweenDelegate.TweenCallback callback);

    /// <summary>Appends the given callback to this Sequence.</summary>
    /// <param name="callback">The function to call.
    /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" /></param>
    /// <param name="callbackParms">Additional comma separated parameters to pass to the function</param>
    void AppendCallback(TweenDelegate.TweenCallbackWParms callback, params object[] callbackParms);

    /// <summary>Appends the given SendMessage callback to this Sequence.</summary>
    /// <param name="sendMessageTarget">GameObject to target for sendMessage</param>
    /// <param name="methodName">Name of the method to call</param>
    /// <param name="value">Eventual additional parameter</param>
    /// <param name="options">SendMessageOptions</param>
    void AppendCallback(GameObject sendMessageTarget, string methodName, object value, SendMessageOptions options = SendMessageOptions.RequireReceiver);

    /// <summary>Inserts the given callback at the given time position.</summary>
    /// <param name="time">Time position where this callback will be placed
    /// (if longer than the whole sequence duration, the callback will never be called)</param>
    /// <param name="callback">The function to call, who must return <c>void</c> and accept no parameters</param>
    void InsertCallback(float time, TweenDelegate.TweenCallback callback);

    /// <summary>Inserts the given callback at the given time position.</summary>
    /// <param name="time">Time position where this callback will be placed
    /// (if longer than the whole sequence duration, the callback will never be called)</param>
    /// <param name="callback">The function to call.
    /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" /></param>
    /// <param name="callbackParms">Additional comma separated parameters to pass to the function</param>
    void InsertCallback(float time, TweenDelegate.TweenCallbackWParms callback, params object[] callbackParms);

    /// <summary>Inserts the given SendMessage callback at the given time position.</summary>
    /// <param name="time">Time position where this callback will be placed
    /// (if longer than the whole sequence duration, the callback will never be called)</param>
    /// <param name="sendMessageTarget">GameObject to target for sendMessage</param>
    /// <param name="methodName">Name of the method to call</param>
    /// <param name="value">Eventual additional parameter</param>
    /// <param name="options">SendMessageOptions</param>
    void InsertCallback(float time, GameObject sendMessageTarget, string methodName, object value,
        SendMessageOptions options = SendMessageOptions.RequireReceiver);

    /// <summary>
    /// Appends an interval to the right of the sequence,
    /// and returns the new Sequence total time length (loops excluded).
    /// </summary>
    /// <param name="pDuration">The duration of the interval.</param>
    /// <returns>The new Sequence total time length (loops excluded).</returns>
    float AppendInterval(float pDuration);

    /// <summary>
    /// Adds the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to the right of the sequence,
    /// and returns the new Sequence total time length (loops excluded).
    /// </summary>
    /// <param name="twMember">
    /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to append.
    /// </param>
    /// <returns>The new Sequence total time length (loops excluded).</returns>
    float Append(IHOTweenComponent twMember);

    /// <summary>
    /// Prepends an interval to the left of the sequence,
    /// and returns the new Sequence total time length (loops excluded).
    /// </summary>
    /// <param name="pDuration">The duration of the interval.</param>
    /// <returns>The new Sequence total time length (loops excluded).</returns>
    float PrependInterval(float pDuration);

    /// <summary>
    /// Adds the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to the left of the sequence,
    /// moving all the existing sequence elements to the right,
    /// and returns the new Sequence total time length (loops excluded).
    /// </summary>
    /// <param name="twMember">
    /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to prepend.
    /// </param>
    /// <returns>The new Sequence total time length (loops excluded).</returns>
    float Prepend(IHOTweenComponent twMember);

    /// <summary>
    /// Inserts the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> at the given time,
    /// and returns the new Sequence total time length (loops excluded).
    /// </summary>
    /// <param name="time">
    /// The time at which the element must be placed.
    /// </param>
    /// <param name="twMember">
    /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to insert.
    /// </param>
    /// <returns>The new Sequence total time length (loops excluded).</returns>
    float Insert(float time, IHOTweenComponent twMember);

    /// <summary>
    /// Clears this sequence and resets its parameters, so it can be re-used.
    /// You can check if a Sequence is clean by querying its isEmpty property.
    /// </summary>
    /// <param name="parms">
    /// New parameters for the Sequence
    /// (if NULL, note that the dafult ones will be used, and not the previous ones)
    /// </param>
    void Clear(SequenceParms parms = null);

}

}