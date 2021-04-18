using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugVector3Z : PlugVector3X
{
    internal override int pluginId => 3;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.IsFrom)
            {
                if (IsRelative)
                    StartVal = TypedStartVal = TypedEndVal + Convert.ToSingle(value);
                else
                    StartVal = TypedStartVal = Convert.ToSingle(value);
            }
            else
            {
                StartVal = value;
                TypedStartVal = ((Vector3)StartVal).z;
            }
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set
        {
            if (TweenObj.IsFrom)
            {
                EndVal = value;
                TypedEndVal = ((Vector3)EndVal).z;
            }
            else
                EndVal = TypedEndVal = Convert.ToSingle(value);
        }
    }

    public PlugVector3Z(float endVal)
        : base(endVal, false)
    {
    }

    public PlugVector3Z(float endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugVector3Z(float endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugVector3Z(float endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
    }

    public PlugVector3Z(float endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
    }

    internal override void Rewind()
    {
        var vector3 = (Vector3)GetValue();
        vector3.z = TypedStartVal;
        SetValue(vector3);
    }

    internal override void Complete()
    {
        var vector3 = (Vector3)GetValue();
        vector3.z = TypedEndVal;
        SetValue(vector3);
    }

    protected override void DoUpdate(float totElapsed)
    {
        var vector3 = (Vector3)GetValue();
        vector3.z = Ease(totElapsed, TypedStartVal, ChangeVal, Duration, TweenObj.EaseOvershootOrAmplitude,
            TweenObj.EasePeriod);
        if (TweenObj.PixelPerfect)
            vector3.z = (int)vector3.z;
        SetValue(vector3);
    }
}

}