using Holoville.HOTween.Plugins.Core;

namespace Holoville.HOTween {

public class TweenEvent
{
    private readonly IHOTweenComponent _tween;
    private readonly object[] _parms;
    private readonly ABSTweenPlugin _plugin;

    public IHOTweenComponent tween => _tween;

    public object[] parms => _parms;

    public ABSTweenPlugin plugin => _plugin;

    internal TweenEvent(IHOTweenComponent tween, object[] parms)
    {
        _tween = tween;
        _parms = parms;
        _plugin = null;
    }

    internal TweenEvent(IHOTweenComponent tween, object[] parms, ABSTweenPlugin plugin)
    {
        _tween = tween;
        _parms = parms;
        _plugin = plugin;
    }
}

}