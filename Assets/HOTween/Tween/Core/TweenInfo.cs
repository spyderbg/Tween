using System.Collections.Generic;

namespace Holoville.HOTween.Core
{
    /// <summary>
    /// Used by <see cref="M:Holoville.HOTween.HOTween.GetTweenInfos" /> and HOTweenInspector,
    /// to store info about tweens that can be displayed.
    /// </summary>
    public class TweenInfo
    {
        /// <summary>Tween.</summary>
        public ABSTweenComponent tween;

        /// <summary>Is sequence.</summary>
        public bool isSequence;

        /// <summary>Targets.</summary>
        public List<object> targets;

        /// <summary>Is paused.</summary>
        public bool isPaused => tween.isPaused;

        /// <summary>Is complete.</summary>
        public bool isComplete => tween.isComplete;

        /// <summary>Is enabled.</summary>
        public bool isEnabled => tween.enabled;

        /// <summary>Creates a new TweenInfo object.</summary>
        public TweenInfo(ABSTweenComponent tween)
        {
            this.tween = tween;
            isSequence = tween is Sequence;
            targets = tween.GetTweenTargets();
        }
    }
}