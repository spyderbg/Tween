using Holoville.HOTween.Core;

namespace Holoville.HOTween {

public class TweenVar
{
    public float duration;

    private float _value;
    private float _startVal;
    private float _endVal;
    private EaseType _easeType;
    private float _elapsed;
    private float changeVal;
    private TweenDelegate.EaseFunc ease;

    public float startVal
    {
        get => _startVal;
        set
        {
            _startVal = value;
            SetChangeVal();
        }
    }

    public float endVal
    {
        get => _endVal;
        set
        {
            _endVal = value;
            SetChangeVal();
        }
    }

    public EaseType easeType
    {
        get => _easeType;
        set
        {
            _easeType = value;
            ease = EaseInfo.GetEaseInfo(_easeType).Ease;
        }
    }

    public float value => _value;

    public float elapsed => _elapsed;

    public TweenVar(float startVal, float endVal, float duration)
        : this(startVal, endVal, duration, EaseType.Linear)
    {
    }

    public TweenVar(float startVal, float endVal, float duration, EaseType easeType)
    {
        this.startVal = _value = startVal;
        this.endVal = endVal;
        this.duration = duration;
        this.easeType = easeType;
    }

    public float Update(float elapsed) => Update(elapsed, false);

    public float Update(float elapsed, bool relative)
    {
        _elapsed = relative ? _elapsed + elapsed : elapsed;
        if (_elapsed > (double)duration)
            _elapsed = duration;
        else if (_elapsed < 0.0)
            _elapsed = 0.0f;
        _value = ease(_elapsed, _startVal, changeVal, duration, HOTween.kDefEaseOvershootOrAmplitude,
            HOTween.kDefEasePeriod);
        return _value;
    }

    private void SetChangeVal() => changeVal = endVal - startVal;
}

}