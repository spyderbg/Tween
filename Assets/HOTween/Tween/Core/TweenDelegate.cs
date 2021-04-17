namespace Holoville.HOTween.Core
{
    /// <summary>Enum of delegates used by HOTween.</summary>
    public static class TweenDelegate
    {
        /// <summary>
        /// Delegate used to store OnEvent (OnStart, OnComplete, etc) functions that will accept a <see cref="T:Holoville.HOTween.TweenEvent" /> parameter.
        /// </summary>
        public delegate void TweenCallbackWParms(TweenEvent p_callbackData);

        /// <summary>
        /// Delegate used to store OnEvent (OnStart, OnComplete, etc) functions without parameters.
        /// </summary>
        public delegate void TweenCallback();

        /// <summary>Delegate used internally for ease functions.</summary>
        public delegate float EaseFunc(
            float elapsed,
            float startValue,
            float changeValue,
            float duration,
            float overshootOrAmplitude,
            float period);

        internal delegate void FilterFunc(int p_index, bool p_optionalBool);

        /// <summary>
        /// Used in place of <c>System.Func</c>, which is not available in mscorlib.
        /// </summary>
        public delegate T HOFunc<out T>();

        /// <summary>
        /// Used in place of <c>System.Action</c>.
        /// </summary>
        public delegate void HOAction<in T>(T p_newValue);
    }
}