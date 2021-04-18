using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugVector3X : ABSTweenPlugin
{
    internal static Type[] ValidPropTypes = new Type[1]
    {
        typeof(Vector3)
    };

    internal static Type[] ValidValueTypes = new Type[1]
    {
        typeof(float)
    };

    protected float TypedStartVal;

    protected float TypedEndVal;

    protected float ChangeVal;

    internal override int pluginId => 1;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.isFrom)
            {
                if (IsRelative)
                    StartVal = TypedStartVal = TypedEndVal + Convert.ToSingle(value);
                else
                    StartVal = TypedStartVal = Convert.ToSingle(value);
            }
            else
            {
                StartVal = value;
                TypedStartVal = ((Vector3)StartVal).x;
            }
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set
        {
            if (TweenObj.isFrom)
            {
                EndVal = value;
                TypedEndVal = ((Vector3)EndVal).x;
            }
            else
                EndVal = TypedEndVal = Convert.ToSingle(value);
        }
    }

    public PlugVector3X(float endVal)
        : base(endVal, false)
    {
    }

    public PlugVector3X(float endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugVector3X(float endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugVector3X(float endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
    }

    public PlugVector3X(float endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
    }

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = ChangeVal / speed;
        if (num < 0.0)
            num = -num;
        return num;
    }

    internal override void Rewind()
    {
        var vector3 = (Vector3)GetValue();
        vector3.x = TypedStartVal;
        SetValue(vector3);
    }

    internal override void Complete()
    {
        var vector3 = (Vector3)GetValue();
        vector3.x = TypedEndVal;
        SetValue(vector3);
    }

    protected override void SetChangeVal()
    {
        if (IsRelative && !TweenObj.isFrom)
        {
            ChangeVal = TypedEndVal;
            endVal = (float)(TypedStartVal + (double)TypedEndVal);
        }
        else
            ChangeVal = TypedEndVal - TypedStartVal;
    }

    protected override void SetIncremental(int diffIncr) => TypedStartVal += ChangeVal * diffIncr;

    protected override void DoUpdate(float totElapsed)
    {
        var vector3 = (Vector3)GetValue();
        vector3.x = Ease(totElapsed, TypedStartVal, ChangeVal, Duration, TweenObj.easeOvershootOrAmplitude,
            TweenObj.easePeriod);
        if (TweenObj.pixelPerfect)
            vector3.x = (int)vector3.x;
        SetValue(vector3);
    }
}

}