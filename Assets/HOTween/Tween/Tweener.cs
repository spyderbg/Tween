using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System.Collections.Generic;
using UnityEngine;

using TweenCallback = Holoville.HOTween.Core.TweenDelegate.TweenCallback;
using TweenCallbackWParms = Holoville.HOTween.Core.TweenDelegate.TweenCallbackWParms;

namespace Holoville.HOTween {

public class Tweener : ABSTweenComponent, ITweener
{
    internal EaseType EaseTypeVal = HOTween.kDefEaseType;
    internal AnimationCurve EaseAnimationCurveVal;
    internal float DelayCountVal;
    internal TweenCallback OnPluginOverwritten;
    internal TweenCallbackWParms OnPluginOverwrittenWParms;
    internal object[] OnPluginOverwrittenParms;
    internal List<ABSTweenPlugin> Plugins;
    private bool _isPartial;
    private EaseType _originalEaseType;
    private PlugVector3Path _pv3Path;

    /// <summary>Indicates whether this is a FROM or a TO tween.</summary>
    public bool IsFrom { get; internal set; }

    public EaseType EaseType
    {
        get => EaseTypeVal;
        set
        {
            EaseTypeVal = value;
            var count = Plugins.Count;
            for (var index = 0; index < count; ++index)
                Plugins[index].SetEase(EaseTypeVal);
        }
    }

    public AnimationCurve EaseAnimationCurve
    {
        get => EaseAnimationCurveVal;
        set
        {
            EaseAnimationCurveVal = value;
            EaseTypeVal = EaseType.AnimationCurve;
            var count = Plugins.Count;
            for (var index = 0; index < count; ++index)
                Plugins[index].SetEase(EaseTypeVal);
        }
    }

    public float EaseOvershootOrAmplitude { get; set; } = HOTween.kDefEaseOvershootOrAmplitude;

    public float EasePeriod { get; set; } = HOTween.kDefEasePeriod;

    public object Target { get; }

    public bool PixelPerfect { get; internal set; }

    public bool SpeedBased { get; internal set; }

    public float Delay { get; internal set; }

    public float ElapsedDelay { get; private set; }

    internal Tweener(object target, float duration, TweenParms parms)
    {
        Target = target;
        Duration = duration;
        parms.InitializeObject(this, Target);
        if (Plugins != null && Plugins.Count > 0)
            IsEmpty = false;
        SetFullDuration();
    }

    internal override void Kill(bool autoRemoveFromHOTween)
    {
        if (Destroyed)
            return;
        if (HOTween.OverwriteManager != null)
            HOTween.OverwriteManager.RemoveTween(this);
        Plugins = null;
        base.Kill(autoRemoveFromHOTween);
    }

    public void Play(bool skipDelay)
    {
        if (!Enabled)
            return;
        if (skipDelay)
            SkipDelay();
        Play();
    }

    public void PlayForward(bool skipDelay)
    {
        if (!Enabled)
            return;
        if (skipDelay)
            SkipDelay();
        PlayForward();
    }

    public override void Rewind() =>
        Rewind(false);

    public void Rewind(bool skipDelay) =>
        Rewind(false, skipDelay);

    public override void Restart() =>
        Restart(false);

    public void Restart(bool skipDelay)
    {
        if (FullElapsed == 0.0)
            PlayForward(skipDelay);
        else
            Rewind(true, skipDelay);
    }

    internal override void Complete(bool autoRemoveFromHOTween)
    {
        if (!Enabled || LoopsVal < 0)
            return;
        FullElapsed = float.IsPositiveInfinity(FullDuration) ? Duration : FullDuration;
        Update(0.0f, true);
        if (!AutoKillOnComplete)
            return;
        Kill(autoRemoveFromHOTween);
    }

    public void ResetAndChangeParms(TweenType tweenType, float newDuration, TweenParms newParms)
    {
        if (Destroyed)
        {
            TweenWarning.Log(
                "ResetAndChangeParms can't run because the tween was destroyed - set AutoKill or autoKillOnComplete to FALSE if you want to avoid destroying a tween after completion");
        }
        else
        {
            Reset();
            Duration = newDuration;
            if (tweenType == TweenType.From)
                newParms = newParms.IsFrom();
            newParms.InitializeObject(this, Target);
            if (Plugins != null && Plugins.Count > 0)
                IsEmpty = false;
            SetFullDuration();
        }
    }

