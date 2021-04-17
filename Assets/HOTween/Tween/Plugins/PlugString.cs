using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>Plugin for the tweening of strings.</summary>
    public class PlugString : ABSTweenPlugin
    {
        internal static Type[] validPropTypes = new Type[1]
        {
            typeof(string)
        };

        internal static Type[] validValueTypes = new Type[1]
        {
            typeof(string)
        };

        private string typedStartVal;
        private string typedEndVal;
        private float changeVal;

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
                    _startVal = typedStartVal = typedEndVal + (value == null ? "" : value.ToString());
                else
                    _startVal = typedStartVal = value == null ? "" : value.ToString();
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = value == null ? "" : value.ToString();
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type,
        /// substituting any existing string with the given one over time.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.String" /> value to tween to.
        /// </param>
        public PlugString(string p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin,
        /// substituting any existing string with the given one over time.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.String" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugString(string p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.String" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given value will be added to any existing string,
        /// if <c>false</c> the existing string will be completely overwritten.
        /// </param>
        public PlugString(string p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.String" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given value will be added to any existing string,
        /// if <c>false</c> the existing string will be completely overwritten.
        /// </param>
        public PlugString(string p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.String" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugString(string p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed)
        {
            var num = changeVal / p_speed;
            if (num < 0.0)
                num = -num;
            return num;
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal() => changeVal = typedEndVal.Length;

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int p_diffIncr)
        {
            if (p_diffIncr > 0)
            {
                for (; p_diffIncr > 0; --p_diffIncr)
                    typedStartVal += typedEndVal;
            }
            else
                typedStartVal = typedStartVal.Substring(0, typedStartVal.Length + typedEndVal.Length * p_diffIncr);
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            var num = (int)Math.Round(ease(p_totElapsed, 0.0f, changeVal, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod));
            
            SetValue(!isRelative
                ? typedEndVal.Substring(0, num) + (num >= (double)changeVal || num >= typedStartVal.Length
                    ? ""
                    : typedStartVal.Substring(num))
                : (object)(typedStartVal + typedEndVal.Substring(0, num)));
        }
    }
}