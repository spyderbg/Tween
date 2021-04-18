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
        tweenObj._pixelPerfect = pixelPerfect;
        tweenObj._speedBased = speedBased;
        tweenObj._easeType = easeType;
        tweenObj._easeAnimationCurve = easeAnimCurve;
        tweenObj._easeOvershootOrAmplitude = easeOvershootOrAmplitude;
        tweenObj._easePeriod = easePeriod;
        tweenObj._delay = tweenObj.delayCount = delay;
        tweenObj.isFrom = isFrom;
        tweenObj.onPluginOverwritten = onPluginOverwritten;
        tweenObj.onPluginOverwrittenWParms = onPluginOverwrittenWParms;
        tweenObj.onPluginOverwrittenParms = onPluginOverwrittenParms;
        tweenObj.plugins = new List<ABSTweenPlugin>();
        var type = target.GetType();
        FieldInfo fieldInfo = null;
        var count = propDatas.Count;
        for (var index = 0; index < count; ++index)
        {
            var propData = propDatas[index];
            var property = type.GetProperty(propData.propName);
            if (property == null)
            {
                fieldInfo = type.GetField(propData.propName);
                if (fieldInfo == null)
                {
                    TweenWarning.Log("\"" + target + "." + propData.propName +
                                     "\" is missing, static, or not public. The tween for this property will not be created.");
                    continue;
                }
            }

            ABSTweenPlugin absTweenPlugin;
            if (propData.endValOrPlugin is ABSTweenPlugin endValOrPlugin2)
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
                        if (ValidateValue(propData.endValOrPlugin, PlugVector2.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector2((Vector2)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "Vector3":
                        if (ValidateValue(propData.endValOrPlugin, PlugVector3.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector3((Vector3)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "Vector4":
                        if (ValidateValue(propData.endValOrPlugin, PlugVector4.validValueTypes))
                        {
                            absTweenPlugin = new PlugVector4((Vector4)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "Quaternion":
                        if (ValidateValue(propData.endValOrPlugin, PlugQuaternion.ValidValueTypes))
                        {
                            absTweenPlugin = !(propData.endValOrPlugin is Vector3)
                                ? new PlugQuaternion((Quaternion)propData.endValOrPlugin, propData.isRelative)
                                : (ABSTweenPlugin)new PlugQuaternion((Vector3)propData.endValOrPlugin,
                                    propData.isRelative);
                            break;
                        }

                        break;
                    case "Color":
                        if (ValidateValue(propData.endValOrPlugin, PlugColor.validValueTypes))
                        {
                            absTweenPlugin = new PlugColor((Color)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "Color32":
                        if (ValidateValue(propData.endValOrPlugin, PlugColor32.validValueTypes))
                        {
                            absTweenPlugin = new PlugColor32((Color32)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "Rect":
                        if (ValidateValue(propData.endValOrPlugin, PlugRect.validValueTypes))
                        {
                            absTweenPlugin = new PlugRect((Rect)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    case "String":
                        if (ValidateValue(propData.endValOrPlugin, PlugString.ValidValueTypes))
                        {
                            absTweenPlugin = new PlugString(propData.endValOrPlugin.ToString(),
                                propData.isRelative);
                            break;
                        }

                        break;
                    case "Int32":
                        if (ValidateValue(propData.endValOrPlugin, PlugInt.validValueTypes))
                        {
                            absTweenPlugin = new PlugInt((int)propData.endValOrPlugin, propData.isRelative);
                            break;
                        }

                        break;
                    default:
                        try
                        {
                            absTweenPlugin = new PlugFloat(Convert.ToSingle(propData.endValOrPlugin),
                                propData.isRelative);
                            break;
                        }
                        catch (Exception ex)
                        {
                            TweenWarning.Log("No valid plugin for animating \"" + target + "." +
                                             propData.propName + "\" (of type " +
                                             (property != null
                                                 ? property.PropertyType
                                                 : (object)fieldInfo.FieldType) +
                                             "). The tween for this property will not be created.");
                            continue;
                        }
                }

                if (absTweenPlugin == null)
                {
                    TweenWarning.Log("The end value set for \"" + target + "." + propData.propName +
                                     "\" tween is invalid. The tween for this property will not be created.");
                    continue;
                }
            }

            absTweenPlugin.Init(tweenObj, propData.propName, easeType, type, property, fieldInfo);
            tweenObj.plugins.Add(absTweenPlugin);
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
        if (propDatas == null)
            propDatas = new List<HOTPropData>();
        propDatas.Add(new HOTPropData(propName, endVal, isRelative));
        return this;
    }

    public TweenParms Id(string id)
    {
        base.id = id;
        return this;
    }

    public TweenParms IntId(int intId)
    {
        base.intId = intId;
        return this;
    }

    public TweenParms AutoKill(bool active)
    {
        autoKillOnComplete = active;
        return this;
    }

    public TweenParms UpdateType(UpdateType updateType)
    {
        base.updateType = updateType;
        return this;
    }

    public TweenParms TimeScale(float timeScale)
    {
        base.timeScale = timeScale;
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

    private class HOTPropData
    {
        public readonly string propName;
        public readonly object endValOrPlugin;
        public readonly bool isRelative;

        public HOTPropData(string propName, object endValOrPlugin, bool isRelative)
        {
            this.propName = propName;
            this.endValOrPlugin = endValOrPlugin;
            this.isRelative = isRelative;
        }
    }
}

}