    protected override void Reset()
    {
        base.Reset();
        if (HOTween.OverwriteManager != null)
            HOTween.OverwriteManager.RemoveTween(this);
        IsFrom = false;
        Plugins = null;
        _isPartial = false;
        _pv3Path = null;
        Delay = ElapsedDelay = DelayCountVal = 0.0f;
        PixelPerfect = false;
        SpeedBased = false;
        EaseTypeVal = HOTween.kDefEaseType;
        EaseOvershootOrAmplitude = HOTween.kDefEaseOvershootOrAmplitude;
        EasePeriod = HOTween.kDefEasePeriod;
        _originalEaseType = HOTween.kDefEaseType;
        OnPluginOverwritten = null;
        onStepCompleteWParms = null;
        OnPluginOverwrittenParms = null;
    }

    protected override void ApplyCallback(bool wParms, CallbackType callbackType, TweenCallback callback, TweenCallbackWParms callbackWParms, params object[] callbackParms)
    {
        if (callbackType == CallbackType.OnPluginOverwritten)
        {
            OnPluginOverwritten = callback;
            OnPluginOverwrittenWParms = callbackWParms;
            OnPluginOverwrittenParms = callbackParms;
        }
        else
            base.ApplyCallback(wParms, callbackType, callback, callbackWParms, callbackParms);
    }

    public override bool IsTweening(object target) =>
        Enabled && target == Target && (!IsSequenced ? !IsPaused : !ContSequence.IsPaused);

    public override bool IsTweening(string id) =>
        Enabled && Id == id && (!IsSequenced ? !IsPaused : !ContSequence.IsPaused);

    public override bool IsTweening(int id) =>
        Enabled && IntId == id && (!IsSequenced ? !IsPaused : !ContSequence.IsPaused);

    public override bool IsLinkedTo(object target) =>
        target == Target;

    public override List<object> GetTweenTargets() => 
        new List<object>() { Target };

    internal override List<IHOTweenComponent> GetTweensById(string id)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (Id == id)
            hoTweenComponentList.Add(this);
        return hoTweenComponentList;
    }

    internal override List<IHOTweenComponent> GetTweensByIntId(int intId)
    {
        var hoTweenComponentList = new List<IHOTweenComponent>();
        if (IntId == intId)
            hoTweenComponentList.Add(this);
        return hoTweenComponentList;
    }

    public Vector3 GetPointOnPath(float t)
    {
        var vector3PathPlugin = GetPlugVector3PathPlugin();
        if (vector3PathPlugin == null) return Vector3.zero;
        
        Startup();
        return vector3PathPlugin.GetConstPointOnPath(t);
    }

    public Tweener UsePartialPath(int waypointId0, int waypointId1)
    {
        var newEaseType = !_isPartial ? EaseTypeVal : _originalEaseType;
        return UsePartialPath(waypointId0, waypointId1, -1f, newEaseType);
    }

    public Tweener UsePartialPath(int waypointId0, int waypointId1, float newDuration)
    {
        var newEaseType = _isPartial ? _originalEaseType : EaseTypeVal;
        return UsePartialPath(waypointId0, waypointId1, newDuration, newEaseType);
    }

    public Tweener UsePartialPath(int waypointId0, int waypointId1, EaseType newEaseType)
    {
        return UsePartialPath(waypointId0, waypointId1, -1f, newEaseType);
    }

