using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins.Core;
using System.Collections.Generic;
using UnityEngine;

using TweenCallback = Holoville.HOTween.Core.TweenDelegate.TweenCallback;
using TweenCallbackWParms = Holoville.HOTween.Core.TweenDelegate.TweenCallbackWParms;

namespace Holoville.HOTween {

public class Sequence : ABSTweenComponent, ISequence
{
    private bool _hasCallbacks;
    private int _prevIncrementalCompletedLoops;
    private float _prevElapsed;
    private List<HOTSeqItem> _items;

    internal override bool SteadyIgnoreCallbacks
    {
        get => SteadyIgnoreCallbacks;
        set
        {
            SteadyIgnoreCallbacks = value;
            if (_items == null)
                return;
            var count = _items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.TwMember != null)
                    hotSeqItem.TwMember.SteadyIgnoreCallbacks = value;
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
        IsPaused = true;
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
        _hasCallbacks = true;
        var hotSeqItem = new HOTSeqItem(time, callback, callbackWParms, callbackParms);
        if (_items == null)
        {
            _items = new List<HOTSeqItem>()
            {
                hotSeqItem
            };
        }
        else
        {
            var flag = false;
            var count = _items.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_items[index].StartTime >= (double)time)
                {
                    _items.Insert(index, hotSeqItem);
                    flag = true;
                    break;
                }
            }

            if (!flag)
                _items.Add(hotSeqItem);
        }

