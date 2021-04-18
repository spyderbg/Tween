using Holoville.HOTween.Plugins.Core;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugSetColor : ABSTweenPlugin
{
    internal static System.Type[] ValidTargetTypes = new System.Type[1]
    {
        typeof(Material)
    };

    internal static System.Type[] ValidPropTypes = new System.Type[1]
    {
        typeof(Color)
    };

    internal static System.Type[] ValidValueTypes = new System.Type[1]
    {
        typeof(Color)
    };

    private Color _typedStartVal;
    private Color _typedEndVal;
    private float _changeVal;
    private Color _diffChangeVal;
    private string _colorName;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.isFrom && IsRelative)
                StartVal = _typedStartVal = _typedEndVal + (Color)value;
            else
                StartVal = _typedStartVal = (Color)value;
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set => EndVal = _typedEndVal = (Color)value;
    }

    public PlugSetColor(Color endVal)
        : this(endVal, false)
    {
    }

    public PlugSetColor(Color endVal, EaseType easeType)
        : this(endVal, easeType, false)
    {
    }

    public PlugSetColor(Color endVal, bool isRelative)
        : base(endVal, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetColor(Color endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetColor(Color endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
        IgnoreAccessor = true;
    }

    public PlugSetColor Property(ColorName colorName) =>
        Property(colorName.ToString());

    public PlugSetColor Property(string propertyName)
    {
        _colorName = propertyName;
        return this;
    }

    internal override bool ValidateTarget(object target) =>
        target is Material;

    protected override void DoUpdate(float totElapsed) => SetValue(Color.Lerp(_typedStartVal, _typedEndVal,
        Ease(totElapsed, 0.0f, _changeVal, Duration, TweenObj.easeOvershootOrAmplitude, TweenObj.easePeriod)));

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = _changeVal / speed;
        if (num < 0.0)
            num = -num;
        return num;
    }

    protected override void SetChangeVal()
    {
        _changeVal = 1f;
        if (IsRelative && !TweenObj.isFrom)
            _typedEndVal = _typedStartVal + _typedEndVal;
        _diffChangeVal = _typedEndVal - _typedStartVal;
    }

    protected override void SetIncremental(int diffIncr)
    {
        _typedStartVal += _diffChangeVal * diffIncr;
        _typedEndVal += _diffChangeVal * diffIncr;
    }

    protected override void SetValue(object value) =>
        ((Material)TweenObj.target).SetColor(_colorName, (Color)value);

    protected override object GetValue() =>
        ((Material)TweenObj.target).GetColor(_colorName);

    public enum ColorName
    {
        Color,
        SpecColor,
        Emission,
        ReflectColor,
    }
}

}