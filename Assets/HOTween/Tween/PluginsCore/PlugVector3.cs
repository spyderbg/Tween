﻿using UnityEngine;

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
            get => _startVal;
            set
            {
                if (tweenObj.isFrom && isRelative)
                    _startVal = typedStartVal = typedEndVal + (Vector3)value;
                else
                    _startVal = typedStartVal = (Vector3)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = (Vector3)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        public PlugVector3(Vector3 p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugVector3(Vector3 p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3(Vector3 p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed)
        {
            var num = changeVal.magnitude / p_speed;
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
        /// <param name="p_diffIncr">
        /// The difference from the previous loop increment.
        /// </param>
        protected override void SetIncremental(int p_diffIncr) => typedStartVal += changeVal * p_diffIncr;

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            var num = ease(p_totElapsed, 0.0f, 1f, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod);
            if (tweenObj.pixelPerfect)
                SetValue(new Vector3((int)(typedStartVal.x + changeVal.x * (double)num),
                    (int)(typedStartVal.y + changeVal.y * (double)num),
                    (int)(typedStartVal.z + changeVal.z * (double)num)));
            else
                SetValue(new Vector3(typedStartVal.x + changeVal.x * num, typedStartVal.y + changeVal.y * num,
                    typedStartVal.z + changeVal.z * num));
        }
    }
}