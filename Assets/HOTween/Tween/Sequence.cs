using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins.Core;
using System.Collections.Generic;
using Packages.Rider.Editor.UnitTesting;
using UnityEngine;

using TweenCallback = Holoville.HOTween.Core.TweenDelegate.TweenCallback;
using TweenCallbackWParms = Holoville.HOTween.Core.TweenDelegate.TweenCallbackWParms;

namespace Holoville.HOTween {

public class Sequence : ABSTweenComponent, ISequence
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

    public Sequence()
        : this(null)
    {
    }

    public Sequence(SequenceParms parms)
    {
        parms?.InitializeSequence(this);
        _isPaused = true;
        HOTween.AddSequence(this);
    }

    public void AppendCallback(TweenCallback callback) =>
        InsertCallback(Duration, callback);

    public void AppendCallback(TweenCallbackWParms callback, params object[] callbackParms) =>
        InsertCallback(Duration, callback, callbackParms);

    public void AppendCallback(GameObject sendMessageTarget, string methodName, object value, SendMessageOptions options = SendMessageOptions.RequireReceiver) =>
        InsertCallback(Duration, sendMessageTarget, methodName, value, options);

    public void InsertCallback(float time, TweenCallback callback) =>
        InsertCallback(time, callback, null, null);

    public void InsertCallback(float time, TweenCallbackWParms callback, params object[] callbackParms) =>
        InsertCallback(time, null, callback, callbackParms);

    public void InsertCallback(float time, GameObject sendMessageTarget, string methodName, object value, SendMessageOptions options = SendMessageOptions.RequireReceiver)
    {
        var callbackWParms = new TweenCallbackWParms(HOTween.DoSendMessage);
        var objArray = new object[4]
        {
            sendMessageTarget,
            methodName,
            value,
            options
        };
        InsertCallback(time, null, callbackWParms, objArray);
    }

    private void InsertCallback(float time, TweenCallback callback, TweenCallbackWParms callbackWParms, params object[] callbackParms)
    {
        hasCallbacks = true;
        var hotSeqItem = new HOTSeqItem(time, callback, callbackWParms, callbackParms);
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
                if (items[index].startTime >= (double)time)
                {
                    items.Insert(index, hotSeqItem);
                    flag = true;
                    break;
                }
            }

