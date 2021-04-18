using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugSetFloat : ABSTweenPlugin
{
    internal static Type[] ValidTargetTypes = new Type[1]
    {
        typeof(Material)
    };

    internal static Type[] ValidPropTypes = new Type[1]
    {
        typeof(Color)
    };

    internal static Type[] ValidValueTypes = new Type[1]
    {
        typeof(float)
    };

    private float _typedStartVal;
    private float _typedEndVal;
    private float _changeVal;
    private string _floatName;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.isFrom && IsRelative)
                StartVal = _typedStartVal = _typedEndVal + Convert.ToSingle(value);
            else
                StartVal = _typedStartVal = Convert.ToSingle(value);
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set => EndVal = _typedEndVal = Convert.ToSingle(value);
    }

    public PlugSetFloat(float endVal)
        : this(endVal, false)
    {
    }

    public PlugSetFloat(float endVal, EaseType easeType)
        : this(endVal, easeType, false)
    {
    }

    public PlugSetFloat(float endVal, bool isRelative)
        : base(endVal, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetFloat(float endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetFloat(float endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetFloat Property(string propertyName)
    {
        _floatName = propertyName;
        return this;
    }

    internal override bool ValidateTarget(object target) =>
        target is Material;

    protected override void DoUpdate(float totElapsed) => SetValue(Ease(totElapsed, _typedStartVal, _changeVal,
        Duration, TweenObj.easeOvershootOrAmplitude, TweenObj.easePeriod));

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = _changeVal / speed;
        if (num < 0.0)
            num = -num;
        return num;
    }

    protected override void SetChangeVal()
    {
        if (IsRelative && !TweenObj.isFrom)
        {
            _changeVal = _typedEndVal;
            endVal = (float)(_typedStartVal + (double)_typedEndVal);
        }
        else
            _changeVal = _typedEndVal - _typedStartVal;
    }

    protected override void SetIncremental(int diffIncr) => _typedStartVal += _changeVal * diffIncr;

    protected override void SetValue(object value) =>
        ((Material)TweenObj.target).SetFloat(_floatName, Convert.ToSingle(value));

    protected override object GetValue() => ((Material)TweenObj.target).GetFloat(_floatName);
}

}