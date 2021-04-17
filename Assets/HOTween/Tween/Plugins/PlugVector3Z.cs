using System;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Plugin for the tweening of only the Z value of Vector3 objects.
    /// </summary>
    public class PlugVector3Z : PlugVector3X
    {
        internal override int pluginId => 3;

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
                    typedStartVal = ((Vector3)_startVal).z;
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
                    typedEndVal = ((Vector3)_endVal).z;
                }
                else
                    _endVal = typedEndVal = Convert.ToSingle(value);
            }
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">The value to tween to.</param>
        public PlugVector3Z(float p_endVal)
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
        public PlugVector3Z(float p_endVal, EaseType p_easeType)
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
        public PlugVector3Z(float p_endVal, bool p_isRelative)
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
        public PlugVector3Z(float p_endVal, EaseType p_easeType, bool p_isRelative)
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
        public PlugVector3Z(float p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Rewinds the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal override void Rewind()
        {
            var vector3 = (Vector3)GetValue();
            vector3.z = typedStartVal;
            SetValue(vector3);
        }

        /// <summary>
        /// Completes the tween.
        /// Should be overriden by tweens that control only part of the property (like HOTPluginVector3X).
        /// </summary>
        internal override void Complete()
        {
            var vector3 = (Vector3)GetValue();
            vector3.z = typedEndVal;
            SetValue(vector3);
        }

        /// <summary>Updates the tween.</summary>
        /// <param name="p_totElapsed">The total elapsed time since startup.</param>
        protected override void DoUpdate(float p_totElapsed)
        {
            var vector3 = (Vector3)GetValue();
            vector3.z = ease(p_totElapsed, typedStartVal, changeVal, _duration, tweenObj.easeOvershootOrAmplitude,
                tweenObj.easePeriod);
            if (tweenObj.pixelPerfect)
                vector3.z = (int)vector3.z;
            SetValue(vector3);
        }
    }
}