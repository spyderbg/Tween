namespace Holoville.HOTween.Core {

public static class TweenDelegate
{
    public delegate void TweenCallbackWParms(TweenEvent callbackData);

    public delegate void TweenCallback();

    public delegate float EaseFunc(float elapsed, float startValue, float changeValue, float duration, float overshootOrAmplitude, float period);

    internal delegate void FilterFunc(int index, bool optionalBool);

    public delegate T HOFunc<out T>();

    public delegate void HOAction<in T>(T newValue);
}

}