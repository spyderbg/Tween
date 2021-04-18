using Holoville.HOTween.Plugins.Core;
using UnityEngine;

namespace Holoville.HOTween.Plugins {

public class PlugQuaternion : ABSTweenPlugin
{
    internal static System.Type[] ValidPropTypes = new System.Type[1]
    {
        typeof(Quaternion)
    };

    internal static readonly System.Type[] ValidValueTypes = new System.Type[2]
    {
        typeof(Vector3),
        typeof(Quaternion)
    };

    private Vector3 _typedStartVal;
    private Vector3 _typedEndVal;
    private Vector3 _changeVal;
    private bool _beyond360;

    protected override object startVal
    {
        get => StartVal;
        set
        {
            if (TweenObj.IsFrom && IsRelative)
            {
                _typedStartVal = _typedEndVal +
                                (value is Quaternion quaternion3 ? quaternion3.eulerAngles : (Vector3)value);
                StartVal = Quaternion.Euler(_typedStartVal);
            }
            else if (value is Quaternion quaternion4)
            {
                StartVal = value;
                _typedStartVal = quaternion4.eulerAngles;
            }
            else
            {
                StartVal = Quaternion.Euler((Vector3)value);
                _typedStartVal = (Vector3)value;
            }
        }
    }

    protected override object endVal
    {
        get => EndVal;
        set
        {
            if (value is Quaternion quaternion)
            {
                EndVal = value;
                _typedEndVal = quaternion.eulerAngles;
            }
            else
            {
                EndVal = Quaternion.Euler((Vector3)value);
                _typedEndVal = (Vector3)value;
            }
        }
    }

    public PlugQuaternion(Quaternion endVal)
        : base(endVal, false)
    {
    }

    public PlugQuaternion(Quaternion endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugQuaternion(Quaternion endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugQuaternion(Quaternion endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
    }

    public PlugQuaternion(Vector3 endVal)
        : base(endVal, false)
    {
    }

    public PlugQuaternion(Vector3 endVal, EaseType easeType)
        : base(endVal, easeType, false)
    {
    }

    public PlugQuaternion(Vector3 endVal, bool isRelative)
        : base(endVal, isRelative)
    {
    }

    public PlugQuaternion(Vector3 endVal, EaseType easeType, bool isRelative)
        : base(endVal, easeType, isRelative)
    {
    }

    public PlugQuaternion(Vector3 endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
    }

    public PlugQuaternion(Quaternion endVal, AnimationCurve easeAnimCurve, bool isRelative)
        : base(endVal, easeAnimCurve, isRelative)
    {
    }

    public PlugQuaternion Beyond360() => Beyond360(true);

    public PlugQuaternion Beyond360(bool beyond360)
    {
        this._beyond360 = beyond360;
        return this;
    }

    protected override float GetSpeedBasedDuration(float speed)
    {
        var num = _changeVal.magnitude / (speed * 360f);
        if (num < 0.0)
            num = -num;
        return num;
    }

    protected override void SetChangeVal()
    {
        if (IsRelative && !TweenObj.IsFrom)
        {
            _changeVal = _typedEndVal;
            endVal = _typedStartVal + _typedEndVal;
        }
        else if (_beyond360)
        {
            _changeVal = _typedEndVal - _typedStartVal;
        }
        else
        {
            var typedEndVal = this._typedEndVal;
            if (typedEndVal.x > 360.0)
                typedEndVal.x %= 360f;
            if (typedEndVal.y > 360.0)
                typedEndVal.y %= 360f;
            if (typedEndVal.z > 360.0)
                typedEndVal.z %= 360f;
            _changeVal = typedEndVal - _typedStartVal;
            var num1 = _changeVal.x > 0.0 ? _changeVal.x : -_changeVal.x;
            if (num1 > 180.0)
                _changeVal.x = _changeVal.x > 0.0 ? (float)-(360.0 - num1) : 360f - num1;
            var num2 = _changeVal.y > 0.0 ? _changeVal.y : -_changeVal.y;
            if (num2 > 180.0)
                _changeVal.y = _changeVal.y > 0.0 ? (float)-(360.0 - num2) : 360f - num2;
            var num3 = _changeVal.z > 0.0 ? _changeVal.z : -_changeVal.z;
            if (num3 <= 180.0)
                return;
            _changeVal.z = _changeVal.z > 0.0 ? (float)-(360.0 - num3) : 360f - num3;
        }
    }

    protected override void SetIncremental(int diffIncr) => _typedStartVal += _changeVal * diffIncr;

    protected override void DoUpdate(float totElapsed)
    {
        var num = Ease(totElapsed, 0.0f, 1f, Duration, TweenObj.EaseOvershootOrAmplitude, TweenObj.EasePeriod);
        SetValue(Quaternion.Euler(new Vector3(_typedStartVal.x + _changeVal.x * num,
            _typedStartVal.y + _changeVal.y * num, _typedStartVal.z + _changeVal.z * num)));
    }
}

}