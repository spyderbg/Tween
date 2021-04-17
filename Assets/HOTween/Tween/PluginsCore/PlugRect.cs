using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins.Core
{
    /// <summary>Default plugin for the tweening of Rect objects.</summary>
    public class PlugRect : ABSTweenPlugin
    {
        internal static Type[] validPropTypes = new Type[1]
        {
            typeof(Rect)
        };

        internal static Type[] validValueTypes = new Type[1]
        {
            typeof(Rect)
        };

        private Rect typedStartVal;
        private Rect typedEndVal;
        private Rect diffChangeVal;

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected override object startVal
        {
            get => _startVal;
            set
            {
                if (tweenObj.isFrom && isRelative)
                {
                    typedStartVal = (Rect)value;
                    typedStartVal.x += typedEndVal.x;
                    typedStartVal.y += typedEndVal.y;
                    typedStartVal.width += typedEndVal.width;
                    typedStartVal.height += typedEndVal.height;
                    _startVal = typedStartVal;
                }
                else
                    _startVal = typedStartVal = (Rect)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = (Rect)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Rect" /> value to tween to.
        /// </param>
        public PlugRect(Rect p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Rect" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugRect(Rect p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Rect" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugRect(Rect p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Rect" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugRect(Rect p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Rect" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugRect(Rect p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed)
        {
            var num1 = typedEndVal.width - typedStartVal.width;
            var num2 = typedEndVal.height - typedStartVal.height;
            var num3 = (float)Math.Sqrt(num1 * (double)num1 + num2 * (double)num2) / p_speed;
            if (num3 < 0.0)
                num3 = -num3;
            return num3;
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (isRelative && !tweenObj.isFrom)
            {
                typedEndVal.x += typedStartVal.x;
                typedEndVal.y += typedStartVal.y;
                typedEndVal.width += typedStartVal.width;
                typedEndVal.height += typedStartVal.height;
            }

            diffChangeVal = new Rect();
            diffChangeVal.x = typedEndVal.x - typedStartVal.x;
            diffChangeVal.y = typedEndVal.y - typedStartVal.y;
            diffChangeVal.width = typedEndVal.width - typedStartVal.width;
            diffChangeVal.height = typedEndVal.height - typedStartVal.height;
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int p_diffIncr)
        {
            var rect = new Rect(diffChangeVal.x, diffChangeVal.y, diffChangeVal.width, diffChangeVal.height);
            rect.x *= p_diffIncr;
            rect.y *= p_diffIncr;
            rect.width *= p_diffIncr;
            rect.height *= p_diffIncr;
            typedStartVal.x += rect.x;
            typedStartVal.y += rect.y;
            typedStartVal.width += rect.width;
            typedStartVal.height += rect.height;
            typedEndVal.x += rect.x;
            typedEndVal.y += rect.y;
            typedEndVal.width += rect.width;
            typedEndVal.height += rect.height;
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            var num = ease(p_totElapsed, 0.0f, 1f, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod);
            SetValue(new Rect()
            {
                x = (typedStartVal.x + diffChangeVal.x * num),
                y = (typedStartVal.y + diffChangeVal.y * num),
                width = (typedStartVal.width + diffChangeVal.width * num),
                height = (typedStartVal.height + diffChangeVal.height * num)
            });
        }
    }
}