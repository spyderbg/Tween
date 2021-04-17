using Holoville.HOTween.Plugins.Core;

namespace Holoville.HOTween
{
    /// <summary>
    /// This object is passed as the only parameter of all HOTween's callbacks.
    /// </summary>
    public class TweenEvent
    {
        private readonly IHOTweenComponent _tween;
        private readonly object[] _parms;
        private readonly ABSTweenPlugin _plugin;

        /// <summary>
        /// A reference to the IHOTweenComponent that invoked the callback method.
        /// </summary>
        public IHOTweenComponent tween => _tween;

        /// <summary>
        /// An array of eventual parameters that were passed to the callback.
        /// </summary>
        public object[] parms => _parms;

        /// <summary>The plugin (if any) that triggered the callback.</summary>
        public ABSTweenPlugin plugin => _plugin;

        internal TweenEvent(IHOTweenComponent tween, object[] parms)
        {
            _tween = tween;
            _parms = parms;
            _plugin = null;
        }

        internal TweenEvent(IHOTweenComponent tween, object[] parms, ABSTweenPlugin plugin)
        {
            _tween = tween;
            _parms = parms;
            _plugin = plugin;
        }
    }
}