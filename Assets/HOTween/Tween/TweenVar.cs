using Holoville.HOTween.Core;

namespace Holoville.HOTween
{
    /// <summary>
    /// A special class used to setup a "virtual" tween,
    /// which will not actually be tweened nor updated,
    /// but will just set and return a value when you call Update.
    /// </summary>
    public class TweenVar
    {
        /// <summary>
        /// Virtual duration
        /// (you could also set it to 1 or 100 to treat it as a percentage).
        /// </summary>
        public float duration;

        private float _value;
        private float _startVal;
        private float _endVal;
        private EaseType _easeType;
        private float _elapsed;
        private float changeVal;
        private TweenDelegate.EaseFunc ease;

        /// <summary>Start value (FROM).</summary>
        public float startVal
        {
            get => _startVal;
            set
            {
                _startVal = value;
                SetChangeVal();
            }
        }

        /// <summary>End value (TO).</summary>
        public float endVal
        {
            get => _endVal;
            set
            {
                _endVal = value;
                SetChangeVal();
            }
        }

        /// <summary>Ease type.</summary>
        public EaseType easeType
        {
            get => _easeType;
            set
            {
                _easeType = value;
                ease = EaseInfo.GetEaseInfo(_easeType).ease;
            }
        }

        /// <summary>
        /// The current value of this <see cref="T:Holoville.HOTween.TweenVar" />
        /// </summary>
        public float value => _value;

        /// <summary>The current elapsed time.</summary>
        public float elapsed => _elapsed;

        /// <summary>Creates a new TweenVar instance using Linear ease.</summary>
        /// <param name="p_startVal">Start value (FROM).</param>
        /// <param name="p_endVal">End value (TO).</param>
        /// <param name="p_duration">
        /// Virtual duration.
        /// (you could also set it to 1 or 100 to treat it as a percentage).
        /// </param>
        public TweenVar(float p_startVal, float p_endVal, float p_duration)
            : this(p_startVal, p_endVal, p_duration, EaseType.Linear)
        {
        }

        /// <summary>Creates a new TweenVar instance.</summary>
        /// <param name="p_startVal">Start value (FROM).</param>
        /// <param name="p_endVal">End value (TO).</param>
        /// <param name="p_duration">
        /// Virtual duration.
        /// (you could also set it to 1 or 100 to treat it as a percentage).
        /// </param>
        /// <param name="p_easeType">Ease type.</param>
        public TweenVar(float p_startVal, float p_endVal, float p_duration, EaseType p_easeType)
        {
            startVal = _value = p_startVal;
            endVal = p_endVal;
            duration = p_duration;
            easeType = p_easeType;
        }

        /// <summary>
        /// Sets and returns the value at which this <see cref="T:Holoville.HOTween.TweenVar" />
        /// would be after the given absolute time.
        /// </summary>
        /// <param name="p_elapsed">The elapsed time to calculate.</param>
        public float Update(float p_elapsed) => Update(p_elapsed, false);

        /// <summary>
        /// Sets and returns the value at which this <see cref="T:Holoville.HOTween.TweenVar" />
        /// would be after the given time.
        /// </summary>
        /// <param name="p_elapsed">The elapsed time to calculate.</param>
        /// <param name="p_relative">
        /// If <c>true</c> consideres p_elapsed as relative,
        /// meaning it will be added to the previous elapsed time,
        /// otherwise it is considered absolute.
        /// </param>
        public float Update(float p_elapsed, bool p_relative)
        {
            _elapsed = p_relative ? _elapsed + p_elapsed : p_elapsed;
            if (_elapsed > (double)duration)
                _elapsed = duration;
            else if (_elapsed < 0.0)
                _elapsed = 0.0f;
            _value = ease(_elapsed, _startVal, changeVal, duration, HOTween.defEaseOvershootOrAmplitude,
                HOTween.defEasePeriod);
            return _value;
        }

        private void SetChangeVal() => changeVal = endVal - startVal;
    }
}