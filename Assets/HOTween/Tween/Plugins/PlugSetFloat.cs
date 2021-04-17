using Holoville.HOTween.Plugins.Core;
using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Plugin for the tweening of the float of your choice.
    /// Used for changing material floats.
    /// Target for this tween must be of type <see cref="T:UnityEngine.Material" />.
    /// </summary>
    public class PlugSetFloat : ABSTweenPlugin
    {
        internal static Type[] validTargetTypes = new Type[1]
        {
            typeof(Material)
        };

        internal static Type[] validPropTypes = new Type[1]
        {
            typeof(Color)
        };

        internal static Type[] validValueTypes = new Type[1]
        {
            typeof(float)
        };

        private float typedStartVal;
        private float typedEndVal;
        private float changeVal;
        private string floatName;

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
                    _startVal = typedStartVal = typedEndVal + Convert.ToSingle(value);
                else
                    _startVal = typedStartVal = Convert.ToSingle(value);
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = Convert.ToSingle(value);
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Single" /> value to tween to.
        /// </param>
        public PlugSetFloat(float p_endVal)
            : this(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Single" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugSetFloat(float p_endVal, EaseType p_easeType)
            : this(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Single" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugSetFloat(float p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
            ignoreAccessor = true;
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:System.Single" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugSetFloat(float p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
            ignoreAccessor = true;
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
        public PlugSetFloat(float p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
            ignoreAccessor = true;
        }

        /// <summary>Selects the color property to change.</summary>
        /// <param name="p_propertyName">
        /// The propertyName/floatName to change (see Unity's <see cref="M:UnityEngine.Material.SetFloat(System.String,System.Single)" /> if you don't know how it works).
        /// </param>
        public PlugSetFloat Property(string p_propertyName)
        {
            floatName = p_propertyName;
            return this;
        }

        /// <summary>
        /// Overridden by plugins that need a specific type of target, to check it and validate it.
        /// Returns <c>true</c> if the tween target is valid.
        /// </summary>
        internal override bool ValidateTarget(object p_target) => p_target is Material;

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed) => SetValue(ease(p_totElapsed, typedStartVal, changeVal,
            _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod));

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

        /// <summary>
        /// Sets the value of the controlled property.
        /// Some plugins (like PlugSetColor) might override this to get values from different properties.
        /// </summary>
        /// <param name="p_value">The new value.</param>
        protected override void SetValue(object p_value) =>
            ((Material)tweenObj.target).SetFloat(floatName, Convert.ToSingle(p_value));

        /// <summary>
        /// Gets the current value of the controlled property.
        /// Some plugins (like PlugSetColor) might override this to set values on different properties.
        /// </summary>
        protected override object GetValue() => ((Material)tweenObj.target).GetFloat(floatName);
    }
}