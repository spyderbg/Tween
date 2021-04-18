using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
namespace Holoville.HOTween {

public class HOTween : MonoBehaviour
{
    public const string  kAuthor      = "Daniele Giardini - Holoville";
    private const string kGameobjname = "HOTween";
    public const string  kVersion     = "1.3.035";

    public const UpdateType   kDefUpdateType               = UpdateType.Update;
    public const float        kDefTimeScale                = 1f;
    public const EaseType     kDefEaseType                 = EaseType.EaseOutQuad;
    public const float        kDefEaseOvershootOrAmplitude = 1.70158f;
    public const float        kDefEasePeriod               = 0.0f;
    public const LoopType     kDefLoopType                 = LoopType.Restart;
    public const WarningLevel kWarningLevel                = WarningLevel.Verbose;

    public static bool ShowPathGizmos;

    internal static bool IsIOS;

    internal static bool IsEditor;

    internal static List<ABSTweenComponent> OnCompletes = new List<ABSTweenComponent>();

    private static bool _initialized;
    private static bool _isPermanent;
    private static bool _renameInstToCountTw;
    private static float _time;
    private static bool _isQuitting;
    private static readonly List<int> TweensToRemoveIndexes = new List<int>();

    internal static OverwriteManager OverwriteManager;

    private static List<ABSTweenComponent> _tweens;
    private static GameObject _tweenGOInstance;
    private static HOTween _it;

    internal static bool isUpdateLoop { get; private set; }

    public static int totTweens =>
        _tweens == null ? 0 : _tweens.Count;

    public static void Init() => Init(false, true, false);

    public static void Init(bool permanentInstance) =>
        Init(permanentInstance, true, false);

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
        if (_tweens == null || !ShowPathGizmos) return;
        
        var plugins = GetPlugins();
        var count = plugins.Count;
        for (var index = 0; index < count; ++index)
        {
            if (plugins[index] is PlugVector3Path plugVector3Path1 && plugVector3Path1.Path != null)
                plugVector3Path1.Path.GizmoDraw(plugVector3Path1.PathPerc, false);
        }
    }

    private void OnDestroy()
    {
        if (_isQuitting || !(this == _it)) return;
        
        Clear();
    }

    internal static void AddSequence(Sequence sequence)
    {
        if (!_initialized)
            Init();
        AddTween(sequence);
    }

    public static Tweener To(object target, float duration, string propName, object endVal)
    {
        return To(target, duration, new TweenParms().Prop(propName, endVal));
    }

    public static Tweener To(object target, float duration, string propName, object endVal, bool isRelative)
    {
        return To(target, duration, new TweenParms().Prop(propName, endVal, isRelative));
    }

    public static Tweener To( object target, float duration, string propName, object endVal, bool isRelative, EaseType easeType, float delay)
    {
        return To(target, duration, new TweenParms().Prop(propName, endVal, isRelative).Delay(delay).Ease(easeType));
    }

    public static Tweener To(object target, float duration, TweenParms parms)
    {
        if (!_initialized)
            Init();
        var tweener = new Tweener(target, duration, parms);
        if (tweener.isEmpty) return null;
        
        AddTween(tweener);
        return tweener;
    }

    public static Tweener From(object target, float duration, string propName, object fromVal)
    {
        return From(target, duration, new TweenParms().Prop(propName, fromVal));
    }

    public static Tweener From(object target, float duration, string propName, object fromVal, bool isRelative)
    {
        return From(target, duration, new TweenParms().Prop(propName, fromVal, isRelative));
    }

    public static Tweener From(object target, float duration, string propName, object fromVal, bool isRelative, EaseType easeType, float delay)
    {
        return From(target, duration, new TweenParms().Prop(propName, fromVal, isRelative).Delay(delay).Ease(easeType));
    }

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

    public static Tweener Punch(object target, float duration, string propName, object fromVal, float punchAmplitude = 0.5f, float punchPeriod = 0.1f)
    {
        var parms = new TweenParms()
            .Prop(propName, fromVal)
            .Ease(EaseType.EaseOutElastic, punchAmplitude, punchPeriod);
        return To(target, duration, parms);
    }

    public static Tweener Punch(object target, float duration, string propName, object fromVal, bool isRelative, float punchAmplitude = 0.5f, float punchPeriod = 0.1f)
    {
        var parms = new TweenParms()
            .Prop(propName, fromVal, isRelative)
            .Ease(EaseType.EaseOutElastic, punchAmplitude, punchPeriod);
        return To(target, duration, parms);
    }

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

    public static Tweener Shake( object target, float duration, string propName, object fromVal, float shakeAmplitude = 0.1f, float shakePeriod = 0.12f)
    {
        var parms = new TweenParms()
            .Prop(propName, fromVal)
            .Ease(EaseType.EaseOutElastic, shakeAmplitude, shakePeriod);
        return From(target, duration, parms);
    }

