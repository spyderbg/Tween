using System.Collections.Generic;

namespace Holoville.HOTween.Core {

internal class OverwriteManager
{
    internal bool enabled;
    internal bool logWarnings;

    private readonly List<Tweener> runningTweens;

    public OverwriteManager() => runningTweens = new List<Tweener>();

    public void AddTween(Tweener p_tween)
    {
        if (enabled)
        {
            var plugins1 = p_tween.plugins;
            var num = runningTweens.Count - 1;
            var count1 = plugins1.Count;
            label_25:
            for (var index1 = num; index1 > -1; --index1)
            {
                var runningTween = runningTweens[index1];
                var plugins2 = runningTween.plugins;
                var count2 = plugins2.Count;
                if (runningTween.target == p_tween.target)
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
                                if (!runningTween.isSequenced || !p_tween.isSequenced ||
                                    runningTween.contSequence != p_tween.contSequence)
                                {
                                    if (!runningTween._isPaused &&
                                        (!runningTween.isSequenced || !runningTween.isComplete))
                                    {
                                        plugins2.RemoveAt(index3);
                                        --count2;
                                        if (HOTween.IsEditor && HOTween.kWarningLevel == WarningLevel.Verbose)
                                        {
                                            var str1 = absTweenPlugin1.GetType().ToString();
                                            var str2 = str1.Substring(str1.LastIndexOf(".") + 1);
                                            var str3 = absTweenPlugin2.GetType().ToString();
                                            var str4 = str3.Substring(str3.LastIndexOf(".") + 1);
                                            if (logWarnings)
                                                TweenWarning.Log(str2 + " is overwriting " + str4 + " for " +
                                                                 runningTween.target + "." +
                                                                 absTweenPlugin2.propName);
                                        }

                                        if (count2 == 0)
                                        {
                                            if (runningTween.isSequenced)
                                                runningTween.contSequence.Remove(runningTween);
                                            runningTweens.RemoveAt(index1);
                                            runningTween.Kill(false);
                                        }

                                        if (runningTween.onPluginOverwritten != null)
                                            runningTween.onPluginOverwritten();
                                        else if (runningTween.onPluginOverwrittenWParms != null)
                                            runningTween.onPluginOverwrittenWParms(new TweenEvent(runningTween,
                                                runningTween.onPluginOverwrittenParms));
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

        runningTweens.Add(p_tween);
    }

    public void RemoveTween(Tweener p_tween)
    {
        var count = runningTweens.Count;
        for (var index = 0; index < count; ++index)
        {
            if (runningTweens[index] != p_tween) continue;
            
            runningTweens.RemoveAt(index);
            break;
        }
    }
}

}