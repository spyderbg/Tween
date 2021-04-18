using UnityEngine;

namespace Holoville.HOTween.Plugins.Core
{
    /// <summary>Default plugin for the tweening of Color32 objects.</summary>
    public class PlugColor32 : ABSTweenPlugin
    {
        internal static System.Type[] validPropTypes = new System.Type[1]
        {
            typeof(Color32)
        };

        internal static System.Type[] validValueTypes = new System.Type[1]
        {
            typeof(Color32)
        };

        private Color typedStartVal;
        private Color typedEndVal;
        private Color diffChangeVal;

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected override object startVal
        {
            get => StartVal;
            set
            {
                if (TweenObj.isFrom && IsRelative)
                    StartVal = typedStartVal = typedEndVal + (Color)value;
                else
                    StartVal = typedStartVal = (Color32)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => EndVal;
            set => EndVal = typedEndVal = (Color32)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        public PlugColor32(Color32 endVal)
            : base(endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugColor32(Color32 endVal, EaseType easeType)
            : base(endVal, easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 endVal, bool isRelative)
            : base(endVal, isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 endVal, EaseType easeType, bool isRelative)
            : base(endVal, easeType, isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 endVal, AnimationCurve easeAnimCurve, bool isRelative)
            : base(endVal, easeAnimCurve, isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float speed)
        {
            var num = 1f / speed;
            if (num < 0.0)
                num = -num;
            return num;
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (IsRelative && !TweenObj.isFrom)
                typedEndVal = typedStartVal + typedEndVal;
            diffChangeVal = typedEndVal - typedStartVal;
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int diffIncr)
        {
            typedStartVal += diffChangeVal * diffIncr;
            typedEndVal += diffChangeVal * diffIncr;
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float totElapsed)
        {
            var num = Ease(totElapsed, 0.0f, 1f, Duration, TweenObj.easeOvershootOrAmplitude, TweenObj.easePeriod);
            SetValue((Color32)new Color(typedStartVal.r + diffChangeVal.r * num,
                typedStartVal.g + diffChangeVal.g * num, typedStartVal.b + diffChangeVal.b * num,
                typedStartVal.a + diffChangeVal.a * num));
        }
    }
}