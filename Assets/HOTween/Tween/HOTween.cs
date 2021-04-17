using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoville.HOTween
{
    /// <summary>
    /// Main tween manager.
    /// Controls all tween types (<see cref="T:Holoville.HOTween.Tweener" /> and <see cref="T:Holoville.HOTween.Sequence" />),
    /// and is used to directly create Tweeners (to create Sequences, directly create a new <see cref="T:Holoville.HOTween.Sequence" /> instead).
    /// <para>Author: Daniele Giardini (http://www.holoville.com)</para>
    /// </summary>
    public class HOTween : MonoBehaviour
    {
        /// <summary>HOTween author - me! :P</summary>
        public const string kAuthor = "Daniele Giardini - Holoville";

        private const string kGameobjname = "HOTween";

        /// <summary>HOTween version.</summary>
        public const string kVersion = "1.3.035";

        /// <summary>
        /// Default <see cref="T:Holoville.HOTween.UpdateType" /> that will be used by any new Tweener/Sequence that doesn't implement a specific ease
        /// (default = <c>EaseType.easeOutQuad</c>)
        /// </summary>
        public static UpdateType defUpdateType = UpdateType.Update;

        /// <summary>
        /// Default time scale that will be used by any new Tweener/Sequence that doesn't implement a specific timeScale
        /// (default = <c>1</c>).
        /// </summary>
        public static float defTimeScale = 1f;

        /// <summary>
        /// Default <see cref="T:Holoville.HOTween.EaseType" /> that will be used by any new Tweener/Sequence that doesn't implement a specific ease
        /// (default = <c>EaseType.easeOutQuad</c>).
        /// </summary>
        public static EaseType defEaseType = EaseType.EaseOutQuad;

        /// <summary>Default overshoot to use with Back easeTypes.</summary>
        public static float defEaseOvershootOrAmplitude = 1.70158f;

        /// <summary>Default period to use with Elastic easeTypes.</summary>
        public static float defEasePeriod = 0.0f;

        /// <summary>
        /// Default <see cref="T:Holoville.HOTween.LoopType" /> that will be used by any Tweener/Sequence that doesn't implement a specific loopType
        /// (default = <c>LoopType.Restart</c>).
        /// </summary>
        public static LoopType defLoopType = LoopType.Restart;

        /// <summary>
        /// If <c>true</c>, shows the eventual paths in use by <see cref="T:Holoville.HOTween.Plugins.PlugVector3Path" />
        /// while playing inside Unity's Editor (and if the Editor's Gizmos button is on).
        /// </summary>
        public static bool showPathGizmos;

        /// <summary>
        /// Level of message output in case an error is encountered.
        /// Warnings are logged when HOTween encounters an error, and automatically resolves it without throwing any exception
        /// (like if you try to tween an unexisting property, in which case the tween simply won't be generated,
        /// and an eventual warning will appear in the output window).
        /// </summary>
        public static WarningLevel warningLevel = WarningLevel.Verbose;

        /// <summary>
        /// <c>true</c> if the current player is iOS (iPhone).
        /// Used so simple Reflection instead than unsupported MemberAccessorCacher will be applyed
        /// (iOS doesn't support <c>Reflection.Emit</c>).
        /// </summary>
        internal static bool IsIOS;

        /// <summary>
        /// <c>true</c> if the current player is running in the Editor.
        /// </summary>
        internal static bool IsEditor;

        /// <summary>
        /// Filled by tweens that are completed, so that their onCompleteDispatch method can be called AFTER HOTween has eventually removed them
        /// (otherwise a Kill + To on the same target won't work).
        /// This field is emptied as soon as all onCompletes are called.
        /// </summary>
        internal static List<ABSTweenComponent> onCompletes = new List<ABSTweenComponent>();

        private static bool _initialized;
        private static bool _isPermanent;
        private static bool _renameInstToCountTw;
        private static float _time;
        private static bool _isQuitting;
        private static readonly List<int> TweensToRemoveIndexes = new List<int>();

        /// <summary>Reference to overwrite manager (if in use).</summary>
        internal static OverwriteManager OverwriteManager;

        private static List<ABSTweenComponent> _tweens;
        private static GameObject _tweenGOInstance;
        private static HOTween _it;

        /// <summary>TRUE while inside the update loop</summary>
        internal static bool isUpdateLoop { get; private set; }

        /// <summary>
        /// Total number of tweeners/sequences (paused and delayed ones are included).
        /// Tweeners and sequences contained into other sequences don't count:
        /// for example, if there's only one sequence that contains 2 tweeners, <c>totTweens</c> will be 1.
        /// </summary>
        public static int totTweens =>
            _tweens == null ? 0 : _tweens.Count;

        /// <summary>
        /// Initializes <see cref="T:Holoville.HOTween.HOTween" /> and sets it as non-permanent
        /// (meaning HOTween instance will be destroyed when all tweens are killed,
        /// and re-created when needed).
        /// Call this method once when your application starts up,
        /// to avoid auto-initialization when the first tween is started or created,
        /// and to set options.
        /// </summary>
        public static void Init() => Init(false, true, false);

        /// <summary>
        /// Initializes <see cref="T:Holoville.HOTween.HOTween" />.
        /// Call this method once when your application starts up,
        /// to avoid auto-initialization when the first tween is started or created,
        /// and to set options.
        /// </summary>
        /// <param name="permanentInstance">
        /// If set to <c>true</c>, doesn't destroy HOTween manager when no tween is present,
        /// otherwise the manager is destroyed when all tweens have been killed,
        /// and re-created when needed.
        /// </param>
        public static void Init(bool permanentInstance) =>
            Init(permanentInstance, true, false);

        /// <summary>
        /// Initializes <see cref="T:Holoville.HOTween.HOTween" />.
        /// Call this method once when your application starts up,
        /// to avoid auto-initialization when the first tween is started or created,
        /// and to set options.
        /// </summary>
        /// <param name="permanentInstance">
        /// If set to <c>true</c>, doesn't destroy HOTween manager when no tween is present,
        /// otherwise the manager is destroyed when all tweens have been killed,
        /// and re-created when needed.
        /// </param>
        /// <param name="renameInstanceToCountTweens">
        /// If <c>true</c>, renames HOTween's instance to show
        /// the current number of running tweens (only while in the Editor).
        /// </param>
        /// <param name="allowOverwriteManager">
        /// If <c>true</c>, allows HOTween's instance to enable or disable
        /// the OverwriteManager to improve performance if it is never needed.
        /// </param>
        public static void Init(bool permanentInstance, bool renameInstanceToCountTweens, bool allowOverwriteManager)
        {
            if (_initialized) return;
            
            _initialized = true;
            IsIOS = Application.platform == RuntimePlatform.IPhonePlayer;
            IsEditor = Application.isEditor;
            _isPermanent = permanentInstance;
            _renameInstToCountTw = renameInstanceToCountTweens;
            
            if (allowOverwriteManager)
                OverwriteManager = new OverwriteManager();
                
            if (!_isPermanent || !(_tweenGOInstance == null)) return;
            
            NewTweenInstance();
            SetGOName();
        }

        private void OnApplicationQuit() => _isQuitting = true;

        private void OnDrawGizmos()
        {
            if (_tweens == null || !showPathGizmos) return;
            
            var plugins = GetPlugins();
            var count = plugins.Count;
            for (var index = 0; index < count; ++index)
            {
                if (plugins[index] is PlugVector3Path plugVector3Path1 && plugVector3Path1.path != null)
                    plugVector3Path1.path.GizmoDraw(plugVector3Path1.pathPerc, false);
            }
        }

        private void OnDestroy()
        {
            if (_isQuitting || !(this == _it)) return;
            
            Clear();
        }

        /// <summary>
        /// Called internally each time a new <see cref="T:Holoville.HOTween.Sequence" /> is created.
        /// Adds the given Sequence to the tween list.
        /// </summary>
        /// <param name="sequence">
        /// The <see cref="T:Holoville.HOTween.Sequence" /> to add.
        /// </param>
        internal static void AddSequence(Sequence sequence)
        {
            if (!_initialized)
                Init();
            AddTween(sequence);
        }

        /// <summary>
        /// Creates a new absolute tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="endVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener To(object target, float duration, string propName, object endVal)
        {
            return To(target, duration, new TweenParms().Prop(propName, endVal));
        }

        /// <summary>
        /// Creates a new tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="endVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener To(object target, float duration, string propName, object endVal, bool isRelative)
        {
            return To(target, duration, new TweenParms().Prop(propName, endVal, isRelative));
        }

        /// <summary>
        /// Creates a new tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="endVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <param name="easeType">The ease to use.</param>
        /// <param name="delay">The eventual delay to apply.</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener To( object target, float duration, string propName, object endVal, bool isRelative, EaseType easeType, float delay)
        {
            return To(target, duration, new TweenParms().Prop(propName, endVal, isRelative).Delay(delay).Ease(easeType));
        }

        /// <summary>
        /// Creates a new tween and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="parms">
        /// A <see cref="T:Holoville.HOTween.TweenParms" /> representing the tween parameters.
        /// You can pass an existing one, or create a new one inline via method chaining,
        /// like <c>new TweenParms().Prop("x",10).Loops(2).OnComplete(myFunction)</c>
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener To(object target, float duration, TweenParms parms)
        {
            if (!_initialized)
                Init();
            var tweener = new Tweener(target, duration, parms);
            if (tweener.isEmpty) return null;
            
            AddTween(tweener);
            return tweener;
        }

        /// <summary>
        /// Creates a new absolute FROM tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="fromVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener From(object target, float duration, string propName, object fromVal)
        {
            return From(target, duration, new TweenParms().Prop(propName, fromVal));
        }

        /// <summary>
        /// Creates a new FROM tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="fromVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener From(object target, float duration, string propName, object fromVal, bool isRelative)
        {
            return From(target, duration, new TweenParms().Prop(propName, fromVal, isRelative));
        }

        /// <summary>
        /// Creates a new FROM tween with default values, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">
        /// The name of the property or field to tween.
        /// </param>
        /// <param name="fromVal">
        /// The end value the property should reach with the tween.
        /// </param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <param name="easeType">The ease to use.</param>
        /// <param name="delay">The eventual delay to apply.</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener From(object target, float duration, string propName, object fromVal, bool isRelative, EaseType easeType, float delay)
        {
            return From(target, duration, new TweenParms().Prop(propName, fromVal, isRelative).Delay(delay).Ease(easeType));
        }

        /// <summary>
        /// Creates a new FROM tween and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="parms">
        /// A <see cref="T:Holoville.HOTween.TweenParms" /> representing the tween parameters.
        /// You can pass an existing one, or create a new one inline via method chaining,
        /// like <c>new TweenParms().Prop("x",10).Loops(2).OnComplete(myFunction)</c>
        /// </param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener From(object target, float duration, TweenParms parms)
        {
            if (!_initialized)
                Init();
            parms = parms.IsFrom();
            var tweener = new Tweener(target, duration, parms);
            if (tweener.isEmpty) return null;
            
            AddTween(tweener);
            if (!tweener._isPaused)
                tweener.Update(0.0f, true, true, false, true);
            return tweener;
        }

        /// <summary>
        /// Creates a new absolute PUNCH tween, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">The name of the property or field to tween.</param>
        /// <param name="fromVal">The end value the property should reach with the tween.</param>
        /// <param name="punchAmplitude">Default: 0.5f - amplitude of the punch effect</param>
        /// <param name="punchPeriod">Default: 0.1f - oscillation period of punch effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Punch(object target, float duration, string propName, object fromVal, float punchAmplitude = 0.5f, float punchPeriod = 0.1f)
        {
            var parms = new TweenParms()
                .Prop(propName, fromVal)
                .Ease(EaseType.EaseOutElastic, punchAmplitude, punchPeriod);
            return To(target, duration, parms);
        }

        /// <summary>
        /// Creates a new PUNCH tween, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">The name of the property or field to tween.</param>
        /// <param name="fromVal">The end value the property should reach with the tween.</param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <param name="punchAmplitude">Default: 0.5f - amplitude of the punch effect</param>
        /// <param name="punchPeriod">Default: 0.1f - oscillation period of punch effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Punch(object target, float duration, string propName, object fromVal, bool isRelative, float punchAmplitude = 0.5f, float punchPeriod = 0.1f)
        {
            var parms = new TweenParms()
                .Prop(propName, fromVal, isRelative)
                .Ease(EaseType.EaseOutElastic, punchAmplitude, punchPeriod);
            return To(target, duration, parms);
        }

        /// <summary>
        /// Creates a new PUNCH tween and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// Any ease type passed won't be considered, since punch uses its own one.
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="parms">
        /// A <see cref="T:Holoville.HOTween.TweenParms" /> representing the tween parameters.
        /// You can pass an existing one, or create a new one inline via method chaining,
        /// like <c>new TweenParms().Prop("x",10).Loops(2).OnComplete(myFunction)</c>
        /// </param>
        /// <param name="punchAmplitude">Default: 0.5f - amplitude of the punch effect</param>
        /// <param name="punchPeriod">Default: 0.1f - oscillation period of punch effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Punch(object target, float duration, TweenParms parms, float punchAmplitude = 0.5f, float punchPeriod = 0.1f)
        {
            if (!_initialized)
                Init();
            parms.Ease(EaseType.EaseOutElastic, punchAmplitude, punchPeriod);
            var tweener = new Tweener(target, duration, parms);
            if (tweener.isEmpty) return null;
            
            AddTween(tweener);
            return tweener;
        }

        /// <summary>
        /// Creates a new absolute SHAKE tween, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">The name of the property or field to tween.</param>
        /// <param name="fromVal">The amount of shaking to apply.</param>
        /// <param name="shakeAmplitude">Default: 0.1f - amplitude of the shake effect</param>
        /// <param name="shakePeriod">Default: 0.12f - oscillation period of shake effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Shake( object target, float duration, string propName, object fromVal, float shakeAmplitude = 0.1f, float shakePeriod = 0.12f)
        {
            var parms = new TweenParms()
                .Prop(propName, fromVal)
                .Ease(EaseType.EaseOutElastic, shakeAmplitude, shakePeriod);
            return From(target, duration, parms);
        }

        /// <summary>
        /// Creates a new SHAKE tween, and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="propName">The name of the property or field to tween.</param>
        /// <param name="fromVal">The amount of shaking to apply.</param>
        /// <param name="isRelative">
        /// If <c>true</c> treats the end value as relative (tween BY instead than tween TO), otherwise as absolute.
        /// </param>
        /// <param name="shakeAmplitude">Default: 0.1f - amplitude of the shake effect</param>
        /// <param name="shakePeriod">Default: 0.12f - oscillation period of shake effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Shake(object target, float duration, string propName, object fromVal, bool isRelative, float shakeAmplitude = 0.1f, float shakePeriod = 0.12f)
        {
            var parms = new TweenParms().Prop(propName, fromVal, isRelative)
                .Ease(EaseType.EaseOutElastic, shakeAmplitude, shakePeriod);
            return From(target, duration, parms);
        }

        /// <summary>
        /// Creates a new SHAKE tween and returns the <see cref="T:Holoville.HOTween.Tweener" /> representing it,
        /// or <c>null</c> if the tween was invalid (no valid property to tween was given).
        /// Any ease type passed won't be considered, since shake uses its own one.
        /// </summary>
        /// <param name="target">
        /// The tweening target (must be the object containing the properties or fields to tween).
        /// </param>
        /// <param name="duration">The duration in seconds of the tween.</param>
        /// <param name="parms">
        /// A <see cref="T:Holoville.HOTween.TweenParms" /> representing the tween parameters.
        /// You can pass an existing one, or create a new one inline via method chaining,
        /// like <c>new TweenParms().Prop("x",10).Loops(2).OnComplete(myFunction)</c>
        /// </param>
        /// <param name="shakeAmplitude">Default: 0.1f - amplitude of the shake effect</param>
        /// <param name="shakePeriod">Default: 0.12f - oscillation period of shake effect</param>
        /// <returns>
        /// The newly created <see cref="T:Holoville.HOTween.Tweener" />,
        /// or <c>null</c> if the parameters were invalid.
        /// </returns>
        public static Tweener Shake(object target, float duration, TweenParms parms, float shakeAmplitude = 0.1f, float shakePeriod = 0.12f)
        {
            if (!_initialized)
                Init();
            parms.Ease(EaseType.EaseOutElastic, shakeAmplitude, shakePeriod).IsFrom();
            var tweener = new Tweener(target, duration, parms);
            if (tweener.isEmpty) return null;
            
            AddTween(tweener);
            return tweener;
        }

        /// <summary>Updates normal tweens.</summary>
        private void Update()
        {
            if (_tweens == null)
                return;
            DoUpdate(UpdateType.Update, Time.deltaTime);
            CheckClear();
        }

        /// <summary>Updates lateUpdate tweens.</summary>
        private void LateUpdate()
        {
            if (_tweens == null)
                return;
            DoUpdate(UpdateType.LateUpdate, Time.deltaTime);
            CheckClear();
        }

        /// <summary>Updates fixedUpdate tweens.</summary>
        private void FixedUpdate()
        {
            if (_tweens == null)
                return;
            DoUpdate(UpdateType.FixedUpdate, Time.fixedDeltaTime);
            CheckClear();
        }

        /// <summary>Updates timeScaleIndependent tweens.</summary>
        private static IEnumerator TimeScaleIndependentUpdate()
        {
            while (_tweens != null)
            {
                var elapsed = Time.realtimeSinceStartup - _time;
                _time = Time.realtimeSinceStartup;
                DoUpdate(UpdateType.TimeScaleIndependentUpdate, elapsed);
                if (CheckClear())
                    break;
                yield return null;
            }
        }

        /// <summary>Enables the overwrite manager (disabled by default).</summary>
        /// <param name="logWarnings">If TRUE, the overwriteManager will log a warning each time a tween is overwritten</param>
        public static void EnableOverwriteManager(bool logWarnings = true)
        {
            if (OverwriteManager == null) return;
            
            OverwriteManager.enabled = true;
            OverwriteManager.logWarnings = logWarnings;
        }

        /// <summary>Disables the overwrite manager (disabled by default).</summary>
        public static void DisableOverwriteManager()
        {
            if (OverwriteManager == null) return;
            
            OverwriteManager.enabled = false;
        }

        /// <summary>
        /// Pauses all the tweens for the given target, and returns the total number of paused Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to pause.</param>
        /// <returns>The total number of paused Tweeners.</returns>
        public static int Pause(object target) =>
            DoFilteredIteration(target, DoFilteredPause, false);

        /// <summary>
        /// Pauses all the Tweeners/Sequences with the given ID, and returns the total number of paused Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to pause.</param>
        /// <returns>The total number of paused Tweeners/Sequences.</returns>
        public static int Pause(string id) =>
            DoFilteredIteration(id, DoFilteredPause, false);

        /// <summary>
        /// Pauses all the Tweeners/Sequences with the given intId, and returns the total number of paused Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to pause.
        /// </param>
        /// <returns>The total number of paused Tweeners/Sequences.</returns>
        public static int Pause(int intId) =>
            DoFilteredIteration(intId, DoFilteredPause, false);

        /// <summary>
        /// Pauses the given Tweener, and returns the total number of paused ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to pause.</param>
        /// <returns>
        /// The total number of paused Tweener (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Pause(Tweener tweener) =>
            DoFilteredIteration(tweener, DoFilteredPause, false);

        /// <summary>
        /// Pauses the given Sequence, and returns the total number of paused ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to pause.</param>
        /// <returns>
        /// The total number of paused Sequence (1 if the sequence existed, otherwise 0).
        /// </returns>
        public static int Pause(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredPause, false);

        /// <summary>
        /// Pauses all Tweeners/Sequences, and returns the total number of paused Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of paused Tweeners/Sequences.</returns>
        public static int Pause() =>
            DoFilteredIteration(null, DoFilteredPause, false);

        /// <summary>
        /// Resumes all the tweens (delays included) for the given target, and returns the total number of resumed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to resume.</param>
        /// <returns>The total number of resumed Tweeners.</returns>
        public static int Play(object target) => Play(target, false);

        /// <summary>
        /// Resumes all the tweens for the given target, and returns the total number of resumed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners.</returns>
        public static int Play(object target, bool skipDelay) =>
            DoFilteredIteration(target, DoFilteredPlay, false, skipDelay);

        /// <summary>
        /// Resumes all the Tweeners (delays included) and Sequences with the given ID, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to resume.</param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play(string id) => Play(id, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given ID, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play(string id, bool skipDelay) =>
            DoFilteredIteration(id, DoFilteredPlay, false, skipDelay);

        /// <summary>
        /// Resumes all the Tweeners (delays included) and Sequences with the given intId, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to resume.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play(int intId) => Play(intId, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given intId, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to resume.
        /// </param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play(int intId, bool skipDelay) =>
            DoFilteredIteration(intId, DoFilteredPlay, false, skipDelay);

        /// <summary>
        /// Resumes the given Tweener (delays included), and returns the total number of resumed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to resume.</param>
        /// <returns>
        /// The total number of resumed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Play(Tweener tweener) => Play(tweener, false);

        /// <summary>
        /// Resumes the given Tweener, and returns the total number of resumed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>
        /// The total number of resumed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Play(Tweener tweener, bool skipDelay) =>
            DoFilteredIteration(tweener, DoFilteredPlay, false, skipDelay);

        /// <summary>
        /// Resumes the given Sequence, and returns the total number of resumed ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to resume.</param>
        /// <returns>
        /// The total number of resumed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Play(Sequence sequence) => 
            DoFilteredIteration(sequence, DoFilteredPlay, false);

        /// <summary>
        /// Resumes all Tweeners (delays included) and Sequences, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play() => Play(false);

        /// <summary>
        /// Resumes all Tweeners/Sequences, and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int Play(bool skipDelay) =>
            DoFilteredIteration(null, DoFilteredPlay, false, skipDelay);

        /// <summary>
        /// Resumes all the tweens (delays included) for the given target,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to resume.</param>
        /// <returns>The total number of resumed Tweeners.</returns>
        public static int PlayForward(object target) => PlayForward(target, false);

        /// <summary>
        /// Resumes all the tweens for the given target,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners.</returns>
        public static int PlayForward(object target, bool skipDelay) =>
            DoFilteredIteration(target, DoFilteredPlayForward, false, skipDelay);

        /// <summary>
        /// Resumes all the Tweeners (delays included) and Sequences with the given ID,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to resume.</param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward(string id) => PlayForward(id, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given ID,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward(string id, bool skipDelay) =>
            DoFilteredIteration(id, DoFilteredPlayForward, false, skipDelay);

        /// <summary>
        /// Resumes all the Tweeners (delays included) and Sequences with the given intId,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to resume.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward(int intId) => PlayForward(intId, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given intId,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to resume.
        /// </param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward(int intId, bool skipDelay) =>
            DoFilteredIteration(intId, DoFilteredPlayForward, false, skipDelay);

        /// <summary>
        /// Resumes the given Tweener (delays included),
        /// sets it so that it moves forward and not backwards,
        /// and returns the total number of resumed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to resume.</param>
        /// <returns>
        /// The total number of resumed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int PlayForward(Tweener tweener) => PlayForward(tweener, false);

        /// <summary>
        /// Resumes the given Tweener,
        /// sets it so that it moves forward and not backwards,
        /// and returns the total number of resumed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to resume.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>
        /// The total number of resumed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int PlayForward(Tweener tweener, bool skipDelay) =>
            DoFilteredIteration(tweener, DoFilteredPlayForward, false, skipDelay);

        /// <summary>
        /// Resumes the given Sequence,
        /// sets it so that it moves forward and not backwards,
        /// and returns the total number of resumed ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to resume.</param>
        /// <returns>
        /// The total number of resumed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int PlayForward(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredPlayForward, false);

        /// <summary>
        /// Resumes all Tweeners (delays included) and Sequences,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward() => PlayForward(false);

        /// <summary>
        /// Resumes all Tweeners/Sequences,
        /// sets the tweens so that they move forward and not backwards,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayForward(bool skipDelay) =>
            DoFilteredIteration(null, DoFilteredPlayForward, false, skipDelay);

        /// <summary>
        /// Resumes all the tweens for the given target,
        /// sets the tweens so that they move backwards instead than forward,
        /// and returns the total number of resumed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to resume.</param>
        /// <returns>The total number of resumed Tweeners.</returns>
        public static int PlayBackwards(object target) =>
            DoFilteredIteration(target, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given ID,
        /// sets the tweens so that they move backwards instead than forward,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to resume.</param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayBackwards(string id) =>
            DoFilteredIteration(id, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Resumes all the Tweeners/Sequences with the given intId,
        /// sets the tweens so that they move backwards instead than forward,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to resume.
        /// </param>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayBackwards(int intId) =>
            DoFilteredIteration(intId, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Resumes the given Tweener,
        /// sets it so that it moves backwards instead than forward,
        /// and returns the total number of resumed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to resume.</param>
        /// <returns>
        /// The total number of resumed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int PlayBackwards(Tweener tweener) =>
            DoFilteredIteration(tweener, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Resumes the given Sequence,
        /// sets it so that it moves backwards instead than forward,
        /// and returns the total number of resumed ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to resume.</param>
        /// <returns>
        /// The total number of resumed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int PlayBackwards(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Resumes all Tweeners/Sequences,
        /// sets the tweens so that they move backwards instead than forward,
        /// and returns the total number of resumed Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of resumed Tweeners/Sequences.</returns>
        public static int PlayBackwards() =>
            DoFilteredIteration(null, DoFilteredPlayBackwards, false);

        /// <summary>
        /// Rewinds all the tweens (delays included) for the given target, and returns the total number of rewinded Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to rewind.</param>
        /// <returns>The total number of rewinded Tweeners.</returns>
        public static int Rewind(object target) => Rewind(target, false);

        /// <summary>
        /// Rewinds all the tweens for the given target, and returns the total number of rewinded Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to rewind.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>The total number of rewinded Tweeners.</returns>
        public static int Rewind(object target, bool skipDelay) =>
            DoFilteredIteration(target, DoFilteredRewind, false, skipDelay);

        /// <summary>
        /// Rewinds all the Tweeners (delays included) and Sequences with the given ID, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to rewind.</param>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind(string id) => Rewind(id, false);

        /// <summary>
        /// Rewinds all the Tweeners/Sequences with the given ID, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to rewind.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind(string id, bool skipDelay) =>
            DoFilteredIteration(id, DoFilteredRewind, false, skipDelay);

        /// <summary>
        /// Rewinds all the Tweeners (delays included) and Sequences with the given intId, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to rewind.
        /// </param>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind(int intId) => Rewind(intId, false);

        /// <summary>
        /// Rewinds all the Tweeners/Sequences with the given intId, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to rewind.
        /// </param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind(int intId, bool skipDelay) =>
            DoFilteredIteration(intId, DoFilteredRewind, false, skipDelay);

        /// <summary>
        /// Rewinds the given Tweener (delays included), and returns the total number of rewinded ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to rewind.</param>
        /// <returns>
        /// The total number of rewinded Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Rewind(Tweener tweener) => Rewind(tweener, false);

        /// <summary>
        /// Rewinds the given Tweener, and returns the total number of rewinded ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to rewind.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>
        /// The total number of rewinded Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Rewind(Tweener tweener, bool skipDelay) =>
            DoFilteredIteration(tweener, DoFilteredRewind, false, skipDelay);

        /// <summary>
        /// Rewinds the given Sequence, and returns the total number of rewinded ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to rewind.</param>
        /// <returns>
        /// The total number of rewinded Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Rewind(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredRewind, false);

        /// <summary>
        /// Rewinds all Tweeners (delay included) and Sequences, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind() => Rewind(false);

        /// <summary>
        /// Rewinds all Tweeners/Sequences, and returns the total number of rewinded Tweeners/Sequences.
        /// </summary>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of rewinded Tweeners/Sequences.</returns>
        public static int Rewind(bool skipDelay) =>
            DoFilteredIteration(null, DoFilteredRewind, false, skipDelay);

        /// <summary>
        /// Restarts all the tweens (delays included) for the given target, and returns the total number of restarted Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to restart.</param>
        /// <returns>The total number of restarted Tweeners.</returns>
        public static int Restart(object target) => Restart(target, false);

        /// <summary>
        /// Restarts all the tweens for the given target, and returns the total number of restarted Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to restart.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>The total number of restarted Tweeners.</returns>
        public static int Restart(object target, bool skipDelay) =>
            DoFilteredIteration(target, DoFilteredRestart, false, skipDelay);

        /// <summary>
        /// Restarts all the Tweeners (delays included) and Sequences with the given ID, and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to restart.</param>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart(string id) => Restart(id, false);

        /// <summary>
        /// Restarts all the Tweeners/Sequences with the given ID, and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to restart.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart(string id, bool skipDelay) =>
            DoFilteredIteration(id, DoFilteredRestart, false, skipDelay);

        /// <summary>
        /// Restarts all the Tweeners (delays included) and Sequences with the given intId, and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to restart.
        /// </param>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart(int intId) => Restart(intId, false);

        /// <summary>
        /// Restarts all the Tweeners/Sequences with the given intId, and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to restart.
        /// </param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart(int intId, bool skipDelay) =>
            DoFilteredIteration(intId, DoFilteredRestart, false, skipDelay);

        /// <summary>
        /// Restarts the given Tweener (delays included), and returns the total number of restarted ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to restart.</param>
        /// <returns>
        /// The total number of restarted Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Restart(Tweener tweener) => Restart(tweener, false);

        /// <summary>
        /// Restarts the given Tweener, and returns the total number of restarted ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to restart.</param>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial delay.
        /// </param>
        /// <returns>
        /// The total number of restarted Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Restart(Tweener tweener, bool skipDelay) =>
            DoFilteredIteration(tweener, DoFilteredRestart, false, skipDelay);

        /// <summary>
        /// Restarts the given Sequence, and returns the total number of restarted ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to restart.</param>
        /// <returns>
        /// The total number of restarted Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Restart(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredRestart, false);

        /// <summary>
        /// Restarts all Tweeners (delay included) and Sequences, and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart() => Restart(false);

        /// <summary>
        /// Restarts all Tweeners/Sequences and returns the total number of restarted Tweeners/Sequences.
        /// </summary>
        /// <param name="skipDelay">
        /// If <c>true</c> skips any initial tween delay.
        /// </param>
        /// <returns>The total number of restarted Tweeners/Sequences.</returns>
        public static int Restart(bool skipDelay) =>
            DoFilteredIteration(null, DoFilteredRestart, false, skipDelay);

        /// <summary>
        /// Reverses all the tweens for the given target,
        /// animating them from their current value back to the starting one,
        /// and returns the total number of reversed Tweeners.
        /// </summary>
        /// <param name="target">The target whose tweens to reverse.</param>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>The total number of reversed Tweeners.</returns>
        public static int Reverse(object target, bool forcePlay = false) =>
            DoFilteredIteration(target, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Reverses all the Tweeners/Sequences with the given ID,
        /// animating them from their current value back to the starting one,
        /// and returns the total number of reversed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to reverse.</param>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>The total number of reversed Tweeners/Sequences.</returns>
        public static int Reverse(string id, bool forcePlay = false) =>
            DoFilteredIteration(id, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Reverses all the Tweeners/Sequences with the given intId,
        /// animating them from their current value back to the starting one,
        /// and returns the total number of reversed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to reverse.
        /// </param>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>The total number of reversed Tweeners/Sequences.</returns>
        public static int Reverse(int intId, bool forcePlay = false) =>
            DoFilteredIteration(intId, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Reverses the given Tweener,
        /// animating it from its current value back to the starting one,
        /// and returns the total number of reversed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to reverse.</param>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>
        /// The total number of reversed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Reverse(Tweener tweener, bool forcePlay = false) =>
            DoFilteredIteration(tweener, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Reverses the given Sequence, and returns the total number of reversed ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to reverse.</param>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>
        /// The total number of reversed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Reverse(Sequence sequence, bool forcePlay = false) =>
            DoFilteredIteration(sequence, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Reverses all Tweeners/Sequences,
        /// animating them from their current value back to the starting one,
        /// and returns the total number of reversed Tweeners/Sequences.
        /// </summary>
        /// <param name="forcePlay">
        /// If TRUE, the tween will also start playing in case it was paused,
        /// otherwise it will maintain its current play/pause state (default).
        /// </param>
        /// <returns>The total number of reversed Tweeners/Sequences.</returns>
        public static int Reverse(bool forcePlay = false) =>
            DoFilteredIteration(null, DoFilteredReverse, forcePlay);

        /// <summary>
        /// Completes all the tweens for the given target, and returns the total number of completed Tweeners.
        /// Where a loop was involved and not infinite, the relative tween completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <param name="target">The target whose tweens to complete.</param>
        /// <returns>The total number of completed Tweeners.</returns>
        public static int Complete(object target) =>
            DoFilteredIteration(target, DoFilteredComplete, true);

        /// <summary>
        /// Completes all the Tweeners/Sequences with the given ID, and returns the total number of completed Tweeners/Sequences.
        /// Where a loop was involved and not infinite, the relative Tweener/Sequence completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to complete.</param>
        /// <returns>The total number of completed Tweeners/Sequences.</returns>
        public static int Complete(string id) =>
            DoFilteredIteration(id, DoFilteredComplete, true);

        /// <summary>
        /// Completes all the Tweeners/Sequences with the given intId, and returns the total number of completed Tweeners/Sequences.
        /// Where a loop was involved and not infinite, the relative Tweener/Sequence completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to complete.
        /// </param>
        /// <returns>The total number of completed Tweeners/Sequences.</returns>
        public static int Complete(int intId) =>
            DoFilteredIteration(intId, DoFilteredComplete, true);

        /// <summary>
        /// Completes the given Tweener, and returns the total number of completed ones (1 if the Tweener existed, otherwise 0).
        /// Where a loop was involved and not infinite, the relative Tweener completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <param name="tweener">The Tweener to complete.</param>
        /// <returns>
        /// The total number of completed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Complete(Tweener tweener) => 
            DoFilteredIteration(tweener, DoFilteredComplete, true);

        /// <summary>
        /// Completes the given Sequence, and returns the total number of completed ones (1 if the Sequence existed, otherwise 0).
        /// Where a loop was involved and not infinite, the relative Sequence completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <param name="sequence">The Sequence to complete.</param>
        /// <returns>
        /// The total number of completed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Complete(Sequence sequence) =>
            DoFilteredIteration( sequence, DoFilteredComplete, true);

        /// <summary>
        /// Completes all Tweeners/Sequences, and returns the total number of completed Tweeners/Sequences.
        /// Where a loop was involved and not infinite, the relative Tweener/Sequence completes at the position where it would actually be after the set number of loops.
        /// If there were infinite loops, this method will have no effect.
        /// </summary>
        /// <returns>The total number of completed Tweeners/Sequences.</returns>
        public static int Complete() =>
            DoFilteredIteration(null, DoFilteredComplete, true);

        /// <summary>
        /// Kills all the tweens for the given target (unless they're were created inside a <see cref="T:Holoville.HOTween.Sequence" />),
        /// and returns the total number of killed Tweeners.
        /// </summary>
        /// <param name="target">The target whose Tweeners to kill.</param>
        /// <returns>The total number of killed Tweeners.</returns>
        public static int Kill(object target) =>
            DoFilteredIteration(target, DoFilteredKill, true);

        /// <summary>
        /// Kills all the Tweeners/Sequences with the given ID, and returns the total number of killed Tweeners/Sequences.
        /// </summary>
        /// <param name="id">The ID of the Tweeners/Sequences to kill.</param>
        /// <returns>The total number of killed Tweeners/Sequences.</returns>
        public static int Kill(string id) =>
            DoFilteredIteration(id, DoFilteredKill, true);

        /// <summary>
        /// Kills all the Tweeners/Sequences with the given intId, and returns the total number of killed Tweeners/Sequences.
        /// </summary>
        /// <param name="intId">
        /// The intId of the Tweeners/Sequences to kill.
        /// </param>
        /// <returns>The total number of killed Tweeners/Sequences.</returns>
        public static int Kill(int intId) =>
            DoFilteredIteration(intId, DoFilteredKill, true);

        /// <summary>
        /// Kills the given Tweener, and returns the total number of killed ones (1 if the Tweener existed, otherwise 0).
        /// </summary>
        /// <param name="tweener">The Tweener to kill.</param>
        /// <returns>
        /// The total number of killed Tweeners (1 if the Tweener existed, otherwise 0).
        /// </returns>
        public static int Kill(Tweener tweener) =>
            DoFilteredIteration(tweener, DoFilteredKill, true);

        /// <summary>
        /// Kills the given Sequence, and returns the total number of killed ones (1 if the Sequence existed, otherwise 0).
        /// </summary>
        /// <param name="sequence">The Sequence to kill.</param>
        /// <returns>
        /// The total number of killed Sequences (1 if the Sequence existed, otherwise 0).
        /// </returns>
        public static int Kill(Sequence sequence) =>
            DoFilteredIteration(sequence, DoFilteredKill, true);

        /// <summary>
        /// Kills all Tweeners/Sequences, and returns the total number of killed Tweeners/Sequences.
        /// </summary>
        /// <returns>The total number of killed Tweeners/Sequences.</returns>
        public static int Kill() => 
            DoFilteredIteration(null, DoFilteredKill, true);

        /// <summary>
        /// Used by Sequences to remove added tweens from main tweens list.
        /// </summary>
        /// <param name="tween"></param>
        internal static void RemoveFromTweens(IHOTweenComponent tween)
        {
            if (_tweens == null) return;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_tweens[index] == tween)
                {
                    _tweens.RemoveAt(index);
                    break;
                }
            }
        }

        /// <summary>
        /// Returns all existing Tweeners (excluding nested ones) and Sequences, paused or not.
        /// </summary>
        public static List<IHOTweenComponent> GetAllTweens()
        {
            if (_tweens == null) return new List<IHOTweenComponent>(1);
            
            var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
            foreach (var tween in _tweens)
                hoTweenComponentList.Add(tween);
            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns all existing Tweeners (excluding nested ones) and Sequences that are currently playing.
        /// </summary>
        public static List<IHOTweenComponent> GetAllPlayingTweens()
        {
            if (_tweens == null) return new List<IHOTweenComponent>(1);
            
            var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
            foreach (var tween in _tweens)
            {
                if (!tween.isPaused)
                    hoTweenComponentList.Add(tween);
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns all existing Tweeners (excluding nested ones) and Sequences that are currently paused.
        /// </summary>
        public static List<IHOTweenComponent> GetAllPausedTweens()
        {
            if (_tweens == null) return new List<IHOTweenComponent>(1);
            
            var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
            foreach (var tween in _tweens)
            {
                if (tween.isPaused)
                    hoTweenComponentList.Add(tween);
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns a list of the eventual existing tweens with the given Id,
        /// (empty if no Tweener/Sequence was found).
        /// </summary>
        /// <param name="id">Id to look for</param>
        /// <param name="includeNestedTweens">If TRUE also searches inside nested tweens</param>
        /// <returns></returns>
        public static List<IHOTweenComponent> GetTweensById(string id, bool includeNestedTweens)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (_tweens == null) return hoTweenComponentList;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                var tween = _tweens[index];
                if (includeNestedTweens)
                    hoTweenComponentList.AddRange(tween.GetTweensById(id));
                else if (tween.id == id)
                    hoTweenComponentList.Add(tween);
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns a list of the eventual existing tweens with the given intId,
        /// (empty if no Tweener/Sequence was found).
        /// </summary>
        /// <param name="intId">IntId to look for</param>
        /// <param name="includeNestedTweens">If TRUE also searches inside nested tweens</param>
        /// <returns></returns>
        public static List<IHOTweenComponent> GetTweensByIntId(int intId, bool includeNestedTweens)
        {
            var hoTweenComponentList = new List<IHOTweenComponent>();
            if (_tweens == null) return hoTweenComponentList;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                var tween = _tweens[index];
                if (includeNestedTweens)
                    hoTweenComponentList.AddRange(tween.GetTweensByIntId(intId));
                else if (tween.intId == intId)
                    hoTweenComponentList.Add(tween);
            }

            return hoTweenComponentList;
        }

        /// <summary>
        /// Returns a list with all the existing <see cref="T:Holoville.HOTween.Tweener" /> objects whose target is the given one,
        /// or an empty list if none was found.
        /// </summary>
        /// <param name="target">Target to look for</param>
        /// <param name="includeNestedTweens">If TRUE also searches inside nested Tweeners</param>
        /// <returns></returns>
        public static List<Tweener> GetTweenersByTarget(object target, bool includeNestedTweens)
        {
            var tweenerList = new List<Tweener>();
            if (_tweens == null) return tweenerList;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                var tween = _tweens[index];
                if (tween is Tweener tweener2)
                {
                    if (tweener2.target == target)
                        tweenerList.Add(tweener2);
                }
                else if (includeNestedTweens)
                    tweenerList.AddRange(((Sequence)tween).GetTweenersByTarget(target));
            }

            return tweenerList;
        }

        /// <summary>
        /// Returns <c>true</c> if the given target is currently involved in any running Tweener or Sequence (taking into account also nested tweens).
        /// Returns <c>false</c> both if the given target is not inside a Tweener, than if the relative Tweener is paused.
        /// To simply check if the target is attached to a Tweener or Sequence use <see cref="M:Holoville.HOTween.HOTween.IsLinkedTo(System.Object)" /> instead.
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>
        /// A value of <c>true</c> if the given target is currently involved in any running Tweener or Sequence (taking into account also nested tweens).
        /// </returns>
        public static bool IsTweening(object target)
        {
            if (_tweens == null) return false;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_tweens[index].IsTweening(target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the given id is involved in any running Tweener or Sequence (taking into account also nested tweens).
        /// </summary>
        /// <param name="id">The target to check.</param>
        public static bool IsTweening(string id)
        {
            if (_tweens == null) return false;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_tweens[index].IsTweening(id))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the given id is involved in any running Tweener or Sequence (taking into account also nested tweens).
        /// </summary>
        /// <param name="id">The target to check.</param>
        public static bool IsTweening(int id)
        {
            if (_tweens == null) return false;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_tweens[index].IsTweening(id))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the given target is linked to any Tweener or Sequence (running or not, taking into account also nested tweens).
        /// </summary>
        /// <param name="target">The target to check.</param>
        /// <returns>
        /// A value of <c>true</c> if the given target is linked to any Tweener or Sequence (running or not, taking into account also nested tweens).
        /// </returns>
        public static bool IsLinkedTo(object target)
        {
            if (_tweens == null) return false;
            
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
            {
                if (_tweens[index].IsLinkedTo(target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a <see cref="T:Holoville.HOTween.Core.TweenInfo" /> list of the current tweens (paused and delayed included),
        /// or null if there are no tweens.
        /// </summary>
        /// <returns></returns>
        public static TweenInfo[] GetTweenInfos()
        {
            if (totTweens <= 0) return null;
            
            var count = _tweens.Count;
            var tweenInfoArray = new TweenInfo[count];
            for (var index = 0; index < count; ++index)
                tweenInfoArray[index] = new TweenInfo(_tweens[index]);
            return tweenInfoArray;
        }

        private static void DoUpdate(UpdateType updateType, float elapsed)
        {
            TweensToRemoveIndexes.Clear();
            isUpdateLoop = true;
            var count1 = _tweens.Count;
            for (var index = 0; index < count1; ++index)
            {
                var tween = _tweens[index];
                if (tween.updateType == updateType && tween.Update(elapsed * tween.timeScale) &&
                    (tween.destroyed || tween.autoKillOnComplete))
                {
                    tween.Kill(false);
                    if (TweensToRemoveIndexes.IndexOf(index) == -1)
                        TweensToRemoveIndexes.Add(index);
                }
            }

            isUpdateLoop = false;
            var count2 = TweensToRemoveIndexes.Count;
            if (count2 > 0)
            {
                TweensToRemoveIndexes.Sort();
                for (var index = 0; index < count2; ++index)
                    _tweens.RemoveAt(TweensToRemoveIndexes[index] -
                                    index);
            }

            var count3 = onCompletes.Count;
            if (count3 <= 0)
                return;
            for (var index = 0; index < count3; ++index)
                onCompletes[index].OnCompleteDispatch();
            onCompletes = new List<ABSTweenComponent>();
        }

        private static void DoFilteredKill(int index, bool optionalBool)
        {
            _tweens[index].Kill(false);
            if (isUpdateLoop)
            {
                if (TweensToRemoveIndexes.IndexOf(index) != -1)
                    return;
                TweensToRemoveIndexes.Add(index);
            }
            else
                _tweens.RemoveAt(index);
        }

        private static void DoFilteredPause(int index, bool optionalBool) =>
            _tweens[index].Pause();

        private static void DoFilteredPlay(int index, bool skipDelay)
        {
            var tween = _tweens[index];
            if (tween is Tweener tweener)
                tweener.Play(skipDelay);
            else
                tween.Play();
        }

        private static void DoFilteredPlayForward(int index, bool skipDelay)
        {
            var tween = _tweens[index];
            if (tween is Tweener tweener)
                tweener.PlayForward(skipDelay);
            else
                tween.PlayForward();
        }

        private static void DoFilteredPlayBackwards(int index, bool optionalBool)
        {
            var tween = _tweens[index];
            if (tween is Tweener tweener)
                tweener.PlayBackwards();
            else
                tween.PlayBackwards();
        }

        private static void DoFilteredRewind(int index, bool skipDelay)
        {
            var tween = _tweens[index];
            if (tween is Tweener tweener)
                tweener.Rewind(skipDelay);
            else
                tween.Rewind();
        }

        private static void DoFilteredRestart(int index, bool skipDelay)
        {
            var tween = _tweens[index];
            if (tween is Tweener tweener)
                tweener.Restart(skipDelay);
            else
                tween.Restart();
        }

        private static void DoFilteredReverse(int index, bool forcePlay = false) =>
            _tweens[index].Reverse(forcePlay);

        private static void DoFilteredComplete(int index, bool optionalBool) =>
            _tweens[index].Complete(false);

        /// <summary>Used by callbacks that are wired to sendMessage.</summary>
        internal static void DoSendMessage(TweenEvent e)
        {
            var param1 = e.parms[0] as GameObject;
            if (param1 == null) return;
            
            var param2 = e.parms[1] as string;
            var param3 = e.parms[2];
            var param4 = (SendMessageOptions)e.parms[3];
            if (param3 != null)
                param1.SendMessage(param2, e.parms[2], param4);
            else
                param1.SendMessage(param2, param4);
        }

        private static void AddTween(ABSTweenComponent tween)
        {
            if (_tweenGOInstance == null)
                NewTweenInstance();
            if (_tweens == null)
            {
                _tweens = new List<ABSTweenComponent>();
                _it.StartCoroutines();
            }

            _tweens.Add(tween);
            SetGOName();
        }

        private static void NewTweenInstance()
        {
            _tweenGOInstance = new GameObject(nameof(HOTween));
            _it = _tweenGOInstance.AddComponent<HOTween>();
            DontDestroyOnLoad(_tweenGOInstance);
        }

        private void StartCoroutines()
        {
            _time = Time.realtimeSinceStartup;
            StartCoroutine(StartCoroutines_StartTimeScaleIndependentUpdate());
        }

        private IEnumerator StartCoroutines_StartTimeScaleIndependentUpdate()
        {
            yield return null;
            StartCoroutine(TimeScaleIndependentUpdate());
        }

        private static void SetGOName()
        {
            if (!IsEditor || !_renameInstToCountTw || (_isQuitting || !(_tweenGOInstance != null))) return;
            
            _tweenGOInstance.name = "HOTween : " + totTweens;
        }

        private static bool CheckClear()
        {
            if (_tweens == null || _tweens.Count == 0)
            {
                Clear();
                if (_isPermanent)
                    SetGOName();
                return true;
            }

            SetGOName();
            return false;
        }

        private static void Clear()
        {
            if (_it != null)
                _it.StopAllCoroutines();
            _tweens = null;
            if (_isPermanent)
                return;
            if (_tweenGOInstance != null)
                Destroy(_tweenGOInstance);
            _tweenGOInstance = null;
            _it = null;
        }

        /// <summary>
        /// Filter filters for:
        /// - ID if <see cref="T:System.String" />
        /// - Tweener if <see cref="T:Holoville.HOTween.Tweener" />
        /// - Sequence if <see cref="T:Holoville.HOTween.Sequence" />
        /// - Tweener target if <see cref="T:System.Object" /> (doesn't look inside sequence tweens)
        /// - Everything if null
        /// </summary>
        private static int DoFilteredIteration( object filter, TweenDelegate.FilterFunc operation, bool collectionChanger)
        {
            return DoFilteredIteration(filter, operation, collectionChanger, false);
        }

        private static int DoFilteredIteration(object filter, TweenDelegate.FilterFunc operation, bool collectionChanger, bool optionalBool)
        {
            if (_tweens == null) return 0;
            
            var num1 = 0;
            var num2 = _tweens.Count - 1;
            switch (filter)
            {
                case null:
                    for (var index = num2; index > -1; --index)
                    {
                        operation(index, optionalBool);
                        ++num1;
                    }
                    break;
                    
                case int num3:
                    for (var index = num2; index > -1; --index)
                    {
                        if (_tweens[index].intId == num3)
                        {
                            operation(index, optionalBool);
                            ++num1;
                        }
                    }
                    break;
                    
                case string _:
                    var str = (string)filter;
                    for (var index = num2; index > -1; --index)
                    {
                        if (_tweens[index].id == str)
                        {
                            operation(index, optionalBool);
                            ++num1;
                        }
                    }
                    break;
                    
                case Tweener _:
                    var tweener = filter as Tweener;
                    for (var index = num2; index > -1; --index)
                    {
                        if (_tweens[index] == tweener)
                        {
                            operation(index, optionalBool);
                            ++num1;
                        }
                    }
                    break;
                    
                case Sequence _:
                    var sequence = filter as Sequence;
                    for (var index = num2; index > -1; --index)
                    {
                        if (_tweens[index] == sequence)
                        {
                            operation(index, optionalBool);
                            ++num1;
                        }
                    }
                    break;
                    
                default:
                    for (var index = num2; index > -1; --index)
                    {
                        if (_tweens[index] is Tweener tween2 && tween2.target == filter)
                        {
                            operation(index, optionalBool);
                            ++num1;
                        }
                    }
                    break;
            }

            if (collectionChanger)
                CheckClear();
                
            return num1;
        }

        /// <summary>
        /// Returns all the currently existing plugins involved in any tween, even if nested or paused,
        /// or <c>null</c> if there are none.
        /// </summary>
        private static List<ABSTweenPlugin> GetPlugins()
        {
            if (_tweens == null) return null;
            
            var plugs = new List<ABSTweenPlugin>();
            var count = _tweens.Count;
            for (var index = 0; index < count; ++index)
                _tweens[index].FillPluginsList(plugs);
            return plugs;
        }
    }
}