    public static Tweener Shake(object target, float duration, string propName, object fromVal, bool isRelative, float shakeAmplitude = 0.1f, float shakePeriod = 0.12f)
    {
        var parms = new TweenParms().Prop(propName, fromVal, isRelative)
            .Ease(EaseType.EaseOutElastic, shakeAmplitude, shakePeriod);
        return From(target, duration, parms);
    }

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

    private void Update()
    {
        if (_tweens == null) return;
        
        DoUpdate(UpdateType.Update, Time.deltaTime);
        CheckClear();
    }

    private void LateUpdate()
    {
        if (_tweens == null) return;
        
        DoUpdate(UpdateType.LateUpdate, Time.deltaTime);
        CheckClear();
    }

    private void FixedUpdate()
    {
        if (_tweens == null) return;
        
        DoUpdate(UpdateType.FixedUpdate, Time.fixedDeltaTime);
        CheckClear();
    }

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

    public static void EnableOverwriteManager(bool logWarnings = true)
    {
        if (OverwriteManager == null) return;
        
        OverwriteManager.enabled = true;
        OverwriteManager.logWarnings = logWarnings;
    }

    public static void DisableOverwriteManager()
    {
        if (OverwriteManager == null) return;
        
        OverwriteManager.enabled = false;
    }

    public static int Pause(object target) =>
        DoFilteredIteration(target, DoFilteredPause, false);

    public static int Pause(string id) =>
        DoFilteredIteration(id, DoFilteredPause, false);

    public static int Pause(int intId) =>
        DoFilteredIteration(intId, DoFilteredPause, false);

    public static int Pause(Tweener tweener) =>
        DoFilteredIteration(tweener, DoFilteredPause, false);

    public static int Pause(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredPause, false);

    public static int Pause() =>
        DoFilteredIteration(null, DoFilteredPause, false);

    public static int Play(object target) => 
        Play(target, false);

    public static int Play(object target, bool skipDelay) =>
        DoFilteredIteration(target, DoFilteredPlay, false, skipDelay);

    public static int Play(string id) =>
        Play(id, false);

    public static int Play(string id, bool skipDelay) =>
        DoFilteredIteration(id, DoFilteredPlay, false, skipDelay);

    public static int Play(int intId) => Play(intId, false);

    public static int Play(int intId, bool skipDelay) =>
        DoFilteredIteration(intId, DoFilteredPlay, false, skipDelay);

    public static int Play(Tweener tweener) =>
        Play(tweener, false);

    public static int Play(Tweener tweener, bool skipDelay) =>
        DoFilteredIteration(tweener, DoFilteredPlay, false, skipDelay);

    public static int Play(Sequence sequence) => 
        DoFilteredIteration(sequence, DoFilteredPlay, false);

    public static int Play() => Play(false);

    public static int Play(bool skipDelay) =>
        DoFilteredIteration(null, DoFilteredPlay, false, skipDelay);

    public static int PlayForward(object target) => PlayForward(target, false);

    public static int PlayForward(object target, bool skipDelay) =>
        DoFilteredIteration(target, DoFilteredPlayForward, false, skipDelay);

    public static int PlayForward(string id) => PlayForward(id, false);

    public static int PlayForward(string id, bool skipDelay) =>
        DoFilteredIteration(id, DoFilteredPlayForward, false, skipDelay);

    public static int PlayForward(int intId) => PlayForward(intId, false);

    public static int PlayForward(int intId, bool skipDelay) =>
        DoFilteredIteration(intId, DoFilteredPlayForward, false, skipDelay);

    public static int PlayForward(Tweener tweener) => 
        PlayForward(tweener, false);

    public static int PlayForward(Tweener tweener, bool skipDelay) =>
        DoFilteredIteration(tweener, DoFilteredPlayForward, false, skipDelay);

    public static int PlayForward(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredPlayForward, false);

    public static int PlayForward() =>
        PlayForward(false);

    public static int PlayForward(bool skipDelay) =>
        DoFilteredIteration(null, DoFilteredPlayForward, false, skipDelay);

    public static int PlayBackwards(object target) =>
        DoFilteredIteration(target, DoFilteredPlayBackwards, false);

    public static int PlayBackwards(string id) =>
        DoFilteredIteration(id, DoFilteredPlayBackwards, false);

    public static int PlayBackwards(int intId) =>
        DoFilteredIteration(intId, DoFilteredPlayBackwards, false);

    public static int PlayBackwards(Tweener tweener) =>
        DoFilteredIteration(tweener, DoFilteredPlayBackwards, false);

    public static int PlayBackwards(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredPlayBackwards, false);

    public static int PlayBackwards() =>
        DoFilteredIteration(null, DoFilteredPlayBackwards, false);

    public static int Rewind(object target) => Rewind(target, false);

    public static int Rewind(object target, bool skipDelay) =>
        DoFilteredIteration(target, DoFilteredRewind, false, skipDelay);

    public static int Rewind(string id) =>
        Rewind(id, false);

    public static int Rewind(string id, bool skipDelay) =>
        DoFilteredIteration(id, DoFilteredRewind, false, skipDelay);

