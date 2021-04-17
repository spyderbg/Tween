﻿using UnityEngine;

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
            get => _startVal;
            set
            {
                if (tweenObj.isFrom && isRelative)
                    _startVal = typedStartVal = typedEndVal + (Color)value;
                else
                    _startVal = typedStartVal = (Color32)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = (Color32)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        public PlugColor32(Color32 p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugColor32(Color32 p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color32" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugColor32(Color32 p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed)
        {
            var num = 1f / p_speed;
            if (num < 0.0)
                num = -num;
            return num;
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (isRelative && !tweenObj.isFrom)
                typedEndVal = typedStartVal + typedEndVal;
            diffChangeVal = typedEndVal - typedStartVal;
        }

        /// <summary>
        /// Sets the correct values in case of Incremental loop type.
        /// </summary>
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int p_diffIncr)
        {
            typedStartVal += diffChangeVal * p_diffIncr;
            typedEndVal += diffChangeVal * p_diffIncr;
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            var num = ease(p_totElapsed, 0.0f, 1f, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod);
            SetValue((Color32)new Color(typedStartVal.r + diffChangeVal.r * num,
                typedStartVal.g + diffChangeVal.g * num, typedStartVal.b + diffChangeVal.b * num,
                typedStartVal.a + diffChangeVal.a * num));
        }
    }
}