﻿using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOTween
{
    /// <summary>
    /// Tween component, created by HOTween for each separate tween.
    /// <para>Author: Daniele Giardini (http://www.holoville.com)</para>
    /// </summary>
    public class Tweener : ABSTweenComponent
    {
        private float _elapsedDelay;
        internal EaseType _easeType = HOTween.defEaseType;
        internal AnimationCurve _easeAnimationCurve;
        internal float _easeOvershootOrAmplitude = HOTween.defEaseOvershootOrAmplitude;
        internal float _easePeriod = HOTween.defEasePeriod;
        internal bool _pixelPerfect;
        internal bool _speedBased;
        internal float _delay;
        internal float delayCount;
        internal TweenDelegate.TweenCallback onPluginOverwritten;
        internal TweenDelegate.TweenCallbackWParms onPluginOverwrittenWParms;
        internal object[] onPluginOverwrittenParms;
        internal List<ABSTweenPlugin> plugins;
        private object _target;
        private bool isPartialled;
        private EaseType _originalEaseType;
        private PlugVector3Path pv3Path;

        /// <summary>Indicates whether this is a FROM or a TO tween.</summary>
        public bool isFrom { get; internal set; }

        /// <summary>
        /// Ease type of this tweener
        /// (consider that the plugins you have set might have different ease types).
        /// Setting it will change the ease of all the plugins used by this tweener.
        /// </summary>
        public EaseType easeType
        {
            get => _easeType;
            set
            {
                _easeType = value;
                var count = plugins.Count;
                for (var index = 0; index < count; ++index)
                    plugins[index].SetEase(_easeType);
            }
        }

        /// <summary>
        /// Ease type of this tweener
        /// (consider that the plugins you have set might have different ease types).
        /// Setting it will change the ease of all the plugins used by this tweener.
        /// </summary>
        public AnimationCurve easeAnimationCurve
        {
            get => _easeAnimationCurve;
            set
            {
                _easeAnimationCurve = value;
                _easeType = EaseType.AnimationCurve;
                var count = plugins.Count;
                for (var index = 0; index < count; ++index)
                    plugins[index].SetEase(_easeType);
            }
        }

        /// <summary>Eventual overshoot to use with Back easeTypes.</summary>
        public float easeOvershootOrAmplitude
        {
            get => _easeOvershootOrAmplitude;
            set => _easeOvershootOrAmplitude = value;
        }

        /// <summary>Eventual period to use with Elastic easeTypes.</summary>
        public float easePeriod
        {
            get => _easePeriod;
            set => _easePeriod = value;
        }

        /// <summary>Target of this tween.</summary>
        public object target => _target;

        /// <summary>
        /// <c>true</c> if this tween is animated via integers values only.
        /// </summary>
        public bool pixelPerfect => _pixelPerfect;

        /// <summary>
        /// <c>true</c> if this tween is animated by speed instead than by duration.
        /// </summary>
        public bool speedBased => _speedBased;

        /// <summary>The delay that was set for this tween.</summary>
        public float delay => _delay;

        /// <summary>The currently elapsed delay time.</summary>
        public float elapsedDelay => _elapsedDelay;

        /// <summary>
        /// Called by HOTween each time a new tween is generated via <c>To</c> or similar methods.
        /// </summary>
        internal Tweener(object p_target, float p_duration, TweenParms p_parms)
        {
            _target = p_target;
            _duration = p_duration;
            p_parms.InitializeObject(this, _target);
            if (plugins != null && plugins.Count > 0)
                _isEmpty = false;
            SetFullDuration();
        }

        /// <summary>Kills this Tweener and cleans it.</summary>
        /// <param name="p_autoRemoveFromHOTween">
        /// If <c>true</c> also calls <c>HOTween.Kill(this)</c> to remove it from HOTween.
        /// Set internally to <c>false</c> when I already know that HOTween is going to remove it.
        /// </param>
        internal override void Kill(bool p_autoRemoveFromHOTween)
        {
            if (_destroyed)
                return;
            if (HOTween.OverwriteManager != null)
                HOTween.OverwriteManager.RemoveTween(this);
            plugins = null;
            base.Kill(p_autoRemoveFromHOTween);
        }

        /// <summary>Resumes this Tweener.</summary>
        /// <param name="p_skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        public void Play(bool p_skipDelay)
        {
            if (!_enabled)
                return;
            if (p_skipDelay)
                SkipDelay();
            Play();
        }

        /// <summary>Resumes this Tweener and plays it forward.</summary>
        /// <param name="p_skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        public void PlayForward(bool p_skipDelay)
        {
            if (!_enabled)
                return;
            if (p_skipDelay)
                SkipDelay();
            PlayForward();
        }

        /// <summary>
        /// Rewinds this Tweener (loops and tween delay included), and pauses it.
        /// </summary>
        public override void Rewind() => Rewind(false);

        /// <summary>Rewinds this Tweener (loops included), and pauses it.</summary>
        /// <param name="p_skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        public void Rewind(bool p_skipDelay) => Rewind(false, p_skipDelay);

        /// <summary>
        /// Restarts this Tweener from the beginning (loops and tween delay included).
        /// </summary>
        public override void Restart() => Restart(false);

        /// <summary>
        /// Restarts this Tweener from the beginning (loops and tween delay included).
        /// </summary>
        /// <param name="p_skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        public void Restart(bool p_skipDelay)
        {
            if (_fullElapsed == 0.0)
                PlayForward(p_skipDelay);
            else
                Rewind(true, p_skipDelay);
        }

        /// <summary>
        /// Completes this Tweener.
        /// Where a loop was involved, the Tweener completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        internal override void Complete(bool p_autoRemoveFromHOTween)
        {
            if (!_enabled || _loops < 0)
                return;
            _fullElapsed = float.IsPositiveInfinity(_fullDuration) ? _duration : _fullDuration;
            Update(0.0f, true);
            if (!_autoKillOnComplete)
                return;
            Kill(p_autoRemoveFromHOTween);
        }

        /// <summary>
        /// Completely resets the Tweener except its target,
        /// and applies a new <see cref="T:Holoville.HOTween.TweenType" />, duration, and <see cref="T:Holoville.HOTween.TweenParms" />.
        /// </summary>
        /// <param name="p_tweenType">New tween type (to/from)</param>
        /// <param name="p_newDuration">New duration</param>
        /// <param name="p_newParms">New parameters</param>
        public void ResetAndChangeParms(
            TweenType p_tweenType,
            float p_newDuration,
            TweenParms p_newParms)
        {
            if (_destroyed)
            {
                TweenWarning.Log(
                    "ResetAndChangeParms can't run because the tween was destroyed - set AutoKill or autoKillOnComplete to FALSE if you want to avoid destroying a tween after completion");
            }
            else
            {
                Reset();
                _duration = p_newDuration;
                if (p_tweenType == TweenType.From)
                    p_newParms = p_newParms.IsFrom();
                p_newParms.InitializeObject(this, _target);
                if (plugins != null && plugins.Count > 0)
                    _isEmpty = false;
                SetFullDuration();
            }
        }

        /// <summary>Completely resets this Tweener, except its target</summary>
        protected override void Reset()
        {
            base.Reset();
            if (HOTween.OverwriteManager != null)
                HOTween.OverwriteManager.RemoveTween(this);
            isFrom = false;
            plugins = null;
            isPartialled = false;
            pv3Path = null;
            _delay = _elapsedDelay = delayCount = 0.0f;
            _pixelPerfect = false;
            _speedBased = false;
            _easeType = HOTween.defEaseType;
            _easeOvershootOrAmplitude = HOTween.defEaseOvershootOrAmplitude;
            _easePeriod = HOTween.defEasePeriod;
            _originalEaseType = HOTween.defEaseType;
            onPluginOverwritten = null;
            onStepCompleteWParms = null;
            onPluginOverwrittenParms = null;
        }

        /// <summary>
        /// Assigns the given callback to this Tweener/Sequence,
        /// overwriting any existing callbacks of the same type.
        /// </summary>
        protected override void ApplyCallback(
            bool p_wParms,
            CallbackType p_callbackType,
            TweenDelegate.TweenCallback p_callback,
            TweenDelegate.TweenCallbackWParms p_callbackWParms,
            params object[] p_callbackParms)
        {
            if (p_callbackType == CallbackType.OnPluginOverwritten)
            {
                onPluginOverwritten = p_callback;
                onPluginOverwrittenWParms = p_callbackWParms;
                onPluginOverwrittenParms = p_callbackParms;
            }
            else
                base.ApplyCallback(p_wParms, p_callbackType, p_callback, p_callbackWParms, p_callbackParms);
        }

        /// <summary>
        /// Returns <c>true</c> if the given target and this Tweener target are the same, and the Tweener is running.
        /// Returns <c>false</c> both if the given target is not the same as this Tweener's, than if this Tweener is paused.
        /// This method is here to uniform <see cref="T:Holoville.HOTween.Tweener" /> with <see cref="T:Holoville.HOTween.Sequence" />.
        /// </summary>
        /// <param name="p_target">The target to check.</param>
        public override bool IsTweening(object p_target)
        {
            if (!_enabled || p_target != _target)
                return false;
            return !isSequenced ? !_isPaused : !contSequence.isPaused;
        }

        /// <summary>
        /// Returns <c>true</c> if the tween with the given string id is currently involved in a running tween or sequence.
        /// This method is here to uniform <see cref="T:Holoville.HOTween.Tweener" /> with <see cref="T:Holoville.HOTween.Sequence" />.
        /// </summary>
        /// <param name="p_id">The id to check for.</param>
        public override bool IsTweening(string p_id)
        {
            if (!_enabled || !(id == p_id))
                return false;
            return !isSequenced ? !_isPaused : !contSequence.isPaused;
        }

        /// <summary>
        /// Returns <c>true</c> if the tween with the given int id is currently involved in a running tween or sequence.
        /// This method is here to uniform <see cref="T:Holoville.HOTween.Tweener" /> with <see cref="T:Holoville.HOTween.Sequence" />.
        /// </summary>
        /// <param name="p_id">The id to check for.</param>
        public override bool IsTweening(int p_id)
        {
            if (!_enabled || intId != p_id)
                return false;
            return !isSequenced ? !_isPaused : !contSequence.isPaused;
        }

        /// <summary>
        /// Returns <c>true</c> if the given target and this Tweener target are the same.
        /// This method is here to uniform <see cref="T:Holoville.HOTween.Tweener" /> with <see cref="T:Holoville.HOTween.Sequence" />.
        /// </summary>
        /// <param name="p_target">The target to check.</param>
        /// <returns>
        /// A value of <c>true</c> if the given target and this Tweener target are the same.
        /// </returns>
        public override bool IsLinkedTo(object p_target) => p_target == _target;

        /// <summary>Returns a list containing the target of this tween.</summary>
        /// <returns>A list containing the target of this tween.</returns>
        public override List<object> GetTweenTargets() => new List<object>()
        {
            target
        };

        /// <summary>
        /// Returns a list containing this tween if the id is the same as the given one
        /// (or and empty list if no tweens were found).
        /// </summary>
        internal override List<IHOTweenComponent> GetTweensById(string p_id)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (id == p_id)
                hoTweenComponentList.Add(this);
            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns a list containing this tween if the id is the same as the given one
        /// (or and empty list if no tweens were found).
        /// </summary>
        internal override List<IHOTweenComponent> GetTweensByIntId(int p_intId)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (intId == p_intId)
                hoTweenComponentList.Add(this);
            return hoTweenComponentList;
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween,
        /// returns a point on the path at the given percentage (0 to 1).
        /// Returns a <c>zero Vector</c> if there's no path tween associated with this tween.
        /// Note that, if the tween wasn't started, the OnStart callback will be called
        /// the first time you call this method, because the tween needs to be initialized.
        /// </summary>
        /// <param name="t">The percentage (0 to 1) at which to get the point</param>
        public Vector3 GetPointOnPath(float t)
        {
            var vector3PathPlugin = GetPlugVector3PathPlugin();
            if (vector3PathPlugin == null)
                return Vector3.zero;
            Startup();
            return vector3PathPlugin.GetConstPointOnPath(t);
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween,
        /// defines a portion of that path to use and re-adapt to (easing included),
        /// also re-adapting the duration to the correct partial,
        /// and rewinds/restarts the tween in its partial form (depending if it was paused or not).
        /// </summary>
        /// <param name="p_waypointId0">
        /// Id of the new starting waypoint on the current path.
        /// If you want to be sure you're targeting the first point in the path, pass -1
        /// (this is because the first waypoint of the path might be different from the first waypoint you passed,
        /// in case the target Transform was not already on the starting position, and thus needed to reach it).
        /// </param>
        /// <param name="p_waypointId1">Id of the new ending waypoint on the current path</param>
        public Tweener UsePartialPath(int p_waypointId0, int p_waypointId1)
        {
            var p_newEaseType = !isPartialled ? _easeType : _originalEaseType;
            return UsePartialPath(p_waypointId0, p_waypointId1, -1f, p_newEaseType);
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween,
        /// defines a portion of that path to use and re-adapt to (easing included),
        /// and rewinds/restarts the tween in its partial form (depending if it was paused or not).
        /// </summary>
        /// <param name="p_waypointId0">
        /// Id of the new starting waypoint on the current path.
        /// If you want to be sure you're targeting the first point in the path, pass -1
        /// (this is because the first waypoint of the path might be different from the first waypoint you passed,
        /// in case the target Transform was not already on the starting position, and thus needed to reach it).
        /// </param>
        /// <param name="p_waypointId1">Id of the new ending waypoint on the current path</param>
        /// <param name="p_newDuration">
        /// Tween duration of the partial path (if -1 auto-calculates the correct partial based on the original duration)
        /// </param>
        public Tweener UsePartialPath(int p_waypointId0, int p_waypointId1, float p_newDuration)
        {
            var p_newEaseType = isPartialled ? _originalEaseType : _easeType;
            return UsePartialPath(p_waypointId0, p_waypointId1, p_newDuration, p_newEaseType);
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween,
        /// defines a portion of that path to use and re-adapt to (easing included),
        /// and rewinds/restarts the tween in its partial form (depending if it was paused or not).
        /// </summary>
        /// <param name="p_waypointId0">
        /// Id of the new starting waypoint on the current path.
        /// If you want to be sure you're targeting the first point in the path, pass -1
        /// (this is because the first waypoint of the path might be different from the first waypoint you passed,
        /// in case the target Transform was not already on the starting position, and thus needed to reach it).
        /// </param>
        /// <param name="p_waypointId1">Id of the new ending waypoint on the current path</param>
        /// <param name="p_newEaseType">New EaseType to apply</param>
        public Tweener UsePartialPath(
            int p_waypointId0,
            int p_waypointId1,
            EaseType p_newEaseType)
        {
            return UsePartialPath(p_waypointId0, p_waypointId1, -1f, p_newEaseType);
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween,
        /// defines a portion of that path to use and re-adapt to,
        /// and rewinds/restarts the tween in its partial form (depending if it was paused or not).
        /// </summary>
        /// <param name="p_waypointId0">
        /// Id of the new starting waypoint on the current path.
        /// If you want to be sure you're targeting the first point in the path, pass -1
        /// (this is because the first waypoint of the path might be different from the first waypoint you passed,
        /// in case the target Transform was not already on the starting position, and thus needed to reach it).
        /// </param>
        /// <param name="p_waypointId1">
        /// Id of the new ending waypoint on the current path
        /// (-1 in case you want to target the ending waypoint of a closed path)
        /// </param>
        /// <param name="p_newDuration">
        /// Tween duration of the partial path (if -1 auto-calculates the correct partial based on the original duration)
        /// </param>
        /// <param name="p_newEaseType">New EaseType to apply</param>
        public Tweener UsePartialPath(
            int p_waypointId0,
            int p_waypointId1,
            float p_newDuration,
            EaseType p_newEaseType)
        {
            if (plugins.Count > 1)
            {
                TweenWarning.Log("Applying a partial path on a Tweener (" + _target +
                                 ") with more than one plugin/property being tweened is not allowed");
                return this;
            }

            if (pv3Path == null)
            {
                pv3Path = GetPlugVector3PathPlugin();
                if (pv3Path == null)
                {
                    TweenWarning.Log("Tweener for " + _target + " contains no PlugVector3Path plugin");
                    return this;
                }
            }

            Startup();
            if (!isPartialled)
            {
                isPartialled = true;
                _originalDuration = _duration;
                _originalEaseType = _easeType;
            }

            var pathId1 = ConvertWaypointIdToPathId(pv3Path, p_waypointId0, true);
            var pathId2 = ConvertWaypointIdToPathId(pv3Path, p_waypointId1, false);
            var lengthPercentage = pv3Path.GetWaypointsLengthPercentage(pathId1, pathId2);
            var p_partialStartPerc = pathId1 == 0 ? 0.0f : pv3Path.GetWaypointsLengthPercentage(0, pathId1);
            _duration = p_newDuration >= 0.0 ? p_newDuration : _originalDuration * lengthPercentage;
            _easeType = p_newEaseType;
            pv3Path.SwitchToPartialPath(_duration, p_newEaseType, p_partialStartPerc, lengthPercentage);
            Startup(true);
            if (!_isPaused)
                Restart(true);
            else
                Rewind(true, true);
            return this;
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" /> tween
        /// that had been partialized, returns it to its original size, easing, and duration,
        /// and rewinds/restarts the tween in its partial form (depending if it was paused or not).
        /// </summary>
        public void ResetPath()
        {
            isPartialled = false;
            _duration = speedBased ? _originalNonSpeedBasedDuration : _originalDuration;
            _easeType = _originalEaseType;
            pv3Path.ResetToFullPath(_duration, _easeType);
            Startup(true);
            if (!_isPaused)
                Restart(true);
            else
                Rewind(true);
        }

        /// <summary>
        /// If this Tweener contains a <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" />, returns it.
        /// Otherwise returns null.
        /// </summary>
        /// <returns></returns>
        private PlugVector3Path GetPlugVector3PathPlugin()
        {
            if (plugins == null)
                return null;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
            {
                if (plugins[index] is PlugVector3Path plugin1)
                    return plugin1;
            }

            return null;
        }

        internal override bool Update(
            float p_shortElapsed,
            bool p_forceUpdate,
            bool p_isStartupIteration,
            bool p_ignoreCallbacks)
        {
            return Update(p_shortElapsed, p_forceUpdate, p_isStartupIteration, p_ignoreCallbacks, false);
        }

        /// <summary>
        /// Updates the Tweener by the given elapsed time,
        /// and returns a value of <c>true</c> if the Tweener is complete.
        /// </summary>
        /// <param name="p_shortElapsed">
        /// The elapsed time since the last update.
        /// </param>
        /// <param name="p_forceUpdate">
        /// If <c>true</c> forces the update even if the Tweener is complete or paused,
        /// but ignores onUpdate, and sends onComplete and onStepComplete calls only if the Tweener wasn't complete before this call.
        /// </param>
        /// <param name="p_isStartupIteration">
        /// If <c>true</c> means the update is due to a startup iteration (managed by Sequence Startup or HOTween.From),
        /// and all callbacks will be ignored.
        /// </param>
        /// <param name="p_ignoreCallbacks">
        /// If <c>true</c> doesn't call any callback method.
        /// </param>
        /// <param name="p_ignoreDelay">
        /// If <c>true</c> uses p_shortElapsed fully ignoring the delay
        /// (useful when setting the initial FROM state).
        /// </param>
        /// <returns>
        /// A value of <c>true</c> if the Tweener is not reversed and is complete (or the tween target doesn't exist anymore), otherwise <c>false</c>.
        /// </returns>
        internal bool Update(
            float p_shortElapsed,
            bool p_forceUpdate,
            bool p_isStartupIteration,
            bool p_ignoreCallbacks,
            bool p_ignoreDelay = false)
        {
            if (_destroyed)
                return true;
            if (_target == null || _target.Equals(null))
            {
                Kill(false);
                return true;
            }

            if (!_enabled)
                return false;
            if (_isComplete && !_isReversed && !p_forceUpdate)
                return true;
            if (_fullElapsed == 0.0 && _isReversed && !p_forceUpdate || _isPaused && !p_forceUpdate)
                return false;
            ignoreCallbacks = p_isStartupIteration || p_ignoreCallbacks;
            if (p_ignoreDelay || delayCount == 0.0)
            {
                Startup();
                if (!_hasStarted)
                    OnStart();
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
            }
            else
            {
                if (_timeScale != 0.0)
                    _elapsedDelay += p_shortElapsed / _timeScale;
                if (_elapsedDelay < (double)delayCount)
                    return false;
                if (_isReversed)
                {
                    _fullElapsed = _elapsed = 0.0f;
                }
                else
                {
                    _fullElapsed = _elapsed = _elapsedDelay - delayCount;
                    if (_fullElapsed > (double)_fullDuration)
                        _fullElapsed = _fullDuration;
                }

                _elapsedDelay = delayCount;
                delayCount = 0.0f;
                Startup();
                if (!_hasStarted)
                    OnStart();
            }

            var isComplete = _isComplete;
            var flag1 = !_isReversed && !isComplete && _elapsed >= (double)_duration;
            SetLoops();
            SetElapsed();
            _isComplete = !_isReversed && _loops >= 0 && _completedLoops >= _loops;
            var flag2 = !isComplete && _isComplete;
            var p_totElapsed = !_isLoopingBack ? _elapsed : _duration - _elapsed;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
            {
                var plugin = plugins[index];
                if (!_isLoopingBack && plugin.easeReversed ||
                    _isLoopingBack && _loopType == LoopType.YoyoInverse && !plugin.easeReversed)
                    plugin.ReverseEase();
                if (_duration > 0.0)
                {
                    plugin.Update(p_totElapsed);
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
            prevFullElapsed = _fullElapsed;
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
            if (plugins == null)
                return;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
                plugins[index].ForceSetIncremental(p_diffIncr);
        }

        /// <summary>
        /// If speed based duration was not already set (meaning OnStart has not yet been called),
        /// calculates the duration and then resets the tween so that OnStart can be called from scratch.
        /// Used by Sequences when Appending/Prepending/Inserting speed based tweens.
        /// </summary>
        internal void ForceSetSpeedBasedDuration()
        {
            if (!_speedBased || plugins == null)
                return;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
                plugins[index].ForceSetSpeedBasedDuration();
            _duration = 0.0f;
            for (var index = 0; index < count; ++index)
            {
                var plugin = plugins[index];
                if (plugin.duration > (double)_duration)
                    _duration = plugin.duration;
            }

            SetFullDuration();
        }

        /// <summary>
        /// Sends the tween to the given time (taking also loops into account) and eventually plays it.
        /// If the time is bigger than the total tween duration, it goes to the end.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the tween reached its end and was completed.
        /// </returns>
        protected override bool GoTo(
            float p_time,
            bool p_play,
            bool p_forceUpdate,
            bool p_ignoreCallbacks)
        {
            if (!_enabled)
                return false;
            if (p_time > (double)_fullDuration)
                p_time = _fullDuration;
            else if (p_time < 0.0)
                p_time = 0.0f;
            if (!p_forceUpdate && _fullElapsed == (double)p_time)
            {
                if (!_isComplete && p_play)
                    Play();
                return _isComplete;
            }

            _fullElapsed = p_time;
            delayCount = 0.0f;
            _elapsedDelay = _delay;
            Update(0.0f, true, false, p_ignoreCallbacks, false);
            if (!_isComplete && p_play)
                Play();
            return _isComplete;
        }

        private void Rewind(bool p_play, bool p_skipDelay)
        {
            if (!_enabled)
                return;
            Startup();
            if (!_hasStarted)
                OnStart();
            _isComplete = false;
            _isLoopingBack = false;
            delayCount = p_skipDelay ? 0.0f : _delay;
            _elapsedDelay = p_skipDelay ? _delay : 0.0f;
            _completedLoops = 0;
            _fullElapsed = _elapsed = 0.0f;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
            {
                var plugin = plugins[index];
                if (plugin.easeReversed)
                    plugin.ReverseEase();
                plugin.Rewind();
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

        private void SkipDelay()
        {
            if (delayCount <= 0.0)
                return;
            delayCount = 0.0f;
            _elapsedDelay = _delay;
            _elapsed = _fullElapsed = 0.0f;
        }

        /// <summary>
        /// Startup this tween
        /// (might or might not call OnStart, depending if the tween is in a Sequence or not).
        /// Can be executed only once per tween.
        /// </summary>
        protected override void Startup() => Startup(false);

        /// <summary>
        /// Startup this tween
        /// (might or might not call OnStart, depending if the tween is in a Sequence or not).
        /// Can be executed only once per tween.
        /// </summary>
        /// <param name="p_force">If TRUE forces startup even if it had already been executed</param>
        private void Startup(bool p_force)
        {
            if (!p_force && startupDone)
                return;
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
            {
                var plugin = plugins[index];
                if (!plugin.wasStarted)
                    plugin.Startup();
            }

            if (_speedBased)
            {
                _originalNonSpeedBasedDuration = _duration;
                _duration = 0.0f;
                for (var index = 0; index < count; ++index)
                {
                    var plugin = plugins[index];
                    if (plugin.duration > (double)_duration)
                        _duration = plugin.duration;
                }

                SetFullDuration();
            }
            else if (p_force)
                SetFullDuration();

            base.Startup();
        }

        /// <summary>Manages on first start behaviour.</summary>
        protected override void OnStart()
        {
            if (steadyIgnoreCallbacks || ignoreCallbacks)
                return;
            if (HOTween.OverwriteManager != null)
                HOTween.OverwriteManager.AddTween(this);
            base.OnStart();
        }

        protected override void OnPlay()
        {
            if (_delay <= 0.0 || _elapsedDelay > 0.0)
                return;
            Update(0.0f, true, true, false, true);
        }

        /// <summary>
        /// Fills the given list with all the plugins inside this tween.
        /// Used by <c>HOTween.GetPlugins</c>.
        /// </summary>
        internal override void FillPluginsList(List<ABSTweenPlugin> p_plugs)
        {
            if (plugins == null) return;
            
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
                p_plugs.Add(plugins[index]);
        }

        /// <summary>
        /// Returns the correct id of the given waypoint, converted to path id.
        /// </summary>
        /// <param name="p_plugVector3Path">Vector3 path plugin to use</param>
        /// <param name="p_waypointId">Waypoint to convert</param>
        /// <param name="p_isStartingWp">If TRUE indicates that the given waypoint is the starting one,
        /// otherwise it's the ending one</param>
        /// <returns></returns>
        private static int ConvertWaypointIdToPathId(
            PlugVector3Path p_plugVector3Path,
            int p_waypointId,
            bool p_isStartingWp)
        {
            return p_waypointId == -1
                ? (!p_isStartingWp ? p_plugVector3Path.path.path.Length - 2 : 1)
                : (p_plugVector3Path.hasAdditionalStartingP ? p_waypointId + 2 : p_waypointId + 1);
        }
    }
}