    public static int Rewind(int intId) =>
        Rewind(intId, false);

    public static int Rewind(int intId, bool skipDelay) =>
        DoFilteredIteration(intId, DoFilteredRewind, false, skipDelay);

    public static int Rewind(Tweener tweener) =>
        Rewind(tweener, false);

    public static int Rewind(Tweener tweener, bool skipDelay) =>
        DoFilteredIteration(tweener, DoFilteredRewind, false, skipDelay);

    public static int Rewind(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredRewind, false);

    public static int Rewind() => 
        Rewind(false);

    public static int Rewind(bool skipDelay) =>
        DoFilteredIteration(null, DoFilteredRewind, false, skipDelay);

    public static int Restart(object target) => Restart(target, false);

    public static int Restart(object target, bool skipDelay) =>
        DoFilteredIteration(target, DoFilteredRestart, false, skipDelay);

    public static int Restart(string id) => Restart(id, false);

    public static int Restart(string id, bool skipDelay) =>
        DoFilteredIteration(id, DoFilteredRestart, false, skipDelay);

    public static int Restart(int intId) => Restart(intId, false);

    public static int Restart(int intId, bool skipDelay) =>
        DoFilteredIteration(intId, DoFilteredRestart, false, skipDelay);

    public static int Restart(Tweener tweener) =>
        Restart(tweener, false);

    public static int Restart(Tweener tweener, bool skipDelay) =>
        DoFilteredIteration(tweener, DoFilteredRestart, false, skipDelay);

    public static int Restart(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredRestart, false);

    public static int Restart() =>
        Restart(false);

    public static int Restart(bool skipDelay) =>
        DoFilteredIteration(null, DoFilteredRestart, false, skipDelay);

    public static int Reverse(object target, bool forcePlay = false) =>
        DoFilteredIteration(target, DoFilteredReverse, forcePlay);

    public static int Reverse(string id, bool forcePlay = false) =>
        DoFilteredIteration(id, DoFilteredReverse, forcePlay);

    public static int Reverse(int intId, bool forcePlay = false) =>
        DoFilteredIteration(intId, DoFilteredReverse, forcePlay);

    public static int Reverse(Tweener tweener, bool forcePlay = false) =>
        DoFilteredIteration(tweener, DoFilteredReverse, forcePlay);

    public static int Reverse(Sequence sequence, bool forcePlay = false) =>
        DoFilteredIteration(sequence, DoFilteredReverse, forcePlay);

    public static int Reverse(bool forcePlay = false) =>
        DoFilteredIteration(null, DoFilteredReverse, forcePlay);

    public static int Complete(object target) =>
        DoFilteredIteration(target, DoFilteredComplete, true);

    public static int Complete(string id) =>
        DoFilteredIteration(id, DoFilteredComplete, true);

    public static int Complete(int intId) =>
        DoFilteredIteration(intId, DoFilteredComplete, true);

    public static int Complete(Tweener tweener) => 
        DoFilteredIteration(tweener, DoFilteredComplete, true);

    public static int Complete(Sequence sequence) =>
        DoFilteredIteration( sequence, DoFilteredComplete, true);

    public static int Complete() =>
        DoFilteredIteration(null, DoFilteredComplete, true);

    public static int Kill(object target) =>
        DoFilteredIteration(target, DoFilteredKill, true);

    public static int Kill(string id) =>
        DoFilteredIteration(id, DoFilteredKill, true);

    public static int Kill(int intId) =>
        DoFilteredIteration(intId, DoFilteredKill, true);

    public static int Kill(Tweener tweener) =>
        DoFilteredIteration(tweener, DoFilteredKill, true);

    public static int Kill(Sequence sequence) =>
        DoFilteredIteration(sequence, DoFilteredKill, true);

    public static int Kill() => 
        DoFilteredIteration(null, DoFilteredKill, true);

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

    public static List<IHOTweenComponent> GetAllTweens()
    {
        if (_tweens == null) return new List<IHOTweenComponent>(1);
        
        var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
        hoTweenComponentList.AddRange(_tweens);
        return hoTweenComponentList;
    }

    public static List<IHOTweenComponent> GetAllPlayingTweens()
    {
        if (_tweens == null) return new List<IHOTweenComponent>(1);
        
        var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
        hoTweenComponentList.AddRange(_tweens.Where(tween => !tween.isPaused));
        return hoTweenComponentList;
    }

    public static List<IHOTweenComponent> GetAllPausedTweens()
    {
        if (_tweens == null) return new List<IHOTweenComponent>(1);

        var hoTweenComponentList = new List<IHOTweenComponent>(_tweens.Count);
        hoTweenComponentList.AddRange(_tweens.Where(tween => tween.isPaused));
        return hoTweenComponentList;
    }

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

        var count3 = OnCompletes.Count;
        if (count3 <= 0)
            return;
        for (var index = 0; index < count3; ++index)
            OnCompletes[index].OnCompleteDispatch();
        OnCompletes = new List<ABSTweenComponent>();
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