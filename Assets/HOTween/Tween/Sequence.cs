using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOTween
{
    /// <summary>
    /// Sequence component. Manager for sequence of Tweeners or other nested Sequences.
    /// <para>Author: Daniele Giardini (http://www.holoville.com)</para>
    /// </summary>
    public class Sequence : ABSTweenComponent
    {
        private bool hasCallbacks;
        private int prevIncrementalCompletedLoops;
        private float prevElapsed;
        private List<HOTSeqItem> items;

        internal override bool steadyIgnoreCallbacks
        {
            get => _steadyIgnoreCallbacks;
            set
            {
                _steadyIgnoreCallbacks = value;
                if (items == null)
                    return;
                var count = items.Count;
                for (var index = 0; index < count; ++index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.twMember != null)
                        hotSeqItem.twMember.steadyIgnoreCallbacks = value;
                }
            }
        }

        /// <summary>Creates a new Sequence without any parameter.</summary>
        public Sequence()
            : this(null)
        {
        }

        /// <summary>Creates a new Sequence.</summary>
        /// <param name="p_parms">
        /// A <see cref="T:Holoville.HOTween.SequenceParms" /> representing the Sequence parameters.
        /// You can pass an existing one, or create a new one inline via method chaining,
        /// like <c>new SequenceParms().Id("sequence1").Loops(2).OnComplete(myFunction)</c>
        /// </param>
        public Sequence(SequenceParms p_parms)
        {
            p_parms?.InitializeSequence(this);
            _isPaused = true;
            HOTween.AddSequence(this);
        }

        /// <summary>Appends the given callback to this Sequence.</summary>
        /// <param name="p_callback">The function to call, who must return <c>void</c> and accept no parameters</param>
        public void AppendCallback(TweenDelegate.TweenCallback p_callback) => InsertCallback(_duration, p_callback);

        /// <summary>Appends the given callback to this Sequence.</summary>
        /// <param name="p_callback">The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" /></param>
        /// <param name="p_callbackParms">Additional comma separated parameters to pass to the function</param>
        public void AppendCallback(
            TweenDelegate.TweenCallbackWParms p_callback,
            params object[] p_callbackParms)
        {
            InsertCallback(_duration, p_callback, p_callbackParms);
        }

        /// <summary>Appends the given SendMessage callback to this Sequence.</summary>
        /// <param name="p_sendMessageTarget">GameObject to target for sendMessage</param>
        /// <param name="p_methodName">Name of the method to call</param>
        /// <param name="p_value">Eventual additional parameter</param>
        /// <param name="p_options">SendMessageOptions</param>
        public void AppendCallback(
            GameObject p_sendMessageTarget,
            string p_methodName,
            object p_value,
            SendMessageOptions p_options = SendMessageOptions.RequireReceiver)
        {
            InsertCallback(_duration, p_sendMessageTarget, p_methodName, p_value, p_options);
        }

        /// <summary>Inserts the given callback at the given time position.</summary>
        /// <param name="p_time">Time position where this callback will be placed
        /// (if longer than the whole sequence duration, the callback will never be called)</param>
        /// <param name="p_callback">The function to call, who must return <c>void</c> and accept no parameters</param>
        public void InsertCallback(float p_time, TweenDelegate.TweenCallback p_callback) =>
            InsertCallback(p_time, p_callback, null, null);

        /// <summary>Inserts the given callback at the given time position.</summary>
        /// <param name="p_time">Time position where this callback will be placed
        /// (if longer than the whole sequence duration, the callback will never be called)</param>
        /// <param name="p_callback">The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" /></param>
        /// <param name="p_callbackParms">Additional comma separated parameters to pass to the function</param>
        public void InsertCallback(
            float p_time,
            TweenDelegate.TweenCallbackWParms p_callback,
            params object[] p_callbackParms)
        {
            InsertCallback(p_time, null, p_callback, p_callbackParms);
        }

        /// <summary>Inserts the given SendMessage callback at the given time position.</summary>
        /// <param name="p_time">Time position where this callback will be placed
        /// (if longer than the whole sequence duration, the callback will never be called)</param>
        /// <param name="p_sendMessageTarget">GameObject to target for sendMessage</param>
        /// <param name="p_methodName">Name of the method to call</param>
        /// <param name="p_value">Eventual additional parameter</param>
        /// <param name="p_options">SendMessageOptions</param>
        public void InsertCallback(
            float p_time,
            GameObject p_sendMessageTarget,
            string p_methodName,
            object p_value,
            SendMessageOptions p_options = SendMessageOptions.RequireReceiver)
        {
            var p_callbackWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
            var objArray = new object[4]
            {
                p_sendMessageTarget,
                p_methodName,
                p_value,
                p_options
            };
            InsertCallback(p_time, null, p_callbackWParms, objArray);
        }

        private void InsertCallback(
            float p_time,
            TweenDelegate.TweenCallback p_callback,
            TweenDelegate.TweenCallbackWParms p_callbackWParms,
            params object[] p_callbackParms)
        {
            hasCallbacks = true;
            var hotSeqItem = new HOTSeqItem(p_time, p_callback, p_callbackWParms, p_callbackParms);
            if (items == null)
            {
                items = new List<HOTSeqItem>()
                {
                    hotSeqItem
                };
            }
            else
            {
                var flag = false;
                var count = items.Count;
                for (var index = 0; index < count; ++index)
                {
                    if (items[index].startTime >= (double)p_time)
                    {
                        items.Insert(index, hotSeqItem);
                        flag = true;
                        break;
                    }
                }

                if (!flag)
                    items.Add(hotSeqItem);
            }

            _isEmpty = false;
        }

        /// <summary>
        /// Appends an interval to the right of the sequence,
        /// and returns the new Sequence total time length (loops excluded).
        /// </summary>
        /// <param name="p_duration">The duration of the interval.</param>
        /// <returns>The new Sequence total time length (loops excluded).</returns>
        public float AppendInterval(float p_duration) => Append(null, p_duration);

        /// <summary>
        /// Adds the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to the right of the sequence,
        /// and returns the new Sequence total time length (loops excluded).
        /// </summary>
        /// <param name="p_twMember">
        /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to append.
        /// </param>
        /// <returns>The new Sequence total time length (loops excluded).</returns>
        public float Append(IHOTweenComponent p_twMember) => Append(p_twMember, 0.0f);

        private float Append(IHOTweenComponent p_twMember, float p_duration)
        {
            if (items == null) return p_twMember == null ? Insert(0.0f, null, p_duration) : Insert(0.0f, p_twMember);
            
            if (p_twMember != null)
            {
                HOTween.RemoveFromTweens(p_twMember);
                ((ABSTweenComponent)p_twMember).contSequence = this;
                CheckSpeedBasedTween(p_twMember);
            }

            var hotSeqItem = p_twMember != null
                ? new HOTSeqItem(_duration, p_twMember as ABSTweenComponent)
                : new HOTSeqItem(_duration, p_duration);
            items.Add(hotSeqItem);
            _duration += hotSeqItem.duration;
            SetFullDuration();
            _isEmpty = false;
            return _duration;
        }

        /// <summary>
        /// Prepends an interval to the left of the sequence,
        /// and returns the new Sequence total time length (loops excluded).
        /// </summary>
        /// <param name="p_duration">The duration of the interval.</param>
        /// <returns>The new Sequence total time length (loops excluded).</returns>
        public float PrependInterval(float p_duration) => Prepend(null, p_duration);

        /// <summary>
        /// Adds the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to the left of the sequence,
        /// moving all the existing sequence elements to the right,
        /// and returns the new Sequence total time length (loops excluded).
        /// </summary>
        /// <param name="p_twMember">
        /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to prepend.
        /// </param>
        /// <returns>The new Sequence total time length (loops excluded).</returns>
        public float Prepend(IHOTweenComponent p_twMember) => Prepend(p_twMember, 0.0f);

        private float Prepend(IHOTweenComponent p_twMember, float p_duration)
        {
            if (items == null) return Insert(0.0f, p_twMember);
            
            if (p_twMember != null)
            {
                HOTween.RemoveFromTweens(p_twMember);
                ((ABSTweenComponent)p_twMember).contSequence = this;
                CheckSpeedBasedTween(p_twMember);
            }

            var hotSeqItem = p_twMember != null
                ? new HOTSeqItem(0.0f, p_twMember as ABSTweenComponent)
                : new HOTSeqItem(0.0f, p_duration);
            var duration = hotSeqItem.duration;
            var count = items.Count;
            for (var index = 0; index < count; ++index)
                items[index].startTime += duration;
            items.Insert(0, hotSeqItem);
            _duration += duration;
            SetFullDuration();
            _isEmpty = false;
            return _duration;
        }

        /// <summary>
        /// Inserts the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> at the given time,
        /// and returns the new Sequence total time length (loops excluded).
        /// </summary>
        /// <param name="p_time">
        /// The time at which the element must be placed.
        /// </param>
        /// <param name="p_twMember">
        /// The <see cref="T:Holoville.HOTween.IHOTweenComponent" /> to insert.
        /// </param>
        /// <returns>The new Sequence total time length (loops excluded).</returns>
        public float Insert(float p_time, IHOTweenComponent p_twMember) => Insert(p_time, p_twMember, 0.0f);

        private float Insert(float p_time, IHOTweenComponent p_twMember, float p_duration)
        {
            if (p_twMember != null)
            {
                HOTween.RemoveFromTweens(p_twMember);
                ((ABSTweenComponent)p_twMember).contSequence = this;
                CheckSpeedBasedTween(p_twMember);
            }

            var hotSeqItem = p_twMember != null
                ? new HOTSeqItem(p_time, p_twMember as ABSTweenComponent)
                : new HOTSeqItem(p_time, p_duration);
                
            if (items == null)
            {
                items = new List<HOTSeqItem>()
                {
                    hotSeqItem
                };
                _duration = hotSeqItem.startTime + hotSeqItem.duration;
                SetFullDuration();
                _isEmpty = false;
                return _duration;
            }

            var flag = false;
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                if (items[index].startTime >= (double)p_time)
                {
                    items.Insert(index, hotSeqItem);
                    flag = true;
                    break;
                }
            }

            if (!flag)
                items.Add(hotSeqItem);
            _duration = Mathf.Max(hotSeqItem.startTime + hotSeqItem.duration, _duration);
            SetFullDuration();
            _isEmpty = false;
            return _duration;
        }

        /// <summary>
        /// Clears this sequence and resets its parameters, so it can be re-used.
        /// You can check if a Sequence is clean by querying its isEmpty property.
        /// </summary>
        /// <param name="p_parms">
        /// New parameters for the Sequence
        /// (if NULL, note that the dafult ones will be used, and not the previous ones)
        /// </param>
        public void Clear(SequenceParms p_parms = null)
        {
            Kill(false);
            Reset();
            hasCallbacks = false;
            prevIncrementalCompletedLoops = prevIncrementalCompletedLoops = 0;
            _destroyed = false;
            p_parms?.InitializeSequence(this);
            _isPaused = true;
        }

        /// <summary>Kills this Sequence and cleans it.</summary>
        /// <param name="p_autoRemoveFromHOTween">
        /// If <c>true</c> also calls <c>HOTween.Kill(this)</c> to remove it from HOTween.
        /// Set internally to <c>false</c> when I already know that HOTween is going to remove it.
        /// </param>
        internal override void Kill(bool p_autoRemoveFromHOTween)
        {
            if (_destroyed) return;
            
            if (items != null)
            {
                var count = items.Count;
                for (var index = 0; index < count; ++index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.seqItemType == SeqItemType.Tween)
                        hotSeqItem.twMember.Kill(false);
                }

                items = null;
            }

            base.Kill(p_autoRemoveFromHOTween);
        }

        /// <summary>
        /// Rewinds this Sequence (loops included), and pauses it.
        /// </summary>
        public override void Rewind() => Rewind(false);

        /// <summary>
        /// Restarts this Sequence from the beginning (loops included).
        /// </summary>
        public override void Restart()
        {
            if (_fullElapsed == 0.0)
                PlayForward();
            else
                Rewind(true);
        }

        /// <summary>
        /// Returns <c>true</c> if the given target is currently involved in a running tween of this Sequence (taking into account also nested tweens).
        /// Returns <c>false</c> both if the given target is not inside any of this Sequence tweens, than if the relative tween is paused.
        /// To simply check if the target is attached to a tween of this Sequence, use <c>IsLinkedTo( target )</c> instead.
        /// </summary>
        /// <param name="p_target">The target to check.</param>
        public override bool IsTweening(object p_target)
        {
            if (!_enabled || items == null) return false;
            
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(p_target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the tween with the given string id is currently involved in a running tween or sequence.
        /// </summary>
        /// <param name="p_id">The id to check for.</param>
        public override bool IsTweening(string p_id)
        {
            if (!_enabled || items == null) return false;
            if (!_isPaused && _id == p_id) return true;
            
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(p_id))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the tween with the given int id is currently involved in a running tween or sequence.
        /// </summary>
        /// <param name="p_id">The id to check for.</param>
        public override bool IsTweening(int p_id)
        {
            if (!_enabled || items == null) return false;
            if (!_isPaused && _intId == p_id) return true;
            
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(p_id))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the given target is linked to a tween of this Sequence (running or not, taking into account also nested tweens).
        /// </summary>
        /// <param name="p_target">The target to check.</param>
        /// <returns>
        /// A value of <c>true</c> if the given target is linked to a tween of this Sequence (running or not, taking into account also nested tweens).
        /// </returns>
        public override bool IsLinkedTo(object p_target)
        {
            if (items == null) return false;
            
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsLinkedTo(p_target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a list of all the targets of this Sequence, or NULL if there are none.
        /// </summary>
        /// <returns>A list of all the targets of this Sequence, or NULL if there are none.</returns>
        public override List<object> GetTweenTargets()
        {
            if (items == null)
                return null;
            var objectList = new List<object>();
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    objectList.AddRange(hotSeqItem.twMember.GetTweenTargets());
            }

            return objectList;
        }

        /// <summary>
        /// Returns a list of the eventual nested <see cref="T:Holoville.HOTween.Tweener" /> objects whose target is the given one,
        /// or an empty list if none was found.
        /// </summary>
        public List<Tweener> GetTweenersByTarget(object p_target)
        {
            var tweenerList = new List<Tweener>();
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                {
                    if (hotSeqItem.twMember is Tweener twMember3)
                    {
                        if (twMember3.target == p_target)
                            tweenerList.Add(twMember3);
                    }
                    else
                        tweenerList.AddRange(((Sequence)hotSeqItem.twMember).GetTweenersByTarget(p_target));
                }
            }

            return tweenerList;
        }

        /// <summary>
        /// Returns a list of the eventual existing tweens with the given Id within this Sequence,
        /// nested tweens included (or an empty list if no tweens were found).
        /// </summary>
        internal override List<IHOTweenComponent> GetTweensById(string p_id)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (id == p_id)
                hoTweenComponentList.Add(this);
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hoTweenComponentList.AddRange(hotSeqItem.twMember.GetTweensById(p_id));
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns a list of the eventual existing tweens with the given Id within this Sequence,
        /// nested tweens included (or an empty list if no tweens were found).
        /// </summary>
        internal override List<IHOTweenComponent> GetTweensByIntId(int p_intId)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (intId == p_intId)
                hoTweenComponentList.Add(this);
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hoTweenComponentList.AddRange(hotSeqItem.twMember.GetTweensByIntId(p_intId));
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Removes the given tween from this Sequence,
        /// and eventually kills the Sequence if all items have been removed.
        /// Used by <see cref="T:Holoville.HOTween.Core.OverwriteManager" /> to remove overwritten tweens.
        /// </summary>
        internal void Remove(ABSTweenComponent p_tween)
        {
            if (items == null) return;
            
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember == p_tween)
                {
                    items.RemoveAt(index);
                    break;
                }
            }

            if (items.Count != 0) return;
            
            if (isSequenced)
                contSequence.Remove(this);
            Kill(!isSequenced);
        }

        /// <summary>
        /// Completes this Sequence.
        /// Where a loop was involved, the Sequence completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        internal override void Complete(bool p_autoRemoveFromHOTween)
        {
            if (!_enabled || items == null || _loops < 0) return;
            
            _fullElapsed = _fullDuration;
            Update(0.0f, true);
            
            if (!_autoKillOnComplete) return;
            
            Kill(p_autoRemoveFromHOTween);
        }

        /// <summary>
        /// Updates the Sequence by the given elapsed time,
        /// and returns a value of <c>true</c> if the Sequence is complete.
        /// </summary>
        /// <param name="p_shortElapsed">
        /// The elapsed time since the last update.
        /// </param>
        /// <param name="p_forceUpdate">
        /// If <c>true</c> forces the update even if the Sequence is complete or paused,
        /// but ignores onUpdate, and sends onComplete and onStepComplete calls only if the Sequence wasn't complete before this call.
        /// </param>
        /// <param name="p_isStartupIteration">
        /// If <c>true</c> means the update is due to a startup iteration (managed by Sequence Startup),
        /// and all callbacks will be ignored.
        /// </param>
        /// <param name="p_ignoreCallbacks">
        /// If <c>true</c> doesn't call any callback method.
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the Sequence is not reversed and is complete (or all the Sequence tween targets don't exist anymore), otherwise <c>false</c>.
        /// </returns>
        internal override bool Update(float p_shortElapsed, bool p_forceUpdate, bool p_isStartupIteration, bool p_ignoreCallbacks)
        {
            if (_destroyed || items == null) return true;
            if (!_enabled) return false;
            if (_isComplete && !_isReversed && !p_forceUpdate) return true;
            if (_fullElapsed == 0.0 && _isReversed && !p_forceUpdate || _isPaused && !p_forceUpdate) return false;
            
            ignoreCallbacks = p_isStartupIteration || p_ignoreCallbacks;
            
            if (!_isReversed)
            {
                _fullElapsed += p_shortElapsed;
                _elapsed += p_shortElapsed;
            }
            else
            {
                _fullElapsed -= p_shortElapsed;
                _elapsed -= p_shortElapsed;
            }

            if (_fullElapsed > (double)_fullDuration)
                _fullElapsed = _fullDuration;
            else if (_fullElapsed < 0.0)
                _fullElapsed = 0.0f;
            Startup();
            if (!_hasStarted)
                OnStart();
            var isComplete = _isComplete;
            var flag1 = !_isReversed && !isComplete && _elapsed >= (double)_duration;
            SetLoops();
            SetElapsed();
            _isComplete = !_isReversed && _loops >= 0 && _completedLoops >= _loops;
            var flag2 = !isComplete && _isComplete;
            if (_loopType == LoopType.Incremental)
            {
                if (prevIncrementalCompletedLoops != _completedLoops)
                {
                    var completedLoops = _completedLoops;
                    if (_loops != -1 && completedLoops >= _loops)
                        --completedLoops;
                    var p_diffIncr = completedLoops - prevIncrementalCompletedLoops;
                    if (p_diffIncr != 0)
                    {
                        SetIncremental(p_diffIncr);
                        prevIncrementalCompletedLoops = completedLoops;
                    }
                }
            }
            else if (prevIncrementalCompletedLoops != 0)
            {
                SetIncremental(-prevIncrementalCompletedLoops);
                prevIncrementalCompletedLoops = 0;
            }

            var count = items.Count;
            if (hasCallbacks && !_isPaused)
            {
                List<HOTSeqItem> hotSeqItemList = null;
                for (var index = 0; index < count; ++index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.seqItemType == SeqItemType.Callback)
                    {
                        var flag3 = prevCompletedLoops != _completedLoops;
                        var flag4 = (_loopType == LoopType.Yoyo || _loopType == LoopType.YoyoInverse) &&
                                    (_isLoopingBack && !flag3 || flag3 && !_isLoopingBack);
                        var num1 = _isLoopingBack ? _duration - _elapsed : _elapsed;
                        var num2 = _isLoopingBack ? _duration - prevElapsed : prevElapsed;
                        if (!_isLoopingBack
                            ? !flag4 &&
                              (hotSeqItem.startTime <= (double)num1 || _completedLoops != prevCompletedLoops) &&
                              hotSeqItem.startTime >= (double)num2 ||
                              hotSeqItem.startTime <= (double)num1 &&
                              (!_isComplete && _completedLoops != prevCompletedLoops ||
                               hotSeqItem.startTime >= (double)num2)
                            : flag4 &&
                            (hotSeqItem.startTime >= (double)num1 || _completedLoops != prevCompletedLoops) &&
                            hotSeqItem.startTime <= (double)num2 || hotSeqItem.startTime >= (double)num1 &&
                            (!_isComplete && _completedLoops != prevCompletedLoops ||
                             hotSeqItem.startTime <= (double)num2))
                        {
                            if (hotSeqItemList == null)
                                hotSeqItemList = new List<HOTSeqItem>();
                            if (hotSeqItem.startTime > (double)num1)
                                hotSeqItemList.Insert(0, hotSeqItem);
                            else
                                hotSeqItemList.Add(hotSeqItem);
                        }
                    }
                }

                if (hotSeqItemList != null)
                {
                    foreach (var hotSeqItem in hotSeqItemList)
                    {
                        if (hotSeqItem.callback != null)
                            hotSeqItem.callback();
                        else if (hotSeqItem.callbackWParms != null)
                            hotSeqItem.callbackWParms(new TweenEvent(this, hotSeqItem.callbackParms));
                    }
                }
            }

            if (_duration > 0.0)
            {
                var num = !_isLoopingBack ? _elapsed : _duration - _elapsed;
                for (var index = count - 1; index > -1; --index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.startTime > (double)num)
                    {
                        if (hotSeqItem.twMember.duration > 0.0)
                            hotSeqItem.twMember.GoTo(num - hotSeqItem.startTime, p_forceUpdate, true);
                        else
                            hotSeqItem.twMember.Rewind();
                    }
                }

                for (var index = 0; index < count; ++index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.startTime <= (double)num)
                    {
                        if (hotSeqItem.twMember.duration > 0.0)
                            hotSeqItem.twMember.GoTo(num - hotSeqItem.startTime, p_forceUpdate);
                        else
                            hotSeqItem.twMember.Complete();
                    }
                }
            }
            else
            {
                for (var index = count - 1; index > -1; --index)
                {
                    var hotSeqItem = items[index];
                    if (hotSeqItem.seqItemType == SeqItemType.Tween)
                        hotSeqItem.twMember.Complete();
                }

                if (!isComplete)
                    flag2 = true;
            }

            if (_fullElapsed != (double)prevFullElapsed)
            {
                OnUpdate();
                if (_fullElapsed == 0.0)
                {
                    if (!_isPaused)
                    {
                        _isPaused = true;
                        OnPause();
                    }

                    OnRewinded();
                }
            }

            if (flag2)
            {
                if (!_isPaused)
                {
                    _isPaused = true;
                    OnPause();
                }

                OnComplete();
            }
            else if (flag1)
                OnStepComplete();

            ignoreCallbacks = false;
            prevElapsed = _elapsed;
            prevFullElapsed = _fullElapsed;
            prevCompletedLoops = _completedLoops;
            return flag2;
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// Also called by Tweener.ApplySequenceIncrement (used by Sequences during Incremental loops).
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        internal override void SetIncremental(int p_diffIncr)
        {
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hotSeqItem.twMember.SetIncremental(p_diffIncr);
            }
        }

        /// <summary>
        /// Sends the sequence to the given time (taking also loops into account) and eventually plays it.
        /// If the time is bigger than the total sequence duration, it goes to the end.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the sequence reached its end and was completed.
        /// </returns>
        protected override bool GoTo(float p_time, bool p_play, bool p_forceUpdate, bool p_ignoreCallbacks)
        {
            if (!_enabled) return false;
            
            if (p_time > (double)_fullDuration)
                p_time = _fullDuration;
            else if (p_time < 0.0)
                p_time = 0.0f;
                
            if (_fullElapsed == (double)p_time && !p_forceUpdate)
            {
                if (!_isComplete && p_play)
                    Play();
                return _isComplete;
            }

            _fullElapsed = p_time;
            Update(0.0f, true, false, p_ignoreCallbacks);
            if (!_isComplete && p_play)
                Play();
            return _isComplete;
        }

        private void Rewind(bool p_play)
        {
            if (!_enabled || items == null) return;
            
            Startup();
            if (!_hasStarted)
                OnStart();
            _isComplete = false;
            _isLoopingBack = false;
            _completedLoops = 0;
            _fullElapsed = _elapsed = 0.0f;
            for (var index = items.Count - 1; index > -1; --index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hotSeqItem.twMember.Rewind();
            }

            if (_fullElapsed != (double)prevFullElapsed)
            {
                OnUpdate();
                if (_fullElapsed == 0.0)
                    OnRewinded();
            }

            prevFullElapsed = _fullElapsed;
            if (p_play)
                Play();
            else
                Pause();
        }

        /// <summary>
        /// Iterates through all the elements in order, to startup the plugins correctly.
        /// Called at OnStart and during Append/Insert/Prepend for speedBased tweens (to calculate correct duration).
        /// </summary>
        private void TweenStartupIteration()
        {
            var flag = !steadyIgnoreCallbacks;
            if (flag)
                steadyIgnoreCallbacks = true;
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hotSeqItem.twMember.Update(hotSeqItem.twMember.duration, true, true);
            }

            for (var index = count - 1; index > -1; --index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween)
                    hotSeqItem.twMember.Rewind();
            }

            if (!flag) return;
            
            steadyIgnoreCallbacks = false;
        }

        /// <summary>
        /// If the given <see cref="T:Holoville.HOTween.IHOTweenComponent" /> is a speedBased <see cref="T:Holoville.HOTween.Tweener" />,
        /// forces it to calculate the correct duration.
        /// </summary>
        private static void CheckSpeedBasedTween(IHOTweenComponent p_twMember)
        {
            if (!(p_twMember is Tweener tweener) || !tweener._speedBased) return;
            
            tweener.ForceSetSpeedBasedDuration();
        }

        /// <summary>
        /// Startup this tween
        /// (might or might not call OnStart, depending if the tween is in a Sequence or not).
        /// Can be executed only once per tween.
        /// </summary>
        protected override void Startup()
        {
            if (startupDone) return;
            
            TweenStartupIteration();
            base.Startup();
        }

        /// <summary>
        /// Fills the given list with all the plugins inside this sequence tween,
        /// while also looking for them recursively through inner sequences.
        /// Used by <c>HOTween.GetPlugins</c>.
        /// </summary>
        internal override void FillPluginsList(List<ABSTweenPlugin> p_plugs)
        {
            if (items == null)
                return;
            var count = items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.twMember != null)
                {
                    if (hotSeqItem.twMember is Sequence twMember3)
                        twMember3.FillPluginsList(p_plugs);
                    else
                        hotSeqItem.twMember.FillPluginsList(p_plugs);
                }
            }
        }

        private enum SeqItemType
        {
            Interval,
            Tween,
            Callback,
        }

        /// <summary>
        /// Single sequencer item.
        /// Tween value can be null (in case this is simply used as a spacer).
        /// </summary>
        private class HOTSeqItem
        {
            public readonly SeqItemType seqItemType;
            public readonly TweenDelegate.TweenCallback callback;
            public readonly TweenDelegate.TweenCallbackWParms callbackWParms;
            public readonly object[] callbackParms;
            public float startTime;
            private readonly float _duration;
            public readonly ABSTweenComponent twMember;

            public float duration => twMember == null ? _duration : twMember.duration;

            public HOTSeqItem(float p_startTime, ABSTweenComponent p_twMember)
            {
                startTime = p_startTime;
                twMember = p_twMember;
                twMember.autoKillOnComplete = false;
                seqItemType = SeqItemType.Tween;
            }

            public HOTSeqItem(float p_startTime, float p_duration)
            {
                seqItemType = SeqItemType.Interval;
                startTime = p_startTime;
                _duration = p_duration;
            }

            public HOTSeqItem(
                float p_startTime,
                TweenDelegate.TweenCallback p_callback,
                TweenDelegate.TweenCallbackWParms p_callbackWParms,
                params object[] p_callbackParms)
            {
                seqItemType = SeqItemType.Callback;
                startTime = p_startTime;
                callback = p_callback;
                callbackWParms = p_callbackWParms;
                callbackParms = p_callbackParms;
            }
        }
    }
}