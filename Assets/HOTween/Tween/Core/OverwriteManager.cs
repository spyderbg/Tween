using System.Collections.Generic;

namespace Holoville.HOTween.Core {

internal class OverwriteManager
{
    internal bool IsEnabled;
    internal bool IsLogWarnings;

    private readonly List<Tweener> _runningTweens;

    public OverwriteManager() =>
        _runningTweens = new List<Tweener>();

    public void AddTween(Tweener tween)
    {
        if (IsEnabled)
        {
            var plugins1 = tween.Plugins;
            var num = _runningTweens.Count - 1;
            var count1 = plugins1.Count;
            label_25:
            for (var index1 = num; index1 > -1; --index1)
            {
                var runningTween = _runningTweens[index1];
                var plugins2 = runningTween.Plugins;
                var count2 = plugins2.Count;
                if (runningTween.Target == tween.Target)
                {
                    for (var index2 = 0; index2 < count1; ++index2)
                    {
                        var absTweenPlugin1 = plugins1[index2];
                        for (var index3 = count2 - 1; index3 > -1; --index3)
                        {
                            var absTweenPlugin2 = plugins2[index3];
                            if (absTweenPlugin2.propName == absTweenPlugin1.propName &&
                                (absTweenPlugin1.pluginId == -1 || absTweenPlugin2.pluginId == -1 ||
                                 absTweenPlugin2.pluginId == absTweenPlugin1.pluginId))
                            {
                                if (!runningTween.IsSequenced || !tween.IsSequenced ||
                                    runningTween.ContSequence != tween.ContSequence)
                                {
                                    if (!runningTween.IsPaused &&
                                        (!runningTween.IsSequenced || !runningTween.IsComplete))
                                    {
                                        plugins2.RemoveAt(index3);
                                        --count2;
                                        if (HOTween.IsEditor && HOTween.kWarningLevel == WarningLevel.Verbose)
                                        {
                                            var str1 = absTweenPlugin1.GetType().ToString();
                                            var str2 = str1.Substring(str1.LastIndexOf(".") + 1);
                                            var str3 = absTweenPlugin2.GetType().ToString();
                                            var str4 = str3.Substring(str3.LastIndexOf(".") + 1);
                                            if (IsLogWarnings)
                                                TweenWarning.Log(str2 + " is overwriting " + str4 + " for " +
                                                                 runningTween.Target + "." +
                                                                 absTweenPlugin2.propName);
                                        }

                                        if (count2 == 0)
                                        {
                                            if (runningTween.IsSequenced)
                                                runningTween.ContSequence.Remove(runningTween);
                                            _runningTweens.RemoveAt(index1);
                                            runningTween.Kill(false);
                                        }

                                        if (runningTween.OnPluginOverwritten != null)
                                            runningTween.OnPluginOverwritten();
                                        else if (runningTween.OnPluginOverwrittenWParms != null)
                                            runningTween.OnPluginOverwrittenWParms(new TweenEvent(runningTween,
                                                runningTween.OnPluginOverwrittenParms));
                                        if (runningTween.destroyed)
                                            goto label_25;
                                    }
                                }
                                else
                                    goto label_25;
                            }
                        }
                    }
                }
            }
        }

        _runningTweens.Add(tween);
    }

    public void RemoveTween(Tweener tween)
    {
        var count = _runningTweens.Count;
        for (var index = 0; index < count; ++index)
        {
            if (_runningTweens[index] != tween) continue;
            
            _runningTweens.RemoveAt(index);
            break;
        }
    }
}

}