using FastDynamicMemberAccessor;
using Holoville.HOTween.Core;
using Holoville.HOTween.Core.Easing;
using System;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween.Plugins.Core {

public abstract class ABSTweenPlugin
{
    protected object StartVal;

    protected object EndVal;

    protected float Duration;

    private bool _initialized;
    private bool _easeReversed;

    protected string PropName;

    internal Type TargetType;

    protected TweenDelegate.EaseFunc Ease;

    protected bool IsRelative;

    protected bool IgnoreAccessor;

    private EaseType _easeType;
    private EaseInfo _easeInfo;
    private EaseCurve _easeCurve;
    private IMemberAccessor _valAccessor;
    internal bool WasStarted;
    private bool _speedBasedDurationWasSet;
    private int _prevCompletedLoops;
    private bool _useSpeedTransformAccessors;
    private Transform _transformTarget;
    private TweenDelegate.HOAction<Vector3> _setTransformVector3;
    private TweenDelegate.HOFunc<Vector3> _getTransformVector3;
    private TweenDelegate.HOAction<Quaternion> _setTransformQuaternion;
    private TweenDelegate.HOFunc<Quaternion> _getTransformQuaternion;
    private PropertyInfo _propInfo;
    private FieldInfo _fieldInfo;

    protected Tweener TweenObj;

    protected abstract object startVal { get; set; }

    protected abstract object endVal { get; set; }

    internal bool initialized => _initialized;

    internal float duration => Duration;

    internal bool easeReversed => _easeReversed;

    internal string propName => PropName;

    internal virtual int pluginId => -1;

    protected ABSTweenPlugin(object endVal, bool isRelative)
    {
        IsRelative = isRelative;
        EndVal = endVal;
    }

    protected ABSTweenPlugin(object endVal, EaseType easeType, bool isRelative)
    {
        IsRelative = isRelative;
        EndVal = endVal;
        _easeType = easeType;
        _easeInfo = EaseInfo.GetEaseInfo(easeType);
        Ease = _easeInfo.Ease;
    }

    protected ABSTweenPlugin(object endVal, AnimationCurve easeAnimCurve, bool isRelative)
    {
        IsRelative = isRelative;
        EndVal = endVal;
        _easeType = EaseType.AnimationCurve;
        _easeCurve = new EaseCurve(easeAnimCurve);
        _easeInfo = null;
        Ease = new TweenDelegate.EaseFunc(_easeCurve.Evaluate);
    }

    internal virtual void Init(Tweener tweenObj, string propertyName, EaseType easeType, Type targetType, PropertyInfo propertyInfo, FieldInfo fieldInfo)
    {
        _initialized = true;
        TweenObj = tweenObj;
        PropName = propertyName;
        TargetType = targetType;
        if (_easeType != EaseType.AnimationCurve && _easeInfo == null || TweenObj.speedBased ||
            _easeType == EaseType.AnimationCurve && _easeCurve == null)
            SetEase(easeType);
        Duration = TweenObj.Duration;
        if (TargetType == typeof(Transform))
        {
            _transformTarget = tweenObj.target as Transform;
            _useSpeedTransformAccessors = true;
            switch (PropName)
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
            _propInfo = propertyInfo;
            _fieldInfo = fieldInfo;
        }
        else
        {
            if (IgnoreAccessor)
                return;
            _valAccessor = MemberAccessorCacher.Make(targetType, propertyName, propertyInfo, fieldInfo);
        }
    }

    internal void Startup() => Startup(false);

    internal void Startup(bool onlyCalcSpeedBasedDur)
    {
        if (WasStarted)
        {
            TweenWarning.Log("Startup() for plugin " + this + " (target: " + TweenObj.target +
                             ") has already been called. Startup() won't execute twice.");
        }
        else
        {
            object obj1 = null;
            object obj2 = null;
            if (onlyCalcSpeedBasedDur)
            {
                if (TweenObj.speedBased && !_speedBasedDurationWasSet)
                {
                    obj1 = StartVal;
                    obj2 = EndVal;
                }
            }
            else
                WasStarted = true;

            if (TweenObj.isFrom)
            {
                var endVal = EndVal;
                this.endVal = GetValue();
                startVal = endVal;
            }
            else
            {
                endVal = EndVal;
                startVal = GetValue();
            }

            SetChangeVal();
            if (!TweenObj.speedBased || _speedBasedDurationWasSet)
                return;
            Duration = GetSpeedBasedDuration(Duration);
            _speedBasedDurationWasSet = true;
            if (!onlyCalcSpeedBasedDur)
                return;
            StartVal = obj1;
            EndVal = obj2;
        }
    }

    internal void ForceSetSpeedBasedDuration()
    {
        if (_speedBasedDurationWasSet)
            return;
        Startup(true);
    }

    internal virtual bool ValidateTarget(object target) => true;

    internal void Update(float totElapsed)
    {
        if (TweenObj.loopType == LoopType.Incremental)
        {
            if (_prevCompletedLoops != TweenObj.completedLoops)
            {
                var completedLoops = TweenObj.completedLoops;
                if (TweenObj._loops != -1 && completedLoops >= TweenObj._loops)
                    --completedLoops;
                var diffIncr = completedLoops - _prevCompletedLoops;
                if (diffIncr != 0)
                {
                    SetIncremental(diffIncr);
                    _prevCompletedLoops = completedLoops;
                }
            }
        }
        else if (_prevCompletedLoops != 0)
        {
            SetIncremental(-_prevCompletedLoops);
            _prevCompletedLoops = 0;
        }

        if (totElapsed > (double)Duration)
            totElapsed = Duration;
        DoUpdate(totElapsed);
    }

    protected abstract void DoUpdate(float totElapsed);

    internal virtual void Rewind() => SetValue(startVal);

    internal virtual void Complete() => SetValue(EndVal);

    internal void ReverseEase()
    {
        _easeReversed = !_easeReversed;
        if (_easeType == EaseType.AnimationCurve || _easeInfo.InverseEase == null)
            return;
        Ease = _easeReversed ? _easeInfo.InverseEase : _easeInfo.Ease;
    }

    internal void SetEase(EaseType easeType)
    {
        _easeType = easeType;
        if (_easeType == EaseType.AnimationCurve)
        {
            if (TweenObj._easeAnimationCurve != null)
            {
                _easeCurve = new EaseCurve(TweenObj._easeAnimationCurve);
                Ease = new TweenDelegate.EaseFunc(_easeCurve.Evaluate);
            }
            else
            {
                _easeType = EaseType.EaseOutQuad;
                _easeInfo = EaseInfo.GetEaseInfo(_easeType);
                Ease = _easeInfo.Ease;
            }
        }
        else
        {
            _easeInfo = EaseInfo.GetEaseInfo(_easeType);
            Ease = _easeInfo.Ease;
        }

        if (!_easeReversed || _easeInfo.InverseEase == null)
            return;
        Ease = _easeInfo.InverseEase;
    }

    protected abstract float GetSpeedBasedDuration(float speed);

    internal ABSTweenPlugin CloneBasic() =>
        Activator.CreateInstance(GetType(), !(TweenObj is {isFrom: true}) ? EndVal : StartVal, _easeType, IsRelative) as ABSTweenPlugin;

    protected abstract void SetChangeVal();

    internal void ForceSetIncremental(int diffIncr) =>
        SetIncremental(diffIncr);

    protected abstract void SetIncremental(int diffIncr);

    protected virtual void SetValue(object value)
    {
        if (_useSpeedTransformAccessors)
        {
            if (_setTransformVector3 != null)
                _setTransformVector3((Vector3)value);
            else
                _setTransformQuaternion((Quaternion)value);
        }
        else if (HOTween.IsIOS)
        {
            if (_propInfo != null)
            {
                try
                {
                    _propInfo.SetValue(TweenObj.target, value, null);
                }
                catch (InvalidCastException ex)
                {
                    _propInfo.SetValue(TweenObj.target, (int)Math.Floor((float)value), null);
                }
                catch (ArgumentException ex)
                {
                    _propInfo.SetValue(TweenObj.target, (int)Math.Floor((float)value), null);
                }
            }
            else
            {
                try
                {
                    _fieldInfo.SetValue(TweenObj.target, value);
                }
                catch (InvalidCastException ex)
                {
                    _fieldInfo.SetValue(TweenObj.target, (int)Math.Floor((float)value));
                }
                catch (ArgumentException ex)
                {
                    _fieldInfo.SetValue(TweenObj.target, (int)Math.Floor((float)value));
                }
            }
        }
        else
        {
            try
            {
                _valAccessor.Set(TweenObj.target, value);
            }
            catch (InvalidCastException ex)
            {
                _valAccessor.Set(TweenObj.target, (int)Math.Floor((float)value));
            }
            catch (ArgumentException ex)
            {
                _valAccessor.Set(TweenObj.target, (int)Math.Floor((float)value));
            }
        }
    }

    protected virtual object GetValue()
    {
        if (_useSpeedTransformAccessors)
            return _getTransformVector3?.Invoke() ?? (object)_getTransformQuaternion();
        if (!HOTween.IsIOS)
            return _valAccessor.Get(TweenObj.target);
        return _propInfo != null
            ? _propInfo.GetGetMethod().Invoke(TweenObj.target, null)
            : _fieldInfo.GetValue(TweenObj.target);
    }
}

}