            if (!flag)
                items.Add(hotSeqItem);
        }

        IsEmpty = false;
    }

    public float AppendInterval(float pDuration) =>
        Append(null, pDuration);

    public float Append(IHOTweenComponent twMember) =>
        Append(twMember, 0.0f);

    private float Append(IHOTweenComponent twMember, float pDuration)
                                                                 {
                                                                 if (items == null) return twMember == null ? Insert(0.0f, null, pDuration) : Insert(0.0f, twMember);
                                                                 
                                                                 if (twMember != null)
                                                                 {
                                                                 HOTween.RemoveFromTweens(twMember);
                                                                 ((ABSTweenComponent)twMember).contSequence = this;
                                                                 CheckSpeedBasedTween(twMember);
                                                                 }
                                                                 
                                                                 var hotSeqItem = twMember != null
                                                                 ? new HOTSeqItem(Duration, twMember as ABSTweenComponent)
                                                                 : new HOTSeqItem(Duration, pDuration);
                                                                 items.Add(hotSeqItem);
                                                                 Duration += hotSeqItem.duration;
                                                                 SetFullDuration();
                                                                 IsEmpty = false;
                                                                 return Duration;
                                                                 }

    public float PrependInterval(float pDuration) =>
        Prepend(null, pDuration);

    public float Prepend(IHOTweenComponent twMember) =>
        Prepend(twMember, 0.0f);

    private float Prepend(IHOTweenComponent twMember, float pDuration)
    {
        if (items == null) return Insert(0.0f, twMember);
        
        if (twMember != null)
        {
            HOTween.RemoveFromTweens(twMember);
            ((ABSTweenComponent)twMember).contSequence = this;
            CheckSpeedBasedTween(twMember);
        }

        var hotSeqItem = twMember != null
            ? new HOTSeqItem(0.0f, twMember as ABSTweenComponent)
            : new HOTSeqItem(0.0f, pDuration);
        var duration = hotSeqItem.duration;
        var count = items.Count;
        for (var index = 0; index < count; ++index)
            items[index].startTime += duration;
        items.Insert(0, hotSeqItem);
        Duration += duration;
        SetFullDuration();
        IsEmpty = false;
        return Duration;
    }

    public float Insert(float time, IHOTweenComponent twMember) =>
        Insert(time, twMember, 0.0f);

    private float Insert(float time, IHOTweenComponent twMember, float pDuration)
    {
        if (twMember != null)
        {
            HOTween.RemoveFromTweens(twMember);
            ((ABSTweenComponent)twMember).contSequence = this;
            CheckSpeedBasedTween(twMember);
        }

        var hotSeqItem = twMember != null
            ? new HOTSeqItem(time, twMember as ABSTweenComponent)
            : new HOTSeqItem(time, pDuration);
            
        if (items == null)
        {
            items = new List<HOTSeqItem>()
            {
                hotSeqItem
            };
            Duration = hotSeqItem.startTime + hotSeqItem.duration;
            SetFullDuration();
            IsEmpty = false;
            return Duration;
        }

        var flag = false;
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            if (items[index].startTime >= (double)time)
            {
                items.Insert(index, hotSeqItem);
                flag = true;
                break;
            }
        }

        if (!flag)
            items.Add(hotSeqItem);
        Duration = Mathf.Max(hotSeqItem.startTime + hotSeqItem.duration, Duration);
        SetFullDuration();
        IsEmpty = false;
        return Duration;
    }

    public void Clear(SequenceParms parms = null)
    {
        Kill(false);
        Reset();
        hasCallbacks = false;
        prevIncrementalCompletedLoops = prevIncrementalCompletedLoops = 0;
        Destroyed = false;
        parms?.InitializeSequence(this);
        _isPaused = true;
    }

    internal override void Kill(bool autoRemoveFromHOTween)
    {
        if (Destroyed) return;
        
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

        base.Kill(autoRemoveFromHOTween);
    }

    public override void Rewind() =>
        Rewind(false);

    public override void Restart()
    {
        if (FullElapsed == 0.0)
            PlayForward();
        else
            Rewind(true);
    }

    public override bool IsTweening(object target)
    {
        if (!_enabled || items == null) return false;
        
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(target))
                return true;
        }

        return false;
    }

    public override bool IsTweening(string id)
    {
        if (!_enabled || items == null) return false;
        if (!_isPaused && _id == id) return true;
        
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(id))
                return true;
        }

        return false;
    }

    public override bool IsTweening(int id)
    {
        if (!_enabled || items == null) return false;
        if (!_isPaused && _intId == id) return true;
        
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsTweening(id))
                return true;
        }

        return false;
    }

    public override bool IsLinkedTo(object target)
    {
        if (items == null) return false;
        
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember.IsLinkedTo(target))
                return true;
        }

        return false;
    }

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

    public List<Tweener> GetTweenersByTarget(object target)
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
                    if (twMember3.target == target)
                        tweenerList.Add(twMember3);
                }
                else
                    tweenerList.AddRange(((Sequence)hotSeqItem.twMember).GetTweenersByTarget(target));
            }
        }

        return tweenerList;
    }

    internal override List<IHOTweenComponent> GetTweensById(string id)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (base.id == id)
            hoTweenComponentList.Add(this);
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween)
                hoTweenComponentList.AddRange(hotSeqItem.twMember.GetTweensById(id));
        }

        return hoTweenComponentList;
    }

    internal override List<IHOTweenComponent> GetTweensByIntId(int intId)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (base.intId == intId)
            hoTweenComponentList.Add(this);
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween)
                hoTweenComponentList.AddRange(hotSeqItem.twMember.GetTweensByIntId(intId));
        }

        return hoTweenComponentList;
    }

    internal void Remove(ABSTweenComponent tween)
    {
        if (items == null) return;
        
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.twMember == tween)
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

    internal override void Complete(bool autoRemoveFromHOTween)
    {
        if (!_enabled || items == null || _loops < 0) return;
        
        FullElapsed = FullDuration;
        Update(0.0f, true);
        
        if (!_autoKillOnComplete) return;
        
        Kill(autoRemoveFromHOTween);
    }

    internal override bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration, bool ignoreCallbacks)
    {
        if (Destroyed || items == null) return true;
        if (!_enabled) return false;
        if (IsComplete && !IsReversed && !forceUpdate) return true;
        if (FullElapsed == 0.0 && IsReversed && !forceUpdate || _isPaused && !forceUpdate) return false;
        
        base.ignoreCallbacks = isStartupIteration || ignoreCallbacks;
        
        if (!IsReversed)
        {
            FullElapsed += shortElapsed;
            Elapsed += shortElapsed;
        }
        else
        {
            FullElapsed -= shortElapsed;
            Elapsed -= shortElapsed;
        }

        if (FullElapsed > (double)FullDuration)
            FullElapsed = FullDuration;
        else if (FullElapsed < 0.0)
            FullElapsed = 0.0f;
        Startup();
        if (!HasStarted)
            OnStart();
        var isComplete = IsComplete;
        var flag1 = !IsReversed && !isComplete && Elapsed >= (double)Duration;
        SetLoops();
        SetElapsed();
        IsComplete = !IsReversed && _loops >= 0 && CompletedLoops >= _loops;
        var flag2 = !isComplete && IsComplete;
        if (_loopType == LoopType.Incremental)
        {
            if (prevIncrementalCompletedLoops != CompletedLoops)
            {
                var completedLoops = CompletedLoops;
                if (_loops != -1 && completedLoops >= _loops)
                    --completedLoops;
                var diffIncr = completedLoops - prevIncrementalCompletedLoops;
                if (diffIncr != 0)
                {
                    SetIncremental(diffIncr);
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
                    var flag3 = PrevCompletedLoops != CompletedLoops;
                    var flag4 = (_loopType == LoopType.Yoyo || _loopType == LoopType.YoyoInverse) &&
                                (IsLoopingBack && !flag3 || flag3 && !IsLoopingBack);
                    var num1 = IsLoopingBack ? Duration - Elapsed : Elapsed;
                    var num2 = IsLoopingBack ? Duration - prevElapsed : prevElapsed;
                    if (!IsLoopingBack
                        ? !flag4 &&
                          (hotSeqItem.startTime <= (double)num1 || CompletedLoops != PrevCompletedLoops) &&
                          hotSeqItem.startTime >= (double)num2 ||
                          hotSeqItem.startTime <= (double)num1 &&
                          (!IsComplete && CompletedLoops != PrevCompletedLoops ||
                           hotSeqItem.startTime >= (double)num2)
                        : flag4 &&
                        (hotSeqItem.startTime >= (double)num1 || CompletedLoops != PrevCompletedLoops) &&
                        hotSeqItem.startTime <= (double)num2 || hotSeqItem.startTime >= (double)num1 &&
                        (!IsComplete && CompletedLoops != PrevCompletedLoops ||
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

        if (Duration > 0.0)
        {
            var num = !IsLoopingBack ? Elapsed : Duration - Elapsed;
            for (var index = count - 1; index > -1; --index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.startTime > (double)num)
                {
                    if (hotSeqItem.twMember.Duration > 0.0)
                        hotSeqItem.twMember.GoTo(num - hotSeqItem.startTime, forceUpdate, true);
                    else
                        hotSeqItem.twMember.Rewind();
                }
            }

            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = items[index];
                if (hotSeqItem.seqItemType == SeqItemType.Tween && hotSeqItem.startTime <= (double)num)
                {
                    if (hotSeqItem.twMember.Duration > 0.0)
                        hotSeqItem.twMember.GoTo(num - hotSeqItem.startTime, forceUpdate);
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

        if (FullElapsed != (double)PrevFullElapsed)
        {
            OnUpdate();
            if (FullElapsed == 0.0)
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

        base.ignoreCallbacks = false;
        prevElapsed = Elapsed;
        PrevFullElapsed = FullElapsed;
        PrevCompletedLoops = CompletedLoops;
        return flag2;
    }

    internal override void SetIncremental(int diffIncr)
    {
        var count = items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween)
                hotSeqItem.twMember.SetIncremental(diffIncr);
        }
    }

    protected override bool GoTo(float time, bool play, bool forceUpdate, bool ignoreCallbacks)
    {
        if (!_enabled) return false;
        
        if (time > (double)FullDuration)
            time = FullDuration;
        else if (time < 0.0)
            time = 0.0f;
            
        if (FullElapsed == (double)time && !forceUpdate)
        {
            if (!IsComplete && play)
                Play();
            return IsComplete;
        }

        FullElapsed = time;
        Update(0.0f, true, false, ignoreCallbacks);
        if (!IsComplete && play)
            Play();
        return IsComplete;
    }

    private void Rewind(bool play)
    {
        if (!_enabled || items == null) return;
        
        Startup();
        if (!HasStarted)
            OnStart();
        IsComplete = false;
        IsLoopingBack = false;
        CompletedLoops = 0;
        FullElapsed = Elapsed = 0.0f;
        for (var index = items.Count - 1; index > -1; --index)
        {
            var hotSeqItem = items[index];
            if (hotSeqItem.seqItemType == SeqItemType.Tween)
                hotSeqItem.twMember.Rewind();
        }

        if (FullElapsed != (double)PrevFullElapsed)
        {
            OnUpdate();
            if (FullElapsed == 0.0)
                OnRewinded();
        }

        PrevFullElapsed = FullElapsed;
        if (play)
            Play();
        else
            Pause();
    }

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
                hotSeqItem.twMember.Update(hotSeqItem.twMember.Duration, true, true);
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

    private static void CheckSpeedBasedTween(IHOTweenComponent twMember)
    {
        if (!(twMember is Tweener tweener) || !tweener._speedBased) return;
        
        tweener.ForceSetSpeedBasedDuration();
    }

    protected override void Startup()
    {
        if (startupDone) return;
        
        TweenStartupIteration();
        base.Startup();
    }

    internal override void FillPluginsList(List<ABSTweenPlugin> plugs)
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
                    twMember3.FillPluginsList(plugs);
                else
                    hotSeqItem.twMember.FillPluginsList(plugs);
            }
        }
    }

    private enum SeqItemType
    {
        Interval,
        Tween,
        Callback,
    }

    private class HOTSeqItem
    {
        public readonly SeqItemType seqItemType;
        public readonly TweenCallback callback;
        public readonly TweenCallbackWParms callbackWParms;
        public readonly object[] callbackParms;
        public float startTime;
        private readonly float Duration;
        public readonly ABSTweenComponent twMember;

        public float duration => twMember == null ? Duration : twMember.Duration;

        public HOTSeqItem(float startTime, ABSTweenComponent twMember)
        {
            this.startTime = startTime;
            this.twMember = twMember;
            this.twMember.autoKillOnComplete = false;
            seqItemType = SeqItemType.Tween;
        }

        public HOTSeqItem(float startTime, float pDuration)
        {
            seqItemType = SeqItemType.Interval;
            this.startTime = startTime;
            Duration = pDuration;
        }

        public HOTSeqItem(float startTime, TweenCallback callback, TweenCallbackWParms callbackWParms, params object[] callbackParms)
        {
            seqItemType = SeqItemType.Callback;
            this.startTime = startTime;
            this.callback = callback;
            this.callbackWParms = callbackWParms;
            this.callbackParms = callbackParms;
        }
    }
}

}