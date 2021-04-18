using System.Collections.Generic;

namespace Holoville.HOTween.Core {

public class TweenInfo
{
    public ABSTweenComponent tween;

    public bool isSequence;

    public List<object> targets;

    public bool isPaused => tween.IsPaused;

    public bool isComplete => tween.IsComplete;

    public bool isEnabled => tween.Enabled;

    public TweenInfo(ABSTweenComponent tween)
    {
        this.tween = tween;
        isSequence = tween is Sequence;
        targets = tween.GetTweenTargets();
    }
}

}