using Holoville.HOTween.Plugins.Core;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Plugin for the tweening of the color of your choice.
    /// Used for changing material colors different from the default one (like _SpecColor or _Emission).
    /// Target for this tween must be of type <see cref="T:UnityEngine.Material" />.
    /// </summary>
    public class PlugSetColor : ABSTweenPlugin
    {
        internal static System.Type[] validTargetTypes = new System.Type[1]
        {
            typeof(Material)
        };

        internal static System.Type[] validPropTypes = new System.Type[1]
        {
            typeof(Color)
        };

        internal static System.Type[] validValueTypes = new System.Type[1]
        {
            typeof(Color)
        };

        private Color typedStartVal;
        private Color typedEndVal;
        private float changeVal;
        private Color diffChangeVal;
        private string colorName;

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
                    _startVal = typedStartVal = (Color)value;
            }
        }

        /// <summary>
        /// Gets the untyped end value,
        /// sets both the untyped and the typed end value.
        /// </summary>
        protected override object endVal
        {
            get => _endVal;
            set => _endVal = typedEndVal = (Color)value;
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color" /> value to tween to.
        /// </param>
        public PlugSetColor(Color p_endVal)
            : this(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugSetColor(Color p_endVal, EaseType p_easeType)
            : this(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugSetColor(Color p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
            ignoreAccessor = true;
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugSetColor(Color p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
            ignoreAccessor = true;
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Color" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugSetColor(Color p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
            ignoreAccessor = true;
        }

        /// <summary>Selects the color property to change.</summary>
        /// <param name="p_colorName">
        /// The propertyName/colorName to change (see Unity's <see cref="M:UnityEngine.Material.SetColor(System.String,UnityEngine.Color)" /> if you don't know how it works),
        /// set via the <see cref="T:Holoville.HOTween.Plugins.PlugSetColor.ColorName" /> enum.
        /// </param>
        public PlugSetColor Property(ColorName p_colorName) => Property(p_colorName.ToString());

        /// <summary>Selects the color property to change.</summary>
        /// <param name="p_propertyName">
        /// The propertyName/colorName to change (see Unity's <see cref="M:UnityEngine.Material.SetColor(System.String,UnityEngine.Color)" /> if you don't know how it works).
        /// </param>
        public PlugSetColor Property(string p_propertyName)
        {
            colorName = p_propertyName;
            return this;
        }

        /// <summary>
        /// Overridden by plugins that need a specific type of target, to check it and validate it.
        /// Returns <c>true</c> if the tween target is valid.
        /// </summary>
        internal override bool ValidateTarget(object p_target) => p_target is Material;

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed) => SetValue(Color.Lerp(typedStartVal, typedEndVal,
            ease(p_totElapsed, 0.0f, changeVal, _duration, tweenObj.easeOvershootOrAmplitude, tweenObj.easePeriod)));

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
            changeVal = 1f;
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

        /// <summary>
        /// Sets the value of the controlled property.
        /// Some plugins (like PlugSetColor) might override this to get values from different properties.
        /// </summary>
        /// <param name="p_value">The new value.</param>
        protected override void SetValue(object p_value) =>
            ((Material)tweenObj.target).SetColor(colorName, (Color)p_value);

        /// <summary>
        /// Gets the current value of the controlled property.
        /// Some plugins (like PlugSetColor) might override this to set values on different properties.
        /// </summary>
        protected override object GetValue() => ((Material)tweenObj.target).GetColor(colorName);

        /// <summary>Enumeration of color properties names.</summary>
        public enum ColorName
        {
            /// <summary>Main color of a material.</summary>
            _Color,

            /// <summary>
            /// Specular color of a material (used in specular/glossy/vertexlit shaders).
            /// </summary>
            _SpecColor,

            /// <summary>
            /// Emissive color of a material (used in vertexlit shaders).
            /// </summary>
            _Emission,

            /// <summary>
            /// Reflection color of a material (used in reflective shaders).
            /// </summary>
            _ReflectColor,
        }
    }
}