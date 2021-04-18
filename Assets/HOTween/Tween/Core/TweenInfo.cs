using System.Collections.Generic;

namespace Holoville.HOTween.Core {

public class TweenInfo
{
    public ABSTweenComponent tween;

    public bool isSequence;

    public List<object> targets;

    public bool isPaused => tween.isPaused;

    public bool isComplete => tween.isComplete;

    public bool isEnabled => tween.enabled;

    public TweenInfo(ABSTweenComponent tween)
    {
        this.tween = tween;
        isSequence = tween is Sequence;
        targets = tween.GetTweenTargets();
    }
}

}