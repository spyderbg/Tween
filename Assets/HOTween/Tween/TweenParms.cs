using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween {

public class TweenParms : ABSTweenComponentParms
{
    private static readonly Dictionary<Type, string> _TypeToShortString = new Dictionary<Type, string>(8)
    {
        {
            typeof(Vector2),
            "Vector2"
        },
        {
            typeof(Vector3),
            "Vector3"
        },
        {
            typeof(Vector4),
            "Vector4"
        },
        {
            typeof(Quaternion),
            "Quaternion"
        },
        {
            typeof(Color),
            "Color"
        },
        {
            typeof(Color32),
            "Color32"
        },
        {
            typeof(Rect),
            "Rect"
        },
        {
            typeof(string),
            "String"
        },
        {
            typeof(int),
            "Int32"
        }
    };

    private bool pixelPerfect;
    private bool speedBased;
    private bool easeSet;
    private EaseType easeType = HOTween.kDefEaseType;
    private AnimationCurve easeAnimCurve;
    private float easeOvershootOrAmplitude = HOTween.kDefEaseOvershootOrAmplitude;
    private float easePeriod = HOTween.kDefEasePeriod;
    private float delay;
    private List<HOTPropData> propDatas;
    private bool isFrom;
    private TweenDelegate.TweenCallback onPluginOverwritten;
    private TweenDelegate.TweenCallbackWParms onPluginOverwrittenWParms;
    private object[] onPluginOverwrittenParms;

    public bool hasProps => propDatas != null;

    internal void InitializeObject(Tweener tweenObj, object target)
    {
        InitializeOwner(tweenObj);
        if (speedBased && !easeSet)
            easeType = EaseType.Linear;
        tweenObj.PixelPerfect = pixelPerfect;
        tweenObj.SpeedBased = speedBased;
        tweenObj.EaseTypeVal = easeType;
        tweenObj.EaseAnimationCurveVal = easeAnimCurve;
        tweenObj.EaseOvershootOrAmplitude = easeOvershootOrAmplitude;
        tweenObj.EasePeriod = easePeriod;
        tweenObj.Delay = tweenObj.DelayCountVal = delay;
        tweenObj.IsFrom = isFrom;
        tweenObj.OnPluginOverwritten = onPluginOverwritten;
        tweenObj.OnPluginOverwrittenWParms = onPluginOverwrittenWParms;
        tweenObj.OnPluginOverwrittenParms = onPluginOverwrittenParms;
        tweenObj.Plugins = new List<ABSTweenPlugin>();
        var type = target.GetType();
        FieldInfo fieldInfo = null;
        var count = propDatas.Count;
        for (var index = 0; index < count; ++index)
        {
            var propData = propDatas[index];
            var property = type.GetProperty(propData.PropName);
            if (property == null)
            {
                fieldInfo = type.GetField(propData.PropName);
                if (fieldInfo == null)
                {
                    TweenWarning.Log("\"" + target + "." + propData.PropName +
                                     "\" is missing, static, or not public. The tween for this property will not be created.");
                    continue;
                }
            }

            ABSTweenPlugin absTweenPlugin;
            if (propData.EndValOrPlugin is ABSTweenPlugin endValOrPlugin2)
            {
                absTweenPlugin = endValOrPlugin2;
                if (absTweenPlugin.ValidateTarget(target))
                {
                    if (absTweenPlugin.initialized)
                        absTweenPlugin = absTweenPlugin.CloneBasic();
                }
                else
                {
                    TweenWarning.Log(Utils.SimpleClassName(absTweenPlugin.GetType()) + " : Invalid target (" +
                                     target + "). The tween for this property will not be created.");
                    continue;
                }
            }
            else
            {
                absTweenPlugin = null;
                switch (property != null
                    ? (_TypeToShortString.ContainsKey(property.PropertyType)
                        ? _TypeToShortString[property.PropertyType]
                        : "")
                    : (_TypeToShortString.ContainsKey(fieldInfo.FieldType)
                        ? _TypeToShortString[fieldInfo.FieldType]
                        : ""))
                {
                    case "Vector2":
                        if (ValidateValue(propData.EndValOrPlugin, PlugVector2.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector2((Vector2)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "Vector3":
                        if (ValidateValue(propData.EndValOrPlugin, PlugVector3.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector3((Vector3)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "Vector4":
                        if (ValidateValue(propData.EndValOrPlugin, PlugVector4.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector4((Vector4)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "Quaternion":
                        if (ValidateValue(propData.EndValOrPlugin, PlugQuaternion.ValidValueTypes))
                        {
                            absTweenPlugin = !(propData.EndValOrPlugin is Vector3)
                                ? new PlugQuaternion((Quaternion)propData.EndValOrPlugin, propData.IsRelative)
                                : (ABSTweenPlugin)new PlugQuaternion((Vector3)propData.EndValOrPlugin,
                                    propData.IsRelative);
                            break;
                        }

                        break;
                    case "Color":
                        if (ValidateValue(propData.EndValOrPlugin, PlugColor.validValueTypes))
                        {
                            absTweenPlugin = new PlugColor((Color)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "Color32":
                        if (ValidateValue(propData.EndValOrPlugin, PlugColor32.validValueTypes))
                        {
                            absTweenPlugin = new PlugColor32((Color32)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "Rect":
                        if (ValidateValue(propData.EndValOrPlugin, PlugRect.validValueTypes))
                        {
                            absTweenPlugin = new PlugRect((Rect)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    case "String":
                        if (ValidateValue(propData.EndValOrPlugin, PlugString.ValidValueTypes))
                        {
                            absTweenPlugin = new PlugString(propData.EndValOrPlugin.ToString(),
                                propData.IsRelative);
                            break;
                        }

                        break;
                    case "Int32":
                        if (ValidateValue(propData.EndValOrPlugin, PlugInt.validValueTypes))
                        {
                            absTweenPlugin = new PlugInt((int)propData.EndValOrPlugin, propData.IsRelative);
                            break;
                        }

                        break;
                    default:
                        try
                        {
                            absTweenPlugin = new PlugFloat(Convert.ToSingle(propData.EndValOrPlugin),
                                propData.IsRelative);
                            break;
                        }
                        catch (Exception ex)
                        {
                            TweenWarning.Log("No valid plugin for animating \"" + target + "." +
                                             propData.PropName + "\" (of type " +
                                             (property != null
                                                 ? property.PropertyType
                                                 : (object)fieldInfo.FieldType) +
                                             "). The tween for this property will not be created.");
                            continue;
                        }
                }

                if (absTweenPlugin == null)
                {
                    TweenWarning.Log("The end value set for \"" + target + "." + propData.PropName +
                                     "\" tween is invalid. The tween for this property will not be created.");
                    continue;
                }
            }

            absTweenPlugin.Init(tweenObj, propData.PropName, easeType, type, property, fieldInfo);
            tweenObj.Plugins.Add(absTweenPlugin);
        }
    }

    public TweenParms PixelPerfect()
    {
        pixelPerfect = true;
        return this;
    }

    public TweenParms SpeedBased() => SpeedBased(true);

    public TweenParms SpeedBased(bool speedBased)
    {
        this.speedBased = speedBased;
        return this;
    }

    public TweenParms Ease(EaseType easeType) =>
        Ease(easeType, HOTween.kDefEaseOvershootOrAmplitude, HOTween.kDefEasePeriod);

    public TweenParms Ease(EaseType easeType, float overshoot) =>
        Ease(easeType, overshoot, HOTween.kDefEasePeriod);

    public TweenParms Ease(EaseType easeType, float amplitude, float period)
    {
        easeSet = true;
        this.easeType = easeType;
        easeOvershootOrAmplitude = amplitude;
        easePeriod = period;
        return this;
    }

    public TweenParms Ease(AnimationCurve easeAnimationCurve)
    {
        easeSet = true;
        easeType = EaseType.AnimationCurve;
        easeAnimCurve = easeAnimationCurve;
        return this;
    }

    public TweenParms Delay(float delay)
    {
        this.delay = delay;
        return this;
    }

    public TweenParms Pause() => Pause(true);

    public TweenParms Pause(bool pause)
    {
        isPaused = pause;
        return this;
    }

    public TweenParms NewProp(string propName, ABSTweenPlugin plugin) =>
        NewProp(propName, plugin, false);

    public TweenParms NewProp(string propName, object endVal) =>
        NewProp(propName, endVal, false);

    public TweenParms NewProp(string propName, object endVal, bool isRelative)
    {
        propDatas = null;
        return Prop(propName, endVal, isRelative);
    }

    public TweenParms Prop(string propName, ABSTweenPlugin plugin) =>
        Prop(propName, plugin, false);

    public TweenParms Prop(string propName, object endVal) =>
        Prop(propName, endVal, false);

    public TweenParms Prop(string propName, object endVal, bool isRelative)
    {
        propDatas ??= new List<HOTPropData>();
        propDatas.Add(new HOTPropData(propName, endVal, isRelative));
        return this;
    }

    public TweenParms Id(string id)
    {
        base.Id = id;
        return this;
    }

    public TweenParms IntId(int intId)
    {
        base.IntId = intId;
        return this;
    }

    public TweenParms AutoKill(bool active)
    {
        AutoKillOnComplete = active;
        return this;
    }

    public TweenParms UpdateType(UpdateType updateType)
    {
        base.UpdateType = updateType;
        return this;
    }

    public TweenParms TimeScale(float timeScale)
    {
        base.TimeScale = timeScale;
        return this;
    }

    public TweenParms Loops(int loops) =>
        Loops(loops, HOTween.kDefLoopType);

    public TweenParms Loops(int loops, LoopType loopType)
    {
        base.loops = loops;
        base.loopType = loopType;
        return this;
    }

    public TweenParms OnStart(TweenDelegate.TweenCallback function)
    {
        onStart = function;
        return this;
    }

    public TweenParms OnStart(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onStartWParms = function;
        onStartParms = funcParms;
        return this;
    }

    public TweenParms OnUpdate(TweenDelegate.TweenCallback function)
    {
        onUpdate = function;
        return this;
    }

    public TweenParms OnUpdate(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onUpdateWParms = function;
        onUpdateParms = funcParms;
        return this;
    }

    public TweenParms OnPluginUpdated(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onPluginUpdatedWParms = function;
        onPluginUpdatedParms = funcParms;
        return this;
    }

    public TweenParms OnPause(TweenDelegate.TweenCallback function)
    {
        onPause = function;
        return this;
    }

    public TweenParms OnPause(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onPauseWParms = function;
        onPauseParms = funcParms;
        return this;
    }

    public TweenParms OnPlay(TweenDelegate.TweenCallback function)
    {
        onPlay = function;
        return this;
    }

    public TweenParms OnPlay(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onPlayWParms = function;
        onPlayParms = funcParms;
        return this;
    }

    public TweenParms OnRewinded(TweenDelegate.TweenCallback function)
    {
        onRewinded = function;
        return this;
    }

    public TweenParms OnRewinded(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onRewindedWParms = function;
        onRewindedParms = funcParms;
        return this;
    }

    public TweenParms OnStepComplete(TweenDelegate.TweenCallback function)
    {
        onStepComplete = function;
        return this;
    }

    public TweenParms OnStepComplete(
        TweenDelegate.TweenCallbackWParms function,
        params object[] funcParms)
    {
        onStepCompleteWParms = function;
        onStepCompleteParms = funcParms;
        return this;
    }

    public TweenParms OnStepComplete(GameObject sendMessageTarget, string methodName, object value = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
    {
        onStepCompleteWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
        onStepCompleteParms = new object[4]
        {
            sendMessageTarget,
            methodName,
            value,
            options
        };
        return this;
    }

    public TweenParms OnComplete(TweenDelegate.TweenCallback function)
    {
        onComplete = function;
        return this;
    }

    public TweenParms OnComplete(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onCompleteWParms = function;
        onCompleteParms = funcParms;
        return this;
    }

    public TweenParms OnComplete(GameObject sendMessageTarget, string methodName, object value = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
    {
        onCompleteWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
        onCompleteParms = new object[4]
        {
            sendMessageTarget,
            methodName,
            value,
            options
        };
        return this;
    }

    public TweenParms OnPluginOverwritten(TweenDelegate.TweenCallback function)
    {
        onPluginOverwritten = function;
        return this;
    }

    public TweenParms OnPluginOverwritten(TweenDelegate.TweenCallbackWParms function, params object[] funcParms)
    {
        onPluginOverwrittenWParms = function;
        onPluginOverwrittenParms = funcParms;
        return this;
    }

    public TweenParms KeepEnabled(Behaviour target)
    {
        if (target == null)
        {
            manageBehaviours = false;
            return this;
        }

        return KeepEnabled(new Behaviour[1] {target}, true);
    }

    public TweenParms KeepEnabled(GameObject target)
    {
        if (target == null)
        {
            manageGameObjects = false;
            return this;
        }

        return KeepEnabled(new GameObject[1] {target}, true);
    }

    public TweenParms KeepEnabled(Behaviour[] targets) =>
        KeepEnabled(targets, true);

    public TweenParms KeepEnabled(GameObject[] targets) => KeepEnabled(targets, true);

    public TweenParms KeepDisabled(Behaviour target)
    {
        if (target == null)
        {
            manageBehaviours = false;
            return this;
        }

        return KeepEnabled(new Behaviour[1] {target}, false);
    }

    public TweenParms KeepDisabled(GameObject target)
    {
        if (target == null)
        {
            manageGameObjects = false;
            return this;
        }

        return KeepEnabled(new GameObject[1] {target}, false);
    }

    public TweenParms KeepDisabled(Behaviour[] targets) =>
        KeepEnabled(targets, false);

    public TweenParms KeepDisabled(GameObject[] targets) =>
        KeepEnabled(targets, false);

    private TweenParms KeepEnabled(Behaviour[] targets, bool enabled)
    {
        manageBehaviours = true;
        if (enabled)
            managedBehavioursOn = targets;
        else
            managedBehavioursOff = targets;
        return this;
    }

    private TweenParms KeepEnabled(GameObject[] targets, bool enabled)
    {
        manageGameObjects = true;
        if (enabled)
            managedGameObjectsOn = targets;
        else
            managedGameObjectsOff = targets;
        return this;
    }

    internal TweenParms IsFrom()
    {
        isFrom = true;
        return this;
    }

    private static bool ValidateValue(object val, Type[] validVals) =>
        Array.IndexOf<Type>(validVals, val.GetType()) != -1;

    // ReSharper disable once InconsistentNaming
    private class HOTPropData
    {
        public readonly string PropName;
        public readonly object EndValOrPlugin;
        public readonly bool IsRelative;

        public HOTPropData(string propName, object endValOrPlugin, bool isRelative)
        {
            PropName = propName;
            EndValOrPlugin = endValOrPlugin;
            IsRelative = isRelative;
        }
    }
}

}