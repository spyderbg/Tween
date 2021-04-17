using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Plugin for the tweening of only the X value of Vector3 objects.
    /// </summary>
    public class PlugVector3X : ABSTweenPlugin
    {
        internal static Type[] validPropTypes = new Type[1]
        {
            typeof(Vector3)
        };

        internal static Type[] validValueTypes = new Type[1]
        {
            typeof(float)
        };

        /// <summary>Start val.</summary>
        protected float typedStartVal;

        /// <summary>End val.</summary>
        protected float typedEndVal;

        /// <summary>Change val.</summary>
        protected float changeVal;

        internal override int pluginId => 1;

        /// <summary>
        /// Gets the untyped start value,
        /// sets both the untyped and the typed start value.
        /// </summary>
        protected override object startVal
        {
            get => _startVal;
            set
            {
                if (tweenObj.isFrom)
                {
                    if (isRelative)
                        _startVal = typedStartVal = typedEndVal + Convert.ToSingle(value);
                    else
                        _startVal = typedStartVal = Convert.ToSingle(value);
                }
                else
                {
                    _startVal = value;
                    typedStartVal = ((Vector3)_startVal).x;
                }
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set
            {
                if (tweenObj.isFrom)
                {
                    _endVal = value;
                    typedEndVal = ((Vector3)_endVal).x;
                }
                else
                    _endVal = typedEndVal = Convert.ToSingle(value);
            }
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">The value to tween to.</param>
        public PlugVector3X(float p_endVal)
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
        public PlugVector3X(float p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">The value to tween to.</param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3X(float p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">The value to tween to.</param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3X(float p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Single" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugVector3X(float p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
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
        /// Rewinds the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal override void Rewind()
        {
            var vector3 = (Vector3)GetValue();
            vector3.x = typedStartVal;
            SetValue(vector3);
        }

        /// <summary>
        /// Completes the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal override void Complete()
        {
            var vector3 = (Vector3)GetValue();
            vector3.x = typedEndVal;
            SetValue(vector3);
        }

        /// <summary>
        /// Sets the typed changeVal based on the current startVal and endVal.
        /// </summary>
        protected override void SetChangeVal()
        {
            if (isRelative && !tweenObj.isFrom)
            {
                changeVal = typedEndVal;
                endVal = (float)(typedStartVal + (double)typedEndVal);
            }
            else
                changeVal = typedEndVal - typedStartVal;
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
            var vector3 = (Vector3)GetValue();
            vector3.x = ease(p_totElapsed, typedStartVal, changeVal, _duration, tweenObj.easeOvershootOrAmplitude,
                tweenObj.easePeriod);
            if (tweenObj.pixelPerfect)
                vector3.x = (int)vector3.x;
            SetValue(vector3);
        }
    }
}