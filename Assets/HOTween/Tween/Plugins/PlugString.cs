using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugString : ABSTweenPlugin
{
    internal static Type[] ValidPropTypes = new Type[1]
    {
        typeof(string)
    };

    internal static readonly Type[] ValidValueTypes = new Type[1]
    {
        typeof(string)
    };

    private string _typedStartVal;
    private string _typedEndVal;
    private float _changeVal;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.IsFrom && IsRelative)
                StartVal = _typedStartVal = _typedEndVal + (value == null ? "" : value.ToString());
            else
                StartVal = _typedStartVal = value == null ? "" : value.ToString();
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set => EndVal = _typedEndVal = value == null ? "" : value.ToString();
    }

    public PlugString(string endVal)
        : base(endVal, false)
    {
    }

    public PlugString(string endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugString(string endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugString(string endVal, EaseType easeType, bool p_isRelative)
        : base(endVal, easeType, p_isRelative)
    {
    }

    public PlugString(string endVal, AnimationCurve easeAnimCurve, bool p_isRelative)
        : base(endVal, easeAnimCurve, p_isRelative)
    {
    }

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = _changeVal / speed;
        if (num < 0.0)
            num = -num;
        return num;
    }

    protected override void SetChangeVal() => _changeVal = _typedEndVal.Length;

    protected override void SetIncremental(int diffIncr)
    {
        if (diffIncr > 0)
        {
            for (; diffIncr > 0; --diffIncr)
                _typedStartVal += _typedEndVal;
        }
        else
            _typedStartVal = _typedStartVal.Substring(0, _typedStartVal.Length + _typedEndVal.Length * diffIncr);
    }

    protected override void DoUpdate(float totElapsed)
    {
        var num = (int)Math.Round(Ease(totElapsed, 0.0f, _changeVal, Duration, TweenObj.EaseOvershootOrAmplitude, TweenObj.EasePeriod));
        
        SetValue(!IsRelative
            ? _typedEndVal.Substring(0, num) + (num >= (double)_changeVal || num >= _typedStartVal.Length
                ? ""
                : _typedStartVal.Substring(num))
            : (object)(_typedStartVal + _typedEndVal.Substring(0, num)));
    }
}

}