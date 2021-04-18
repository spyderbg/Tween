using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugInt : ABSTweenPlugin
{
    internal static Type[] ValidPropTypes = new Type[2]
    {
        typeof(float),
        typeof(int)
    };

    internal static Type[] validValueTypes = new Type[1]
    {
        typeof(int)
    };

    private float _typedStartVal;
    private float _typedEndVal;
    private float _changeVal;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.IsFrom && IsRelative)
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

    public PlugInt(float endVal)
        : base(endVal, false)
    {
    }

    public PlugInt(float endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugInt(float endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugInt(float endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
    }

    public PlugInt(float endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
    }

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = _changeVal / speed;
        if (num < 0.0)
            num = -num;
        return num;
    }

    protected override void SetChangeVal()
    {
        if (IsRelative && !TweenObj.IsFrom)
        {
            _changeVal = _typedEndVal;
            endVal = (float)(_typedStartVal + (double)_typedEndVal);
        }
        else
            _changeVal = _typedEndVal - _typedStartVal;
    }

    protected override void SetIncremental(int diffIncr) => _typedStartVal += _changeVal * diffIncr;

    protected override void DoUpdate(float totElapsed) => SetValue((float)Math.Round(Ease(totElapsed,
        _typedStartVal, _changeVal, Duration, TweenObj.EaseOvershootOrAmplitude, TweenObj.EasePeriod)));
}

}