        IsEmpty = false;
    }

    public float AppendInterval(float pDuration) =>
        Append(null, pDuration);

    public float Append(IHOTweenComponent twMember) =>
        Append(twMember, 0.0f);

    private float Append(IHOTweenComponent twMember, float pDuration)
    {
        if (_items == null) return twMember == null ? Insert(0.0f, null, pDuration) : Insert(0.0f, twMember);

        if (twMember != null)
        {
            HOTween.RemoveFromTweens(twMember);
            ((ABSTweenComponent)twMember).ContSequence = this;
            CheckSpeedBasedTween(twMember);
        }

        var hotSeqItem = twMember != null
            ? new HOTSeqItem(Duration, twMember as ABSTweenComponent)
            : new HOTSeqItem(Duration, pDuration);
        _items.Add(hotSeqItem);
        Duration += hotSeqItem.Duration;
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
        if (_items == null) return Insert(0.0f, twMember);
        
        if (twMember != null)
        {
            HOTween.RemoveFromTweens(twMember);
            ((ABSTweenComponent)twMember).ContSequence = this;
            CheckSpeedBasedTween(twMember);
        }

        var hotSeqItem = twMember != null
            ? new HOTSeqItem(0.0f, twMember as ABSTweenComponent)
            : new HOTSeqItem(0.0f, pDuration);
        var duration = hotSeqItem.Duration;
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
            _items[index].StartTime += duration;
        _items.Insert(0, hotSeqItem);
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
            ((ABSTweenComponent)twMember).ContSequence = this;
            CheckSpeedBasedTween(twMember);
        }

        var hotSeqItem = twMember != null
            ? new HOTSeqItem(time, twMember as ABSTweenComponent)
            : new HOTSeqItem(time, pDuration);
            
        if (_items == null)
        {
            _items = new List<HOTSeqItem>()
            {
                hotSeqItem
            };
            Duration = hotSeqItem.StartTime + hotSeqItem.Duration;
            SetFullDuration();
            IsEmpty = false;
            return Duration;
        }

        var flag = false;
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            if (_items[index].StartTime >= (double)time)
            {
                _items.Insert(index, hotSeqItem);
                flag = true;
                break;
            }
        }

        if (!flag)
            _items.Add(hotSeqItem);
        Duration = Mathf.Max(hotSeqItem.StartTime + hotSeqItem.Duration, Duration);
        SetFullDuration();
        IsEmpty = false;
        return Duration;
    }

    public void Clear(SequenceParms parms = null)
    {
        Kill(false);
        Reset();
        _hasCallbacks = false;
        _prevIncrementalCompletedLoops = _prevIncrementalCompletedLoops = 0;
        Destroyed = false;
        parms?.InitializeSequence(this);
        IsPaused = true;
    }

    internal override void Kill(bool autoRemoveFromHOTween)
    {
        if (Destroyed) return;
        
        if (_items != null)
        {
            var count = _items.Count;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                    hotSeqItem.TwMember.Kill(false);
            }

            _items = null;
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
        if (!Enabled || _items == null) return false;
        
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.TwMember.IsTweening(target))
                return true;
        }

        return false;
    }

    public override bool IsTweening(string id)
    {
        if (!Enabled || _items == null) return false;
        if (!IsPaused && base.Id == id) return true;
        
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.TwMember.IsTweening(id))
                return true;
        }

        return false;
    }

    public override bool IsTweening(int id)
    {
        if (!Enabled || _items == null) return false;
        if (!IsPaused && IntId == id) return true;
        
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.TwMember.IsTweening(id))
                return true;
        }

        return false;
    }

    public override bool IsLinkedTo(object target)
    {
        if (_items == null) return false;
        
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.TwMember.IsLinkedTo(target))
                return true;
        }

        return false;
    }

    public override List<object> GetTweenTargets()
    {
        if (_items == null)
            return null;
        var objectList = new List<object>();
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                objectList.AddRange(hotSeqItem.TwMember.GetTweenTargets());
        }

        return objectList;
    }

    public List<Tweener> GetTweenersByTarget(object target)
    {
        var tweenerList = new List<Tweener>();
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
            {
                if (hotSeqItem.TwMember is Tweener twMember3)
                {
                    if (twMember3.Target == target)
                        tweenerList.Add(twMember3);
                }
                else
                    tweenerList.AddRange(((Sequence)hotSeqItem.TwMember).GetTweenersByTarget(target));
            }
        }

        return tweenerList;
    }

    internal override List<IHOTweenComponent> GetTweensById(string id)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (base.Id == id)
            hoTweenComponentList.Add(this);
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hoTweenComponentList.AddRange(hotSeqItem.TwMember.GetTweensById(id));
        }

        return hoTweenComponentList;
    }

    internal override List<IHOTweenComponent> GetTweensByIntId(int intId)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (base.IntId == intId)
            hoTweenComponentList.Add(this);
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hoTweenComponentList.AddRange(hotSeqItem.TwMember.GetTweensByIntId(intId));
        }

        return hoTweenComponentList;
    }

    internal void Remove(ABSTweenComponent tween)
    {
        if (_items == null) return;
        
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.TwMember == tween)
            {
                _items.RemoveAt(index);
                break;
            }
        }

        if (_items.Count != 0) return;
        
        if (IsSequenced)
            ContSequence.Remove(this);
        Kill(!IsSequenced);
    }

    internal override void Complete(bool autoRemoveFromHOTween)
    {
        if (!Enabled || _items == null || LoopsVal < 0) return;
        
        FullElapsed = FullDuration;
        Update(0.0f, true);
        
        if (!AutoKillOnComplete) return;
        
        Kill(autoRemoveFromHOTween);
    }

    internal override bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration, bool ignoreCallbacks)
    {
        if (Destroyed || _items == null) return true;
        if (!Enabled) return false;
        if (base.IsComplete && !IsReversed && !forceUpdate) return true;
        if (FullElapsed == 0.0 && IsReversed && !forceUpdate || IsPaused && !forceUpdate) return false;
        
        base.IgnoreCallbacksVal = isStartupIteration || ignoreCallbacks;
        
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
        var isComplete = base.IsComplete;
        var flag1 = !IsReversed && !isComplete && Elapsed >= (double)Duration;
        SetLoops();
        SetElapsed();
        base.IsComplete = !IsReversed && LoopsVal >= 0 && CompletedLoops >= LoopsVal;
        var flag2 = !isComplete && base.IsComplete;
        if (LoopType == LoopType.Incremental)
        {
            if (_prevIncrementalCompletedLoops != CompletedLoops)
            {
                var completedLoops = base.CompletedLoops;
                if (LoopsVal != -1 && completedLoops >= LoopsVal)
                    --completedLoops;
                var diffIncr = completedLoops - _prevIncrementalCompletedLoops;
                if (diffIncr != 0)
                {
                    SetIncremental(diffIncr);
                    _prevIncrementalCompletedLoops = completedLoops;
                }
            }
        }
        else if (_prevIncrementalCompletedLoops != 0)
        {
            SetIncremental(-_prevIncrementalCompletedLoops);
            _prevIncrementalCompletedLoops = 0;
        }

        var count = _items.Count;
        if (_hasCallbacks && !IsPaused)
        {
            List<HOTSeqItem> hotSeqItemList = null;
            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.SeqItemType == SeqItemType.Callback)
                {
                    var flag3 = PrevCompletedLoops != CompletedLoops;
                    var flag4 = (LoopType == LoopType.Yoyo || LoopType == LoopType.YoyoInverse) &&
                                (IsLoopingBack && !flag3 || flag3 && !IsLoopingBack);
                    var num1 = IsLoopingBack ? Duration - Elapsed : Elapsed;
                    var num2 = IsLoopingBack ? Duration - _prevElapsed : _prevElapsed;
                    if (!IsLoopingBack
                        ? !flag4 &&
                          (hotSeqItem.StartTime <= (double)num1 || CompletedLoops != PrevCompletedLoops) &&
                          hotSeqItem.StartTime >= (double)num2 ||
                          hotSeqItem.StartTime <= (double)num1 &&
                          (!base.IsComplete && CompletedLoops != PrevCompletedLoops ||
                           hotSeqItem.StartTime >= (double)num2)
                        : flag4 &&
                        (hotSeqItem.StartTime >= (double)num1 || CompletedLoops != PrevCompletedLoops) &&
                        hotSeqItem.StartTime <= (double)num2 || hotSeqItem.StartTime >= (double)num1 &&
                        (!base.IsComplete && CompletedLoops != PrevCompletedLoops ||
                         hotSeqItem.StartTime <= (double)num2))
                    {
                        if (hotSeqItemList == null)
                            hotSeqItemList = new List<HOTSeqItem>();
                        if (hotSeqItem.StartTime > (double)num1)
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
                    if (hotSeqItem.Callback != null)
                        hotSeqItem.Callback();
                    else if (hotSeqItem.CallbackWParms != null)
                        hotSeqItem.CallbackWParms(new TweenEvent(this, hotSeqItem.CallbackParms));
                }
            }
        }

        if (Duration > 0.0)
        {
            var num = !IsLoopingBack ? Elapsed : Duration - Elapsed;
            for (var index = count - 1; index > -1; --index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.StartTime > (double)num)
                {
                    if (hotSeqItem.TwMember.Duration > 0.0)
                        hotSeqItem.TwMember.GoTo(num - hotSeqItem.StartTime, forceUpdate, true);
                    else
                        hotSeqItem.TwMember.Rewind();
                }
            }

            for (var index = 0; index < count; ++index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.SeqItemType == SeqItemType.Tween && hotSeqItem.StartTime <= (double)num)
                {
                    if (hotSeqItem.TwMember.Duration > 0.0)
                        hotSeqItem.TwMember.GoTo(num - hotSeqItem.StartTime, forceUpdate);
                    else
                        hotSeqItem.TwMember.Complete();
                }
            }
        }
        else
        {
            for (var index = count - 1; index > -1; --index)
            {
                var hotSeqItem = _items[index];
                if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                    hotSeqItem.TwMember.Complete();
            }

            if (!isComplete)
                flag2 = true;
        }

        if (FullElapsed != (double)PrevFullElapsed)
        {
            OnUpdate();
            if (FullElapsed == 0.0)
            {
                if (!IsPaused)
                {
                    IsPaused = true;
                    OnPause();
                }

                OnRewinded();
            }
        }

        if (flag2)
        {
            if (!IsPaused)
            {
                IsPaused = true;
                OnPause();
            }

            OnComplete();
        }
        else if (flag1)
            OnStepComplete();

        base.IgnoreCallbacksVal = false;
        _prevElapsed = Elapsed;
        PrevFullElapsed = FullElapsed;
        PrevCompletedLoops = CompletedLoops;
        return flag2;
    }

    internal override void SetIncremental(int diffIncr)
    {
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hotSeqItem.TwMember.SetIncremental(diffIncr);
        }
    }

    protected override bool GoTo(float time, bool play, bool forceUpdate, bool ignoreCallbacks)
    {
        if (!Enabled) return false;
        
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
        if (!Enabled || _items == null) return;
        
        Startup();
        if (!HasStarted)
            OnStart();
        IsComplete = false;
        IsLoopingBack = false;
        CompletedLoops = 0;
        FullElapsed = Elapsed = 0.0f;
        for (var index = _items.Count - 1; index > -1; --index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hotSeqItem.TwMember.Rewind();
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
        var flag = !SteadyIgnoreCallbacks;
        if (flag)
            SteadyIgnoreCallbacks = true;
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hotSeqItem.TwMember.Update(hotSeqItem.TwMember.Duration, true, true);
        }

        for (var index = count - 1; index > -1; --index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.SeqItemType == SeqItemType.Tween)
                hotSeqItem.TwMember.Rewind();
        }

        if (!flag) return;
        
        SteadyIgnoreCallbacks = false;
    }

    private static void CheckSpeedBasedTween(IHOTweenComponent twMember)
    {
        if (!(twMember is Tweener tweener) || !tweener.SpeedBased) return;
        
        tweener.ForceSetSpeedBasedDuration();
    }

    protected override void Startup()
    {
        if (StartupDone) return;
        
        TweenStartupIteration();
        base.Startup();
    }

    internal override void FillPluginsList(List<ABSTweenPlugin> plugs)
    {
        if (_items == null)
            return;
        var count = _items.Count;
        for (var index = 0; index < count; ++index)
        {
            var hotSeqItem = _items[index];
            if (hotSeqItem.TwMember != null)
            {
                if (hotSeqItem.TwMember is Sequence twMember3)
                    twMember3.FillPluginsList(plugs);
                else
                    hotSeqItem.TwMember.FillPluginsList(plugs);
            }
        }
    }

    private enum SeqItemType
    {
        Interval,
        Tween,
        Callback,
    }

    // ReSharper disable once InconsistentNaming
    private class HOTSeqItem
    {
        public readonly SeqItemType SeqItemType;
        public readonly TweenCallback Callback;
        public readonly TweenCallbackWParms CallbackWParms;
        public readonly object[] CallbackParms;
        public float StartTime;
        public readonly ABSTweenComponent TwMember;
        
        private readonly float _duration;

        public float Duration => TwMember?.Duration ?? _duration;

        public HOTSeqItem(float startTime, ABSTweenComponent twMember)
        {
            StartTime = startTime;
            TwMember = twMember;
            TwMember.AutoKillOnComplete = false;
            SeqItemType = SeqItemType.Tween;
        }

        public HOTSeqItem(float startTime, float pDuration)
        {
            SeqItemType = SeqItemType.Interval;
            StartTime = startTime;
            _duration = pDuration;
        }

        public HOTSeqItem(float startTime, TweenCallback callback, TweenCallbackWParms callbackWParms, params object[] callbackParms)
        {
            SeqItemType = SeqItemType.Callback;
            StartTime = startTime;
            Callback = callback;
            CallbackWParms = callbackWParms;
            CallbackParms = callbackParms;
        }
    }
}

}