    public Tweener UsePartialPath(int waypointId0, int waypointId1, float newDuration, EaseType newEaseType)
    {
        if (Plugins.Count > 1)
        {
            TweenWarning.Log("Applying a partial path on a Tweener (" + Target +
                             ") with more than one plugin/property being tweened is not allowed");
            return this;
        }

        if (_pv3Path == null)
        {
            _pv3Path = GetPlugVector3PathPlugin();
            if (_pv3Path == null)
            {
                TweenWarning.Log("Tweener for " + Target + " contains no PlugVector3Path plugin");
                return this;
            }
        }

        Startup();
        if (!_isPartial)
        {
            _isPartial = true;
            OriginalDuration = Duration;
            _originalEaseType = EaseTypeVal;
        }

        var pathId1 = ConvertWaypointIdToPathId(_pv3Path, waypointId0, true);
        var pathId2 = ConvertWaypointIdToPathId(_pv3Path, waypointId1, false);
        var lengthPercentage = _pv3Path.GetWaypointsLengthPercentage(pathId1, pathId2);
        var partialStartPerc = pathId1 == 0 ? 0.0f : _pv3Path.GetWaypointsLengthPercentage(0, pathId1);
        Duration = newDuration >= 0.0 ? newDuration : OriginalDuration * lengthPercentage;
        EaseTypeVal = newEaseType;
        _pv3Path.SwitchToPartialPath(Duration, newEaseType, partialStartPerc, lengthPercentage);
        Startup(true);
        if (!IsPaused)
            Restart(true);
        else
            Rewind(true, true);
        return this;
    }

    public void ResetPath()
    {
        _isPartial = false;
        Duration = SpeedBased ? OriginalNonSpeedBasedDuration : OriginalDuration;
        EaseTypeVal = _originalEaseType;
        _pv3Path.ResetToFullPath(Duration, EaseTypeVal);
        Startup(true);
        if (!IsPaused)
            Restart(true);
        else
            Rewind(true);
    }

    private PlugVector3Path GetPlugVector3PathPlugin()
    {
        if (Plugins == null) return null;
        
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
        {
            if (Plugins[index] is PlugVector3Path plugin1)
                return plugin1;
        }

        return null;
    }

