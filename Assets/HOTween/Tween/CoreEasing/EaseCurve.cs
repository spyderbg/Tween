using UnityEngine;

namespace Holoville.HOTween.Core.Easing
{
    /// <summary>Used to interpret AnimationCurves as eases.</summary>
    internal class EaseCurve
    {
        private AnimationCurve animCurve;

        public EaseCurve(AnimationCurve p_animCurve) => animCurve = p_animCurve;

        public float Evaluate(
            float time,
            float startValue,
            float changeValue,
            float duration,
            float unusedOvershoot,
            float unusedPeriod)
        {
            var time1 = animCurve[animCurve.length - 1].time;
            var num = animCurve.Evaluate(time / duration * time1);
            return changeValue * num + startValue;
        }
    }
}