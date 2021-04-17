using Holoville.HOTween.Plugins.Core;
using UnityEngine;

namespace Holoville.HOTween.Plugins
{
    /// <summary>
    /// Default plugin for the tweening of Quaternion objects.
    /// </summary>
    public class PlugQuaternion : ABSTweenPlugin
    {
        internal static System.Type[] validPropTypes = new System.Type[1]
        {
            typeof(Quaternion)
        };

        internal static System.Type[] validValueTypes = new System.Type[2]
        {
            typeof(Vector3),
            typeof(Quaternion)
        };

        private Vector3 typedStartVal;
        private Vector3 typedEndVal;
        private Vector3 changeVal;
        private bool beyond360;

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
                    typedStartVal = typedEndVal +
                                    (value is Quaternion quaternion3 ? quaternion3.eulerAngles : (Vector3)value);
                    _startVal = Quaternion.Euler(typedStartVal);
                }
                else if (value is Quaternion quaternion4)
                {
                    _startVal = value;
                    typedStartVal = quaternion4.eulerAngles;
                }
                else
                {
                    _startVal = Quaternion.Euler((Vector3)value);
                    typedStartVal = (Vector3)value;
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
                if (value is Quaternion quaternion)
                {
                    _endVal = value;
                    typedEndVal = quaternion.eulerAngles;
                }
                else
                {
                    _endVal = Quaternion.Euler((Vector3)value);
                    typedEndVal = (Vector3)value;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Quaternion" /> value to tween to.
        /// </param>
        public PlugQuaternion(Quaternion p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Quaternion" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugQuaternion(Quaternion p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Quaternion" /> value to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugQuaternion(Quaternion p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Quaternion" /> value to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugQuaternion(Quaternion p_endVal, EaseType p_easeType, bool p_isRelative)
            : base(p_endVal, p_easeType, p_isRelative)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> euler angles to tween to.
        /// </param>
        public PlugQuaternion(Vector3 p_endVal)
            : base(p_endVal, false)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> euler angles to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public PlugQuaternion(Vector3 p_endVal, EaseType p_easeType)
            : base(p_endVal, p_easeType, false)
        {
        }

        /// <summary>
        /// Creates a new instance of this plugin using the main ease type.
        /// </summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> euler angles to tween to.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugQuaternion(Vector3 p_endVal, bool p_isRelative)
            : base(p_endVal, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Vector3" /> euler angles to tween to.
        /// </param>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugQuaternion(Vector3 p_endVal, EaseType p_easeType, bool p_isRelative)
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
        public PlugQuaternion(Vector3 p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>Creates a new instance of this plugin.</summary>
        /// <param name="p_endVal">
        /// The <see cref="T:UnityEngine.Quaternion" /> value to tween to.
        /// </param>
        /// <param name="p_easeAnimCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use for easing.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c>, the given end value is considered relative instead than absolute.
        /// </param>
        public PlugQuaternion(Quaternion p_endVal, AnimationCurve p_easeAnimCurve, bool p_isRelative)
            : base(p_endVal, p_easeAnimCurve, p_isRelative)
        {
        }

        /// <summary>
        /// Parameter &gt; Sets rotations to be calculated fully,
        /// and the end value will be reached using the full degrees of the given rotation, even if beyond 360 degrees.
        /// </summary>
        public PlugQuaternion Beyond360() => Beyond360(true);

        /// <summary>
        /// Parameter &gt; Choose whether you want to calculate angles bigger than 360 degrees or not.
        /// In the first case, the end value will be reached using the full degrees of the given rotation.
        /// In the second case, the end value will be reached from the shortest direction.
        /// If the endValue is set as <c>relative</c>, this option will have no effect, and full beyond 360 rotations will always be used.
        /// </summary>
        /// <param name="p_beyond360">
        /// Set to <c>true</c> to use angles bigger than 360 degrees.
        /// </param>
        public PlugQuaternion Beyond360(bool p_beyond360)
        {
            beyond360 = p_beyond360;
            return this;
        }

        /// <summary>
        /// Returns the speed-based duration based on the given speed x second.
        /// </summary>
        protected override float GetSpeedBasedDuration(float p_speed)
        {
            var num = changeVal.magnitude / (p_speed * 360f);
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
            else if (beyond360)
            {
                changeVal = typedEndVal - typedStartVal;
            }
            else
            {
                var typedEndVal = this.typedEndVal;
                if (typedEndVal.x > 360.0)
                    typedEndVal.x %= 360f;
                if (typedEndVal.y > 360.0)
                    typedEndVal.y %= 360f;
                if (typedEndVal.z > 360.0)
                    typedEndVal.z %= 360f;
                changeVal = typedEndVal - typedStartVal;
                var num1 = changeVal.x > 0.0 ? changeVal.x : -changeVal.x;
                if (num1 > 180.0)
                    changeVal.x = changeVal.x > 0.0 ? (float)-(360.0 - num1) : 360f - num1;
                var num2 = changeVal.y > 0.0 ? changeVal.y : -changeVal.y;
                if (num2 > 180.0)
                    changeVal.y = changeVal.y > 0.0 ? (float)-(360.0 - num2) : 360f - num2;
                var num3 = changeVal.z > 0.0 ? changeVal.z : -changeVal.z;
                if (num3 <= 180.0)
                    return;
                changeVal.z = changeVal.z > 0.0 ? (float)-(360.0 - num3) : 360f - num3;
            }
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
            SetValue(Quaternion.Euler(new Vector3(typedStartVal.x + changeVal.x * num,
                typedStartVal.y + changeVal.y * num, typedStartVal.z + changeVal.z * num)));
        }
    }
}