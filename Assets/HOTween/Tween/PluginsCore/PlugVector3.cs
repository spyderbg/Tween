using UnityEngine;

namespace Holoville.HOTween.Plugins.Core
{
    /// <summary>Default plugin for the tweening of Vector3 objects.</summary>
    public class PlugVector3 : ABSTweenPlugin
    {
        internal static System.Type[] validPropTypes = new System.Type[1]
        {
            typeof(Vector3)
        };

        internal static System.Type[] validValueTypes = new System.Type[1]
        {
            typeof(Vector3)
        };

        private Vector3 typedStartVal;
        private Vector3 typedEndVal;
        private Vector3 changeVal;

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected override object startVal
        {
            get => StartVal;
            set
            {
                if (TweenObj.IsFrom && IsRelative)
                    StartVal = typedStartVal = typedEndVal + (Vector3)value;
                else
                    StartVal = typedStartVal = (Vector3)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => EndVal;
            set => EndVal = typedEndVal = (Vector3)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        public PlugVector3(Vector3 endVal)
            : base(endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugVector3(Vector3 endVal, EaseType easeType)
            : base(endVal, easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 endVal, bool isRelative)
            : base(endVal, isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 endVal, EaseType easeType, bool isRelative)
            : base(endVal, easeType, isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 endVal, AnimationCurve easeAnimCurve, bool isRelative)
            : base(endVal, easeAnimCurve, isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float speed)
        {
            var num = changeVal.magnitude / speed;
            if (num < 0.0)
                num = -num;
            return num;
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (IsRelative && !TweenObj.IsFrom)
            {
                changeVal = typedEndVal;
                endVal = typedStartVal + typedEndVal;
            }
            else
                changeVal = new Vector3(typedEndVal.x - typedStartVal.x, typedEndVal.y - typedStartVal.y,
                    typedEndVal.z - typedStartVal.z);
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int diffIncr) => typedStartVal += changeVal * diffIncr;

        /// <summary>Updates the tween.</summary>
        /// <param name="totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float totElapsed)
        {
            var num = Ease(totElapsed, 0.0f, 1f, Duration, TweenObj.EaseOvershootOrAmplitude, TweenObj.EasePeriod);
            if (TweenObj.PixelPerfect)
                SetValue(new Vector3((int)(typedStartVal.x + changeVal.x * (double)num),
                    (int)(typedStartVal.y + changeVal.y * (double)num),
                    (int)(typedStartVal.z + changeVal.z * (double)num)));
            else
                SetValue(new Vector3(typedStartVal.x + changeVal.x * num, typedStartVal.y + changeVal.y * num,
                    typedStartVal.z + changeVal.z * num));
        }
    }
}