﻿using FastDynamicMemberAccessor;
using Holoville.HOTween.Core;
using Holoville.HOTween.Core.Easing;
using System;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween.Plugins.Core
{
    /// <summary>
    /// ABSTRACT base class for all <see cref="T:Holoville.HOTween.Plugins.Core.ABSTweenPlugin" /> classes.
    /// </summary>
    public abstract class ABSTweenPlugin
    {
        /// <summary>Untyped start value.</summary>
        protected object _startVal;

        /// <summary>Untyped end value.</summary>
        protected object _endVal;

        /// <summary>
        /// Stored so it can be set indipendently in case of speed-based tweens.
        /// </summary>
        protected float _duration;

        private bool _initialized;
        private bool _easeReversed;

        /// <summary>
        /// Name of the property being tweened. Stored during Init, used by overwrite manager and log messages.
        /// </summary>
        protected string _propName;

        /// <summary>
        /// Stored to be used during recreation of plugin for partial tweens.
        /// </summary>
        internal Type targetType;

        /// <summary>Ease type.</summary>
        protected TweenDelegate.EaseFunc ease;

        /// <summary>
        /// Indicates that the end value is relative instead than absolute.
        /// Default: <c>false</c>.
        /// </summary>
        protected bool isRelative;

        /// <summary>
        /// Some plugins (like PlugSetColor) may set this to <c>false</c> when instantiated,
        /// to prevent the creation of a useless valAccessor.
        /// </summary>
        protected bool ignoreAccessor;

        private EaseType easeType;
        private EaseInfo easeInfo;
        private EaseCurve easeCurve;
        private IMemberAccessor valAccessor;
        internal bool wasStarted;
        private bool speedBasedDurationWasSet;
        private int prevCompletedLoops;
        private bool _useSpeedTransformAccessors;
        private Transform _transformTarget;
        private TweenDelegate.HOAction<Vector3> _setTransformVector3;
        private TweenDelegate.HOFunc<Vector3> _getTransformVector3;
        private TweenDelegate.HOAction<Quaternion> _setTransformQuaternion;
        private TweenDelegate.HOFunc<Quaternion> _getTransformQuaternion;
        internal PropertyInfo propInfo;
        internal FieldInfo fieldInfo;

        /// <summary>Reference to the Tweener controlling this plugin.</summary>
        protected Tweener tweenObj;

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected abstract object startVal { get; set; }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected abstract object endVal { get; set; }

        /// <summary>
        /// Used by TweenParms to understand if this plugin was initialized with
        /// another Tweener, and thus clone it.
        /// </summary>
        internal bool initialized => _initialized;

        internal float duration => _duration;

        internal bool easeReversed => _easeReversed;

        /// <summary>
        /// Used by <see cref="T:Holoville.HOTween.Core.OverwriteManager" /> to get the property name.
        /// </summary>
        internal string propName => _propName;

        /// <summary>
        /// Some plugins might override this to specify a different ID (like PlugVector3X).
        /// Used by <see cref="T:Holoville.HOTween.Core.OverwriteManager" /> to check if two plugins are the same (for overwrite purposes).
        /// Plugins with -1 ids always overwrite and are overwritten.
        /// Plugins with different ids are always overwritten by plugins with -1 ids,
        /// but overwrite only identical ids.
        /// </summary>
        /// <value>The plugin identifier.</value>
        internal virtual int pluginId => -1;

        /// <summary>
        /// Creates a new instance of this plugin with the given options.
        /// Used because easeType can't be null, and otherwise there's no way
        /// to understand if the ease was voluntarily set by the user or not.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Object" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        protected ABSTweenPlugin(object p_endVal, bool p_isRelative)
        {
            isRelative = p_isRelative;
            _endVal = p_endVal;
        }

        /// <summary>
        /// Creates a new instance of this plugin with the given options.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Object" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        protected ABSTweenPlugin(object p_endVal, EaseType p_easeType, bool p_isRelative)
        {
            isRelative = p_isRelative;
            _endVal = p_endVal;
            easeType = p_easeType;
            easeInfo = EaseInfo.GetEaseInfo(p_easeType);
            ease = easeInfo.ease;
        }

        /// <summary>
        /// Creates a new instance of this plugin with the given options.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Object" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        protected ABSTweenPlugin(object p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
        {
            isRelative = p_isRelative;
            _endVal = p_endVal;
            easeType = EaseType.AnimationCurve;
            easeCurve = new EaseCurve(p_easeAnimCurve);
            easeInfo = null;
            ease = new TweenDelegate.EaseFunc(easeCurve.Evaluate);
        }

        /// <summary>
        /// Initializes the plugin after its instantiation.
        /// Called by Tweener after a property and plugin have been validated, and the plugin has to be set and added.
        /// Virtual because some classes (like PlugVector3Path) override it to avoid isRelative being TRUE.
        /// </summary>
        /// <param name="p_tweenObj">
        /// The <see cref="T:Holoville.HOTween.Tweener" /> to refer to.
        /// </param>
        /// <param name="p_propertyName">
        /// The name of the property to control.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_targetType">
        /// Directly passed from TweenParms to speed up MemberAccessor creation.
        /// </param>
        /// <param name="p_propertyInfo">
        /// Directly passed from TweenParms to speed up MemberAccessor creation.
        /// </param>
        /// <param name="p_fieldInfo">
        /// Directly passed from TweenParms to speed up MemberAccessor creation.
        /// </param>
        internal virtual void Init(
            Tweener p_tweenObj,
            string p_propertyName,
            EaseType p_easeType,
            Type p_targetType,
            PropertyInfo p_propertyInfo,
            FieldInfo p_fieldInfo)
        {
            _initialized = true;
            tweenObj = p_tweenObj;
            _propName = p_propertyName;
            targetType = p_targetType;
            if (easeType != EaseType.AnimationCurve && easeInfo == null || tweenObj.speedBased ||
                easeType == EaseType.AnimationCurve && easeCurve == null)
                SetEase(p_easeType);
            _duration = tweenObj.duration;
            if (targetType == typeof(Transform))
            {
                _transformTarget = p_tweenObj.target as Transform;
                _useSpeedTransformAccessors = true;
                switch (_propName)
                {
                    case "position":
                        _setTransformVector3 = value => _transformTarget.position = value;
                        _getTransformVector3 = () => _transformTarget.position;
                        break;
                    case "localPosition":
                        _setTransformVector3 = value => _transformTarget.localPosition = value;
                        _getTransformVector3 = () => _transformTarget.localPosition;
                        break;
                    case "localScale":
                        _setTransformVector3 = value => _transformTarget.localScale = value;
                        _getTransformVector3 = () => _transformTarget.localScale;
                        break;
                    case "rotation":
                        _setTransformQuaternion = value => _transformTarget.rotation = value;
                        _getTransformQuaternion = () => _transformTarget.rotation;
                        break;
                    case "localRotation":
                        _setTransformQuaternion = value => _transformTarget.localRotation = value;
                        _getTransformQuaternion = () => _transformTarget.localRotation;
                        break;
                    default:
                        _transformTarget = null;
                        _useSpeedTransformAccessors = false;
                        break;
                }
            }

            if (_useSpeedTransformAccessors)
                return;
            if (HOTween.IsIOS)
            {
                propInfo = p_propertyInfo;
                fieldInfo = p_fieldInfo;
            }
            else
            {
                if (ignoreAccessor)
                    return;
                valAccessor = MemberAccessorCacher.Make(p_targetType, p_propertyName, p_propertyInfo, p_fieldInfo);
            }
        }

        /// <summary>
        /// Starts up the plugin, getting the actual start and change values.
        /// Called by Tweener right before starting the effective animations.
        /// </summary>
        internal void Startup() => Startup(false);

        /// <summary>
        /// Starts up the plugin, getting the actual start and change values.
        /// Called by Tweener right before starting the effective animations.
        /// </summary>
        /// <param name="p_onlyCalcSpeedBasedDur">
        /// Set to <c>true</c> by <see cref="M:Holoville.HOTween.Plugins.Core.ABSTweenPlugin.ForceSetSpeedBasedDuration" />,
        /// to calculate only the speed based duration and then reset any startup changes
        /// (so Startup can be called from scratch when truly starting up).
        /// </param>
        internal void Startup(bool p_onlyCalcSpeedBasedDur)
        {
            if (wasStarted)
            {
                TweenWarning.Log("Startup() for plugin " + this + " (target: " + tweenObj.target +
                                 ") has already been called. Startup() won't execute twice.");
            }
            else
            {
                object obj1 = null;
                object obj2 = null;
                if (p_onlyCalcSpeedBasedDur)
                {
                    if (tweenObj.speedBased && !speedBasedDurationWasSet)
                    {
                        obj1 = _startVal;
                        obj2 = _endVal;
                    }
                }
                else
                    wasStarted = true;

                if (tweenObj.isFrom)
                {
                    var endVal = _endVal;
                    this.endVal = GetValue();
                    startVal = endVal;
                }
                else
                {
                    endVal = _endVal;
                    startVal = GetValue();
                }

                SetChangeVal();
                if (!tweenObj.speedBased || speedBasedDurationWasSet)
                    return;
                _duration = GetSpeedBasedDuration(_duration);
                speedBasedDurationWasSet = true;
                if (!p_onlyCalcSpeedBasedDur)
                    return;
                _startVal = obj1;
                _endVal = obj2;
            }
        }

        /// <summary>
        /// If speed based duration was not already set (meaning Startup has not yet been called),
        /// calculates the duration and then resets the plugin so that Startup will restart from scratch.
        /// Used by <see cref="M:Holoville.HOTween.Tweener.ForceSetSpeedBasedDuration" />.
        /// </summary>
        internal void ForceSetSpeedBasedDuration()
        {
            if (speedBasedDurationWasSet)
                return;
            Startup(true);
        }

        /// <summary>
        /// Overridden by plugins that need a specific type of target, to check it and validate it.
        /// Returns <c>true</c> if the tween target is valid.
        /// </summary>
        internal virtual bool ValidateTarget(object p_target) => true;

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">
        /// The total elapsed time since startup (loops excluded).
        /// </param>
        internal void Update(float p_totElapsed)
        {
            if (tweenObj.loopType == LoopType.Incremental)
            {
                if (prevCompletedLoops != tweenObj.completedLoops)
                {
                    var completedLoops = tweenObj.completedLoops;
                    if (tweenObj._loops != -1 && completedLoops >= tweenObj._loops)
                        --completedLoops;
                    var p_diffIncr = completedLoops - prevCompletedLoops;
                    if (p_diffIncr != 0)
                    {
                        SetIncremental(p_diffIncr);
                        prevCompletedLoops = completedLoops;
                    }
                }
            }
            else if (prevCompletedLoops != 0)
            {
                SetIncremental(-prevCompletedLoops);
                prevCompletedLoops = 0;
            }

            if (p_totElapsed > (double)_duration)
                p_totElapsed = _duration;
            DoUpdate(p_totElapsed);
        }

        /// <summary>Updates the plugin.</summary>
        protected abstract void DoUpdate(float p_totElapsed);

        /// <summary>
        /// Rewinds the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal virtual void Rewind() => SetValue(startVal);

        /// <summary>
        /// Completes the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal virtual void Complete() => SetValue(_endVal);

        /// <summary>Reverses the ease of this plugin.</summary>
        internal void ReverseEase()
        {
            _easeReversed = !_easeReversed;
            if (easeType == EaseType.AnimationCurve || easeInfo.inverseEase == null)
                return;
            ease = _easeReversed ? easeInfo.inverseEase : easeInfo.ease;
        }

        /// <summary>
        /// Sets the ease type (called during Init, but can also be called by Tweener to change easeType while playing).
        /// </summary>
        internal void SetEase(EaseType p_easeType)
        {
            easeType = p_easeType;
            if (easeType == EaseType.AnimationCurve)
            {
                if (tweenObj._easeAnimationCurve != null)
                {
                    easeCurve = new EaseCurve(tweenObj._easeAnimationCurve);
                    ease = new TweenDelegate.EaseFunc(easeCurve.Evaluate);
                }
                else
                {
                    easeType = EaseType.EaseOutQuad;
                    easeInfo = EaseInfo.GetEaseInfo(easeType);
                    ease = easeInfo.ease;
                }
            }
            else
            {
                easeInfo = EaseInfo.GetEaseInfo(easeType);
                ease = easeInfo.ease;
            }

            if (!_easeReversed || easeInfo.inverseEase == null)
                return;
            ease = easeInfo.inverseEase;
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed.
        /// </summary>
        protected abstract float GetSpeedBasedDuration(float p_speed);

        /// <summary>
        /// Returns a clone of the basic plugin
        /// (as it was at construction, without anything that was set during Init).
        /// </summary>
        internal ABSTweenPlugin CloneBasic() => Activator.CreateInstance(GetType(),
            tweenObj == null || !tweenObj.isFrom ? _endVal : _startVal, easeType, isRelative) as ABSTweenPlugin;

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// Can only be called once, otherwise some typedEndVal (like HOTPluginColor) will be set incorrectly.
        /// </summary>
        protected abstract void SetChangeVal();

        /// <summary>
        /// Used by Tweeners to force SetIncremental
        /// (SetIncremental can't be made internal since
        /// it needs to be overridden outside of HOTweem for custom plugin).
        /// </summary>
        internal void ForceSetIncremental(int p_diffIncr) => SetIncremental(p_diffIncr);

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// Also called by Tweener.ApplySequenceIncrement (used by Sequences during Incremental loops).
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected abstract void SetIncremental(int p_diffIncr);

        /// <summary>
        /// Sets the value of the controlled property.
        /// Some plugins (like PlugSetColor or PlugQuaterion) might override this to get values from different properties.
        /// </summary>
        /// <param name="p_value">The new value.</param>
        protected virtual void SetValue(object p_value)
        {
            if (_useSpeedTransformAccessors)
            {
                if (_setTransformVector3 != null)
                    _setTransformVector3((Vector3)p_value);
                else
                    _setTransformQuaternion((Quaternion)p_value);
            }
            else if (HOTween.IsIOS)
            {
                if (propInfo != null)
                {
                    try
                    {
                        propInfo.SetValue(tweenObj.target, p_value, null);
                    }
                    catch (InvalidCastException ex)
                    {
                        propInfo.SetValue(tweenObj.target, (int)Math.Floor((float)p_value), null);
                    }
                    catch (ArgumentException ex)
                    {
                        propInfo.SetValue(tweenObj.target, (int)Math.Floor((float)p_value), null);
                    }
                }
                else
                {
                    try
                    {
                        fieldInfo.SetValue(tweenObj.target, p_value);
                    }
                    catch (InvalidCastException ex)
                    {
                        fieldInfo.SetValue(tweenObj.target, (int)Math.Floor((float)p_value));
                    }
                    catch (ArgumentException ex)
                    {
                        fieldInfo.SetValue(tweenObj.target, (int)Math.Floor((float)p_value));
                    }
                }
            }
            else
            {
                try
                {
                    valAccessor.Set(tweenObj.target, p_value);
                }
                catch (InvalidCastException ex)
                {
                    valAccessor.Set(tweenObj.target, (int)Math.Floor((float)p_value));
                }
                catch (ArgumentException ex)
                {
                    valAccessor.Set(tweenObj.target, (int)Math.Floor((float)p_value));
                }
            }
        }

        /// <summary>
        /// Gets the current value of the controlled property.
        /// Some plugins (like PlugSetColor) might override this to set values on different properties.
        /// </summary>
        protected virtual object GetValue()
        {
            if (_useSpeedTransformAccessors)
                return _getTransformVector3 != null ? _getTransformVector3() : (object)_getTransformQuaternion();
            if (!HOTween.IsIOS)
                return valAccessor.Get(tweenObj.target);
            return propInfo != null
                ? propInfo.GetGetMethod().Invoke(tweenObj.target, null)
                : fieldInfo.GetValue(tweenObj.target);
        }
    }
}