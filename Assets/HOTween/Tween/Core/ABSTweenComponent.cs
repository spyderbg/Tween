using Holoville.HOTween.Plugins.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOTween.Core {

public abstract class ABSTweenComponent : IHOTweenComponent
{
    internal string _id = "";
    internal int _intId = -1;
    internal bool _autoKillOnComplete = true;
    internal bool _enabled = true;
    internal float _timeScale = HOTween.kDefTimeScale;
    internal int _loops = 1;
    internal LoopType _loopType = HOTween.kDefLoopType;
    internal UpdateType _updateType = HOTween.kDefUpdateType;
    internal bool _isPaused;

    internal bool ignoreCallbacks;

    internal bool _steadyIgnoreCallbacks;

    internal Sequence contSequence;
    internal bool startupDone;
    internal TweenDelegate.TweenCallback onStart;
    internal TweenDelegate.TweenCallbackWParms onStartWParms;
    internal object[] onStartParms;
    internal TweenDelegate.TweenCallback onUpdate;
    internal TweenDelegate.TweenCallbackWParms onUpdateWParms;
    internal object[] onUpdateParms;
    internal TweenDelegate.TweenCallback onPluginUpdated;
    internal TweenDelegate.TweenCallbackWParms onPluginUpdatedWParms;
    internal object[] onPluginUpdatedParms;
    internal TweenDelegate.TweenCallback onPause;
    internal TweenDelegate.TweenCallbackWParms onPauseWParms;
    internal object[] onPauseParms;
    internal TweenDelegate.TweenCallback onPlay;
    internal TweenDelegate.TweenCallbackWParms onPlayWParms;
    internal object[] onPlayParms;
    internal TweenDelegate.TweenCallback onRewinded;
    internal TweenDelegate.TweenCallbackWParms onRewindedWParms;
    internal object[] onRewindedParms;
    internal TweenDelegate.TweenCallback onStepComplete;
    internal TweenDelegate.TweenCallbackWParms onStepCompleteWParms;
    internal object[] onStepCompleteParms;
    internal TweenDelegate.TweenCallback onComplete;
    internal TweenDelegate.TweenCallbackWParms onCompleteWParms;
    internal object[] onCompleteParms;

    protected int CompletedLoops;

    public float Duration { get; protected set; }

    public float FullDuration { get; protected set; }
    
    protected float OriginalDuration;

    protected float OriginalNonSpeedBasedDuration;

    protected float Elapsed;

    protected float FullElapsed;

    protected bool Destroyed;

    protected bool IsEmpty = true;

    protected bool IsReversed;

    protected bool IsLoopingBack;

    protected bool HasStarted;

    protected bool IsComplete;

    protected float PrevFullElapsed;

    protected int PrevCompletedLoops;

    internal bool ManageBehaviours;

    internal bool ManageGameObjects;

    internal Behaviour[] ManagedBehavioursOn;

    internal Behaviour[] ManagedBehavioursOff;

    internal GameObject[] ManagedGameObjectsOn;

    internal GameObject[] ManagedGameObjectsOff;

    internal bool[] ManagedBehavioursOriginalState;

    internal bool[] managedGameObjectsOriginalState;

    internal virtual bool steadyIgnoreCallbacks
    {
        get => _steadyIgnoreCallbacks;
        set => _steadyIgnoreCallbacks = value;
    }

    public string id
    {
        get => _id;
        set => _id = value;
    }

    public int intId
    {
        get => _intId;
        set => _intId = value;
    }

    public bool autoKillOnComplete
    {
        get => _autoKillOnComplete;
        set => _autoKillOnComplete = value;
    }

    public bool enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    public float timeScale
    {
        get => _timeScale;
        set => _timeScale = value;
    }

    public int loops
    {
        get => _loops;
        set
        {
            _loops = value;
            SetFullDuration();
        }
    }

    public LoopType loopType
    {
        get => _loopType;
        set => _loopType = value;
    }

    public float position
    {
        get => _loops >= 1 ? FullElapsed : Elapsed;
        set => GoTo(value, !_isPaused);
    }

    public float elapsed => Elapsed;

    public float fullElapsed => FullElapsed;

    public UpdateType updateType => _updateType;

    public int completedLoops => CompletedLoops;

    public bool destroyed => Destroyed;

    public bool isEmpty => IsEmpty;

    public bool isReversed => IsReversed;

    public bool isLoopingBack => IsLoopingBack;

    public bool isPaused => _isPaused;

    public bool hasStarted => HasStarted;

    public bool isComplete => IsComplete;

    public bool isSequenced => contSequence != null;

    public void Kill() => Kill(true);

    internal virtual void Kill(bool autoRemoveFromHOTween)
    {
        if (Destroyed)
            return;
        Destroyed = IsEmpty = true;
        if (!autoRemoveFromHOTween)
            return;
        HOTween.Kill(this);
    }

    public void Play()
    {
        if (!_enabled)
            return;
        PlayIfPaused();
    }

    private void PlayIfPaused()
    {
        if (!_isPaused || (IsReversed || IsComplete) && (!IsReversed || FullElapsed <= 0.0))
            return;
        _isPaused = false;
        OnPlay();
    }

    public void PlayForward()
    {
        if (!_enabled)
            return;
        IsReversed = false;
        PlayIfPaused();
    }

    public void PlayBackwards()
    {
        if (!_enabled)
            return;
        IsReversed = true;
        PlayIfPaused();
    }

    public void Pause()
    {
        if (!_enabled || _isPaused)
            return;
        _isPaused = true;
        OnPause();
    }

    public abstract void Rewind();

    public abstract void Restart();

    public void Reverse(bool forcePlay = false)
    {
        if (!_enabled)
            return;
        IsReversed = !IsReversed;
        if (!forcePlay)
            return;
        Play();
    }

    public void Complete() => Complete(true);

    public bool GoTo(float time) => GoTo(time, false, false, false);

    public bool GoTo(float time, bool forceUpdate) => GoTo(time, false, forceUpdate, false);

    internal bool GoTo(float time, bool forceUpdate, bool ignoreCallbacks) =>
        GoTo(time, false, forceUpdate, ignoreCallbacks);

    public bool GoToAndPlay(float time) =>
        GoTo(time, true, false, false);

    public bool GoToAndPlay(float time, bool forceUpdate) =>
        GoTo(time, true, forceUpdate, false);

    internal bool GoToAndPlay(float time, bool forceUpdate, bool ignoreCallbacks) =>
        GoTo(time, true, forceUpdate, ignoreCallbacks);

    public IEnumerator WaitForCompletion()
    {
        while (!IsComplete)
            yield return 0;
    }

    public IEnumerator WaitForRewind()
    {
        while (FullElapsed > 0.0)
            yield return 0;
    }

    protected virtual void Reset()
    {
        _id = "";
        _intId = -1;
        _autoKillOnComplete = true;
        _enabled = true;
        _timeScale = HOTween.kDefTimeScale;
        _loops = 1;
        _loopType = HOTween.kDefLoopType;
        _updateType = HOTween.kDefUpdateType;
        _isPaused = false;
        CompletedLoops = 0;
        Duration = OriginalDuration = OriginalNonSpeedBasedDuration = FullDuration = 0.0f;
        Elapsed = FullElapsed = 0.0f;
        IsEmpty = true;
        IsReversed = IsLoopingBack = HasStarted = IsComplete = false;
        startupDone = false;
        onStart = null;
        onStartWParms = null;
        onStartParms = null;
        onUpdate = null;
        onUpdateWParms = null;
        onUpdateParms = null;
        onPluginUpdated = null;
        onPluginUpdatedWParms = null;
        onPluginUpdatedParms = null;
        onStepComplete = null;
        onStepCompleteWParms = null;
        onStepCompleteParms = null;
        onComplete = null;
        onCompleteWParms = null;
        onCompleteParms = null;
        onPause = null;
        onPauseWParms = null;
        onPauseParms = null;
        onPlay = null;
        onPlayWParms = null;
        onPlayParms = null;
        onRewinded = null;
        onRewindedWParms = null;
        onRewindedParms = null;
        ManageBehaviours = false;
        ManagedBehavioursOff = null;
        ManagedBehavioursOn = null;
        ManagedBehavioursOriginalState = null;
        ManageGameObjects = false;
        ManagedGameObjectsOff = null;
        ManagedGameObjectsOn = null;
        managedGameObjectsOriginalState = null;
    }

    public void ApplyCallback(CallbackType callbackType, TweenDelegate.TweenCallback callback) =>
        ApplyCallback(false, callbackType, callback, null, null);

    public void ApplyCallback(
        CallbackType callbackType,
        TweenDelegate.TweenCallbackWParms callback,
        params object[] callbackParms)
    {
        ApplyCallback(true, callbackType, null, callback, callbackParms);
    }

    public void ApplyCallback(CallbackType callbackType, GameObject sendMessageTarget, string methodName,
        object value, SendMessageOptions options = SendMessageOptions.RequireReceiver)
    {
        var callbackWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
        var objArray = new object[4]
        {
            sendMessageTarget,
            methodName,
            value,
            options
        };
        ApplyCallback(true, callbackType, null, callbackWParms, objArray);
    }

    protected virtual void ApplyCallback(bool wParms, CallbackType callbackType,
        TweenDelegate.TweenCallback callback, TweenDelegate.TweenCallbackWParms callbackWParms,
        params object[] callbackParms)
    {
        switch (callbackType)
        {
            case CallbackType.OnStart:
                onStart = callback;
                onStartWParms = callbackWParms;
                onStartParms = callbackParms;
                break;
            case CallbackType.OnUpdate:
                onUpdate = callback;
                onUpdateWParms = callbackWParms;
                onUpdateParms = callbackParms;
                break;
            case CallbackType.OnStepComplete:
                onStepComplete = callback;
                onStepCompleteWParms = callbackWParms;
                onStepCompleteParms = callbackParms;
                break;
            case CallbackType.OnComplete:
                onComplete = callback;
                onCompleteWParms = callbackWParms;
                onCompleteParms = callbackParms;
                break;
            case CallbackType.OnPause:
                onPause = callback;
                onPauseWParms = callbackWParms;
                onPauseParms = callbackParms;
                break;
            case CallbackType.OnPlay:
                onPlay = callback;
                onPlayWParms = callbackWParms;
                onPlayParms = callbackParms;
                break;
            case CallbackType.OnRewinded:
                onRewinded = callback;
                onRewindedWParms = callbackWParms;
                onRewindedParms = callbackParms;
                break;
            case CallbackType.OnPluginOverwritten:
                TweenWarning.Log(
                    "ApplyCallback > OnPluginOverwritten type is available only with Tweeners and not with Sequences");
                break;
        }
    }

    public abstract bool IsTweening(object target);

    public abstract bool IsTweening(string id);

    public abstract bool IsTweening(int id);

    public abstract bool IsLinkedTo(object target);

    public abstract List<object> GetTweenTargets();

    internal abstract List<IHOTweenComponent> GetTweensById(string id);

    internal abstract List<IHOTweenComponent> GetTweensByIntId(int intId);

    internal abstract void Complete(bool autoRemoveFromHOTween);

    internal bool Update(float shortElapsed) =>
        Update(shortElapsed, false, false, false);

    internal bool Update(float shortElapsed, bool forceUpdate) =>
        Update(shortElapsed, forceUpdate, false, false);

    internal bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration) =>
        Update(shortElapsed, forceUpdate, isStartupIteration, false);

    internal abstract bool Update(float shortElapsed, bool forceUpdate, bool isStartupIteration, bool ignoreCallbacks);

    internal abstract void SetIncremental(int diffIncr);

    protected abstract bool GoTo(float time, bool play, bool forceUpdate, bool ignoreCallbacks);

    protected virtual void Startup() => startupDone = true;

    protected virtual void OnStart()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        HasStarted = true;
        if (onStart != null)
            onStart();
        else if (onStartWParms != null)
            onStartWParms(new TweenEvent(this, onStartParms));
        OnPlay();
    }

    protected void OnUpdate()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        if (onUpdate != null)
        {
            onUpdate();
        }
        else
        {
            if (onUpdateWParms == null)
                return;
            onUpdateWParms(new TweenEvent(this, onUpdateParms));
        }
    }

    protected void OnPluginUpdated(ABSTweenPlugin plugin)
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        if (onPluginUpdated != null)
        {
            onPluginUpdated();
        }
        else
        {
            if (onPluginUpdatedWParms == null)
                return;
            onPluginUpdatedWParms(new TweenEvent(this, onPluginUpdatedParms, plugin));
        }
    }

    protected void OnPause()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        ManageObjects(false);
        if (onPause != null)
        {
            onPause();
        }
        else
        {
            if (onPauseWParms == null)
                return;
            onPauseWParms(new TweenEvent(this, onPauseParms));
        }
    }

    protected virtual void OnPlay()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        ManageObjects(true);
        if (onPlay != null)
        {
            onPlay();
        }
        else
        {
            if (onPlayWParms == null)
                return;
            onPlayWParms(new TweenEvent(this, onPlayParms));
        }
    }

    protected void OnRewinded()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        if (onRewinded != null)
        {
            onRewinded();
        }
        else
        {
            if (onRewindedWParms == null)
                return;
            onRewindedWParms(new TweenEvent(this, onRewindedParms));
        }
    }

    protected void OnStepComplete()
    {
        if (steadyIgnoreCallbacks || ignoreCallbacks)
            return;
        if (onStepComplete != null)
        {
            onStepComplete();
        }
        else
        {
            if (onStepCompleteWParms == null)
                return;
            onStepCompleteWParms(new TweenEvent(this, onStepCompleteParms));
        }
    }

    protected void OnComplete()
    {
        IsComplete = true;
        OnStepComplete();
        if (steadyIgnoreCallbacks || ignoreCallbacks || onComplete == null && onCompleteWParms == null)
            return;
        if (HOTween.isUpdateLoop)
            HOTween.OnCompletes.Add(this);
        else
            OnCompleteDispatch();
    }

    internal void OnCompleteDispatch()
    {
        if (onComplete != null)
        {
            onComplete();
        }
        else
        {
            if (onCompleteWParms == null)
                return;
            onCompleteWParms(new TweenEvent(this, onCompleteParms));
        }
    }

    protected void SetFullDuration() =>
        FullDuration = _loops < 0 ? float.PositiveInfinity : Duration * _loops;

    protected void SetElapsed()
    {
        if (Duration == 0.0 || _loops >= 0 && CompletedLoops >= _loops)
            Elapsed = Duration;
        else if (FullElapsed < (double)Duration)
            Elapsed = FullElapsed;
        else
            Elapsed = FullElapsed % Duration;
    }

    protected void SetLoops()
    {
        if (Duration == 0.0)
        {
            CompletedLoops = 1;
        }
        else
        {
            var num1 = FullElapsed / Duration;
            var num2 = (int)Math.Ceiling(num1);
            CompletedLoops = num2 - (double)num1 >= 1.0000000116861E-07 ? num2 - 1 : num2;
        }

        IsLoopingBack = _loopType != LoopType.Restart && _loopType != LoopType.Incremental &&
                        (_loops > 0 && (CompletedLoops < _loops && CompletedLoops % 2 != 0 ||
                                        CompletedLoops >= _loops && CompletedLoops % 2 == 0) ||
                         _loops < 0 && CompletedLoops % 2 != 0);
    }

    protected void ManageObjects(bool isPlay)
    {
        if (ManageBehaviours)
        {
            var num = 0;
            if (ManagedBehavioursOn != null)
            {
                num = ManagedBehavioursOn.Length;
                for (var index = 0; index < ManagedBehavioursOn.Length; ++index)
                {
                    var behaviour = ManagedBehavioursOn[index];
                    if (!(behaviour == null))
                    {
                        if (isPlay)
                        {
                            ManagedBehavioursOriginalState[index] = behaviour.enabled;
                            behaviour.enabled = true;
                        }
                        else
                            behaviour.enabled = ManagedBehavioursOriginalState[index];
                    }
                }
            }

            if (ManagedBehavioursOff != null)
            {
                for (var index = 0; index < ManagedBehavioursOff.Length; ++index)
                {
                    var behaviour = ManagedBehavioursOff[index];
                    if (!(behaviour == null))
                    {
                        if (isPlay)
                        {
                            ManagedBehavioursOriginalState[num + index] = behaviour.enabled;
                            behaviour.enabled = false;
                        }
                        else
                            behaviour.enabled = ManagedBehavioursOriginalState[index + num];
                    }
                }
            }
        }

        if (!ManageGameObjects) return;

        var num1 = 0;
        if (ManagedGameObjectsOn != null)
        {
            num1 = ManagedGameObjectsOn.Length;
            for (var index = 0; index < ManagedGameObjectsOn.Length; ++index)
            {
                var gameObject = ManagedGameObjectsOn[index];
                if (!(gameObject == null))
                {
                    if (isPlay)
                    {
                        managedGameObjectsOriginalState[index] = gameObject.active;
                        gameObject.active = true;
                    }
                    else
                        gameObject.active = managedGameObjectsOriginalState[index];
                }
            }
        }

        if (ManagedGameObjectsOff == null) return;

        for (var index = 0; index < ManagedGameObjectsOff.Length; ++index)
        {
            var gameObject = ManagedGameObjectsOff[index];
            if (!(gameObject == null))
            {
                if (isPlay)
                {
                    managedGameObjectsOriginalState[num1 + index] = gameObject.active;
                    gameObject.active = false;
                }
                else
                    gameObject.active = managedGameObjectsOriginalState[index + num1];
            }
        }
    }

    internal abstract void FillPluginsList(List<ABSTweenPlugin> plugs);
}

}