    internal override bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration, bool ignoreCallbacks)
    {
        return Update(shortElapsed, forceUpdate, isStartupIteration, ignoreCallbacks, false);
    }

    internal bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration, bool ignoreCallbacks, bool ignoreDelay = false)
    {
        if (Destroyed) return true;
        
        if (Target == null || Target.Equals(null))
        {
            Kill(false);
            return true;
        }

        if (!Enabled)
            return false;
        if (IsComplete && !IsReversed && !forceUpdate)
            return true;
        if (FullElapsed == 0.0 && IsReversed && !forceUpdate || IsPaused && !forceUpdate)
            return false;
        this.IgnoreCallbacksVal = isStartupIteration || ignoreCallbacks;
        if (ignoreDelay || DelayCountVal == 0.0)
        {
            Startup();
            if (!HasStarted)
                OnStart();
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
        }
        else
        {
            if (TimeScale != 0.0)
                ElapsedDelay += shortElapsed / TimeScale;
            if (ElapsedDelay < (double)DelayCountVal)
                return false;
            if (IsReversed)
            {
                FullElapsed = Elapsed = 0.0f;
            }
            else
            {
                FullElapsed = Elapsed = ElapsedDelay - DelayCountVal;
                if (FullElapsed > (double)FullDuration)
                    FullElapsed = FullDuration;
            }

            ElapsedDelay = DelayCountVal;
            DelayCountVal = 0.0f;
            Startup();
            if (!HasStarted)
                OnStart();
        }

        var isComplete = IsComplete;
        var flag1 = !IsReversed && !isComplete && Elapsed >= (double)Duration;
        SetLoops();
        SetElapsed();
        IsComplete = !IsReversed && LoopsVal >= 0 && CompletedLoops >= LoopsVal;
        var flag2 = !isComplete && IsComplete;
        var totElapsed = !IsLoopingBack ? Elapsed : Duration - Elapsed;
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
        {
            var plugin = Plugins[index];
            if (!IsLoopingBack && plugin.easeReversed ||
                IsLoopingBack && LoopType == LoopType.YoyoInverse && !plugin.easeReversed)
                plugin.ReverseEase();
            if (Duration > 0.0)
            {
                plugin.Update(totElapsed);
                OnPluginUpdated(plugin);
            }
            else
            {
                OnPluginUpdated(plugin);
                plugin.Complete();
                if (!isComplete)
                    flag2 = true;
            }
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

        this.IgnoreCallbacksVal = false;
        PrevFullElapsed = FullElapsed;
        return flag2;
    }

    internal override void SetIncremental(int diffIncr)
    {
        if (Plugins == null) return;
        
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
            Plugins[index].ForceSetIncremental(diffIncr);
    }

    internal void ForceSetSpeedBasedDuration()
    {
        if (!SpeedBased || Plugins == null) return;
        
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
            Plugins[index].ForceSetSpeedBasedDuration();
        Duration = 0.0f;
        for (var index = 0; index < count; ++index)
        {
            var plugin = Plugins[index];
            if (plugin.duration > (double)Duration)
                Duration = plugin.duration;
        }

        SetFullDuration();
    }

    protected override bool GoTo(float time, bool play, bool forceUpdate, bool ignoreCallbacks)
    {
        if (!Enabled) return false;
        
        if (time > (double)FullDuration)
            time = FullDuration;
        else if (time < 0.0)
            time = 0.0f;
        if (!forceUpdate && FullElapsed == (double)time)
        {
            if (!IsComplete && play)
                Play();
            return IsComplete;
        }

        FullElapsed = time;
        DelayCountVal = 0.0f;
        ElapsedDelay = Delay;
        Update(0.0f, true, false, ignoreCallbacks, false);
        if (!IsComplete && play)
            Play();
        return IsComplete;
    }

    private void Rewind(bool play, bool skipDelay)
    {
        if (!Enabled) return;
        
        Startup();
        if (!HasStarted)
            OnStart();
        IsComplete = false;
        IsLoopingBack = false;
        DelayCountVal = skipDelay ? 0.0f : Delay;
        ElapsedDelay = skipDelay ? Delay : 0.0f;
        CompletedLoops = 0;
        FullElapsed = Elapsed = 0.0f;
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
        {
            var plugin = Plugins[index];
            if (plugin.easeReversed)
                plugin.ReverseEase();
            plugin.Rewind();
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

    private void SkipDelay()
    {
        if (DelayCountVal <= 0.0) return;
        
        DelayCountVal = 0.0f;
        ElapsedDelay = Delay;
        Elapsed = FullElapsed = 0.0f;
    }

    protected override void Startup() =>
        Startup(false);

    private void Startup(bool force)
    {
        if (!force && StartupDone) return;
        
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
        {
            var plugin = Plugins[index];
            if (!plugin.WasStarted)
                plugin.Startup();
        }

        if (SpeedBased)
        {
            OriginalNonSpeedBasedDuration = Duration;
            Duration = 0.0f;
            for (var index = 0; index < count; ++index)
            {
                var plugin = Plugins[index];
                if (plugin.duration > (double)Duration)
                    Duration = plugin.duration;
            }

            SetFullDuration();
        }
        else if (force)
            SetFullDuration();

        base.Startup();
    }

    protected override void OnStart()
    {
        if (SteadyIgnoreCallbacks || IgnoreCallbacksVal) return;
        
        if (HOTween.OverwriteManager != null)
            HOTween.OverwriteManager.AddTween(this);
        base.OnStart();
    }

    protected override void OnPlay()
    {
        if (Delay <= 0.0 || ElapsedDelay > 0.0) return;
        
        Update(0.0f, true, true, false, true);
    }

    internal override void FillPluginsList(List<ABSTweenPlugin> plugs)
    {
        if (Plugins == null) return;
        
        var count = Plugins.Count;
        for (var index = 0; index < count; ++index)
            plugs.Add(Plugins[index]);
    }

    private static int ConvertWaypointIdToPathId(PlugVector3Path plugVector3Path, int waypointId, bool isStartingWp)
    {
        return waypointId == -1
            ? (!isStartingWp ? plugVector3Path.Path.path.Length - 2 : 1)
            : (plugVector3Path.HasAdditionalStartingP ? waypointId + 2 : waypointId + 1);
    }
}

}