using Holoville.HOTween.Core;
using Holoville.HOTween.Plugins;
using Holoville.HOTween.Plugins.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Holoville.HOTween
{
    /// <summary>
    /// Method chaining parameters for a <see cref="T:Holoville.HOTween.Tweener" />.
    /// </summary>
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
        private EaseType easeType = HOTween.defEaseType;
        private AnimationCurve easeAnimCurve;
        private float easeOvershootOrAmplitude = HOTween.defEaseOvershootOrAmplitude;
        private float easePeriod = HOTween.defEasePeriod;
        private float delay;
        private List<HOTPropData> propDatas;
        private bool isFrom;
        private TweenDelegate.TweenCallback onPluginOverwritten;
        private TweenDelegate.TweenCallbackWParms onPluginOverwrittenWParms;
        private object[] onPluginOverwrittenParms;

        /// <summary>
        /// Returns <c>true</c> if at least one property tween was added to these parameters,
        /// either via <c>Prop()</c> or <c>NewProp()</c>.
        /// </summary>
        public bool hasProps => propDatas != null;

        /// <summary>
        /// Initializes the given <see cref="T:Holoville.HOTween.Tweener" /> with the stored parameters.
        /// </summary>
        /// <param name="p_tweenObj">
        /// The <see cref="T:Holoville.HOTween.Tweener" /> to initialize.
        /// </param>
        /// <param name="p_target">
        /// The <see cref="T:Holoville.HOTween.Tweener" /> target.
        /// </param>
        internal void InitializeObject(Tweener p_tweenObj, object p_target)
        {
            InitializeOwner(p_tweenObj);
            if (speedBased && !easeSet)
                easeType = EaseType.Linear;
            p_tweenObj._pixelPerfect = pixelPerfect;
            p_tweenObj._speedBased = speedBased;
            p_tweenObj._easeType = easeType;
            p_tweenObj._easeAnimationCurve = easeAnimCurve;
            p_tweenObj._easeOvershootOrAmplitude = easeOvershootOrAmplitude;
            p_tweenObj._easePeriod = easePeriod;
            p_tweenObj._delay = p_tweenObj.delayCount = delay;
            p_tweenObj.isFrom = isFrom;
            p_tweenObj.onPluginOverwritten = onPluginOverwritten;
            p_tweenObj.onPluginOverwrittenWParms = onPluginOverwrittenWParms;
            p_tweenObj.onPluginOverwrittenParms = onPluginOverwrittenParms;
            p_tweenObj.plugins = new List<ABSTweenPlugin>();
            var type = p_target.GetType();
            FieldInfo p_fieldInfo = null;
            var count = propDatas.Count;
            for (var index = 0; index < count; ++index)
            {
                var propData = propDatas[index];
                var property = type.GetProperty(propData.propName);
                if (property == null)
                {
                    p_fieldInfo = type.GetField(propData.propName);
                    if (p_fieldInfo == null)
                    {
                        TweenWarning.Log("\"" + p_target + "." + propData.propName +
                                         "\" is missing, static, or not public. The tween for this property will not be created.");
                        continue;
                    }
                }

                ABSTweenPlugin absTweenPlugin;
                if (propData.endValOrPlugin is ABSTweenPlugin endValOrPlugin2)
                {
                    absTweenPlugin = endValOrPlugin2;
                    if (absTweenPlugin.ValidateTarget(p_target))
                    {
                        if (absTweenPlugin.initialized)
                            absTweenPlugin = absTweenPlugin.CloneBasic();
                    }
                    else
                    {
                        TweenWarning.Log(Utils.SimpleClassName(absTweenPlugin.GetType()) + " : Invalid target (" +
                                         p_target + "). The tween for this property will not be created.");
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
                        : (_TypeToShortString.ContainsKey(p_fieldInfo.FieldType)
                            ? _TypeToShortString[p_fieldInfo.FieldType]
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
                            if (ValidateValue(propData.endValOrPlugin, PlugQuaternion.validValueTypes))
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
                            if (ValidateValue(propData.endValOrPlugin, PlugString.validValueTypes))
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
                                TweenWarning.Log("No valid plugin for animating \"" + p_target + "." +
                                                 propData.propName + "\" (of type " +
                                                 (property != null
                                                     ? property.PropertyType
                                                     : (object)p_fieldInfo.FieldType) +
                                                 "). The tween for this property will not be created.");
                                continue;
                            }
                    }

                    if (absTweenPlugin == null)
                    {
                        TweenWarning.Log("The end value set for \"" + p_target + "." + propData.propName +
                                         "\" tween is invalid. The tween for this property will not be created.");
                        continue;
                    }
                }

                absTweenPlugin.Init(p_tweenObj, propData.propName, easeType, type, property, p_fieldInfo);
                p_tweenObj.plugins.Add(absTweenPlugin);
            }
        }

        /// <summary>
        /// Sets this tween so that it works with pixel perfect values.
        /// Only works with <see cref="T:UnityEngine.Vector3" />, <see cref="T:UnityEngine.Vector2" />, <see cref="T:System.Single" />,
        /// <see cref="T:Holoville.HOTween.Plugins.PlugVector3X" />, <see cref="T:Holoville.HOTween.Plugins.PlugVector3Y" />, <see cref="T:Holoville.HOTween.Plugins.PlugVector3Z" />
        /// plugins.
        /// </summary>
        /// <returns></returns>
        public TweenParms PixelPerfect()
        {
            pixelPerfect = true;
            return this;
        }

        /// <summary>
        /// Sets this tween to work by speed instead than time.
        /// When a tween is based on speed instead than time,
        /// duration is considered as the amount that the property will change every second,
        /// and ease is automatically set to Linear.
        /// In case of Vectors, the amount represents the vector length x second;
        /// in case of Quaternions, the amount represents the full rotation (360°) speed x second;
        /// in case of strings, the amount represents the amount of changed letters x second.
        /// </summary>
        public TweenParms SpeedBased() => SpeedBased(true);

        /// <summary>
        /// Sets whether to tween by speed or not.
        /// When a tween is based on speed instead than time,
        /// duration is considered as the amount that the property will change every second,
        /// and ease is automatically set to Linear.
        /// In case of Vectors, the amount represents the vector length x second;
        /// in case of strings, the amount represents the amount of changed letters x second.
        /// </summary>
        /// <param name="p_speedBased">
        /// If <c>true</c> this tween will work by speed instead than by time.
        /// </param>
        public TweenParms SpeedBased(bool p_speedBased)
        {
            speedBased = p_speedBased;
            return this;
        }

        /// <summary>
        /// Sets the ease type to use (default = <c>EaseType.easeOutQuad</c>).
        /// If you set this tween to use speed instead than time,
        /// this parameter becomes useless, because it will be managed internally.
        /// </summary>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        public TweenParms Ease(EaseType p_easeType) =>
            Ease(p_easeType, HOTween.defEaseOvershootOrAmplitude, HOTween.defEasePeriod);

        /// <summary>
        /// Sets the ease type to use (default = <c>EaseType.easeOutQuad</c>).
        /// If you set this tween to use speed instead than time,
        /// this parameter becomes useless, because it will be managed internally.
        /// </summary>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_overshoot">
        /// Eventual overshoot to use with Back easeType (default is 1.70158).
        /// </param>
        public TweenParms Ease(EaseType p_easeType, float p_overshoot) =>
            Ease(p_easeType, p_overshoot, HOTween.defEasePeriod);

        /// <summary>
        /// Sets the ease type to use (default = <c>EaseType.easeOutQuad</c>).
        /// If you set this tween to use speed instead than time,
        /// this parameter becomes useless, because it will be managed internally.
        /// </summary>
        /// <param name="p_easeType">
        /// The <see cref="T:Holoville.HOTween.EaseType" /> to use.
        /// </param>
        /// <param name="p_amplitude">
        /// Eventual amplitude to use with Elastic easeType (default is 0).
        /// </param>
        /// <param name="p_period">
        /// Eventual period to use with Elastic easeType (default is 0).
        /// </param>
        public TweenParms Ease(EaseType p_easeType, float p_amplitude, float p_period)
        {
            easeSet = true;
            easeType = p_easeType;
            easeOvershootOrAmplitude = p_amplitude;
            easePeriod = p_period;
            return this;
        }

        /// <summary>
        /// Sets the ease to use the given AnimationCurve.
        /// If you set this tween to use speed instead than time,
        /// this parameter becomes useless, because it will be managed internally.
        /// </summary>
        /// <param name="p_easeAnimationCurve">
        /// The <see cref="T:UnityEngine.AnimationCurve" /> to use.
        /// </param>
        public TweenParms Ease(AnimationCurve p_easeAnimationCurve)
        {
            easeSet = true;
            easeType = EaseType.AnimationCurve;
            easeAnimCurve = p_easeAnimationCurve;
            return this;
        }

        /// <summary>
        /// Sets the seconds of delay before the tween should start (default = <c>0</c>).
        /// </summary>
        /// <param name="p_delay">The seconds of delay.</param>
        public TweenParms Delay(float p_delay)
        {
            delay = p_delay;
            return this;
        }

        /// <summary>Sets the Tweener in a paused state.</summary>
        public TweenParms Pause() => Pause(true);

        /// <summary>Choose whether to set the Tweener in a paused state.</summary>
        public TweenParms Pause(bool p_pause)
        {
            isPaused = p_pause;
            return this;
        }

        /// <summary>
        /// Sets a property or field to tween,
        /// directly assigning the given <c>TweenPlugin</c> to it.
        /// Behaves as <c>Prop()</c>, but removes any other property tween previously set in this <see cref="T:Holoville.HOTween.TweenParms" />
        /// (useful if you want to reuse the same parameters with a new set of property tweens).
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_plugin">
        /// The <see cref="T:Holoville.HOTween.Plugins.Core.ABSTweenPlugin" /> to use.
        /// </param>
        public TweenParms NewProp(string p_propName, ABSTweenPlugin p_plugin) => NewProp(p_propName, p_plugin, false);

        /// <summary>
        /// Sets a property or field to tween.
        /// Behaves as <c>Prop()</c>, but removes any other property tween previously set in this <see cref="T:Holoville.HOTween.TweenParms" />
        /// (useful if you want to reuse the same parameters with a new set of property tweens).
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_endVal">
        /// The absolute end value the object should reach with the tween.
        /// </param>
        public TweenParms NewProp(string p_propName, object p_endVal) => NewProp(p_propName, p_endVal, false);

        /// <summary>
        /// Sets a property or field to tween.
        /// Behaves as <c>Prop()</c>, but removes any other property tween previously set in this <see cref="T:Holoville.HOTween.TweenParms" />
        /// (useful if you want to reuse the same parameters with a new set of property tweens).
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_endVal">
        /// The end value the object should reach with the tween.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c> treats the end value as relative, otherwise as absolute.
        /// </param>
        public TweenParms NewProp(string p_propName, object p_endVal, bool p_isRelative)
        {
            propDatas = null;
            return Prop(p_propName, p_endVal, p_isRelative);
        }

        /// <summary>
        /// Sets a property or field to tween,
        /// directly assigning the given <c>TweenPlugin</c> to it.
        /// Behaves as <c>NewProp()</c>, but without removing the other property tweens that were set in this <see cref="T:Holoville.HOTween.TweenParms" />.
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_plugin">
        /// The <see cref="T:Holoville.HOTween.Plugins.Core.ABSTweenPlugin" /> to use.
        /// </param>
        public TweenParms Prop(string p_propName, ABSTweenPlugin p_plugin) => Prop(p_propName, p_plugin, false);

        /// <summary>
        /// Sets a property or field to tween.
        /// Behaves as <c>NewProp()</c>, but without removing the other property tweens that were set in this <see cref="T:Holoville.HOTween.TweenParms" />.
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_endVal">
        /// The absolute end value the object should reach with the tween.
        /// </param>
        public TweenParms Prop(string p_propName, object p_endVal) => Prop(p_propName, p_endVal, false);

        /// <summary>
        /// Sets a property or field to tween.
        /// Behaves as <c>NewProp()</c>, but without removing the other property tweens that were set in this <see cref="T:Holoville.HOTween.TweenParms" />.
        /// </summary>
        /// <param name="p_propName">The name of the property.</param>
        /// <param name="p_endVal">
        /// The end value the object should reach with the tween.
        /// </param>
        /// <param name="p_isRelative">
        /// If <c>true</c> treats the end value as relative, otherwise as absolute.
        /// </param>
        public TweenParms Prop(string p_propName, object p_endVal, bool p_isRelative)
        {
            if (propDatas == null)
                propDatas = new List<HOTPropData>();
            propDatas.Add(new HOTPropData(p_propName, p_endVal, p_isRelative));
            return this;
        }

        /// <summary>
        /// Sets the ID of this Tweener (default = "").
        /// The same ID can be applied to multiple Tweeners, thus allowing for group operations.
        /// You can also use <c>IntId</c> instead of <c>Id</c> for faster operations.
        /// </summary>
        /// <param name="p_id">The ID for this Tweener.</param>
        public TweenParms Id(string p_id)
        {
            id = p_id;
            return this;
        }

        /// <summary>
        /// Sets the int ID of this Tweener (default = 0).
        /// The same intId can be applied to multiple Tweeners, thus allowing for group operations.
        /// The main difference from <c>Id</c> is that while <c>Id</c> is more legible, <c>IntId</c> allows for faster operations.
        /// </summary>
        /// <param name="p_intId">The int ID for this Tweener.</param>
        public TweenParms IntId(int p_intId)
        {
            intId = p_intId;
            return this;
        }

        /// <summary>
        /// Sets auto-kill behaviour for when the Tweener reaches its end (default = <c>false</c>).
        /// </summary>
        /// <param name="p_active">
        /// If <c>true</c> the Tweener is killed and removed from HOTween as soon as it's completed.
        /// If <c>false</c> doesn't remove this Tweener from HOTween when it is completed,
        /// and you will need to call an <c>HOTween.Kill</c> to remove this Tweener.
        /// </param>
        public TweenParms AutoKill(bool p_active)
        {
            autoKillOnComplete = p_active;
            return this;
        }

        /// <summary>
        /// Sets the type of update to use for this Tweener (default = <see cref="M:Holoville.HOTween.TweenParms.UpdateType(Holoville.HOTween.UpdateType)" /><c>.Update</c>).
        /// </summary>
        /// <param name="p_updateType">The type of update to use.</param>
        public TweenParms UpdateType(UpdateType p_updateType)
        {
            updateType = p_updateType;
            return this;
        }

        /// <summary>
        /// Sets the time scale that will be used by this Tweener.
        /// </summary>
        /// <param name="p_timeScale">The time scale to use.</param>
        public TweenParms TimeScale(float p_timeScale)
        {
            timeScale = p_timeScale;
            return this;
        }

        /// <summary>
        /// Sets the number of times the Tweener will run (default = <c>1</c>, meaning only one go and no other loops).
        /// </summary>
        /// <param name="p_loops">
        /// Number of loops (set it to <c>-1</c> or <see cref="F:System.Single.PositiveInfinity" /> to apply infinite loops).
        /// </param>
        public TweenParms Loops(int p_loops) => Loops(p_loops, HOTween.defLoopType);

        /// <summary>
        /// Sets the number of times the Tweener will run,
        /// and the type of loop behaviour to apply
        /// (default = <c>1</c>, <c>LoopType.Restart</c>).
        /// </summary>
        /// <param name="p_loops">
        /// Number of loops (set it to <c>-1</c> or <see cref="F:System.Single.PositiveInfinity" /> to apply infinite loops).
        /// </param>
        /// <param name="p_loopType">
        /// The <see cref="T:Holoville.HOTween.LoopType" /> behaviour to use.
        /// </param>
        public TweenParms Loops(int p_loops, LoopType p_loopType)
        {
            loops = p_loops;
            loopType = p_loopType;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener is started for the very first time.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnStart(TweenDelegate.TweenCallback p_function)
        {
            onStart = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener is started for the very first time.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnStart(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onStartWParms = p_function;
            onStartParms = p_funcParms;
            return this;
        }

        /// <summary>Function to call each time the Tweener is updated.</summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnUpdate(TweenDelegate.TweenCallback p_function)
        {
            onUpdate = p_function;
            return this;
        }

        /// <summary>Function to call each time the Tweener is updated.</summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnUpdate(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onUpdateWParms = p_function;
            onUpdateParms = p_funcParms;
            return this;
        }

        /// <summary>Function to call each time a plugin is updated.</summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnPluginUpdated(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPluginUpdatedWParms = p_function;
            onPluginUpdatedParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener switches from a playing state to a paused state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnPause(TweenDelegate.TweenCallback p_function)
        {
            onPause = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener switches from a playing state to a paused state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnPause(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPauseWParms = p_function;
            onPauseParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener switches from a paused state to a playing state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnPlay(TweenDelegate.TweenCallback p_function)
        {
            onPlay = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Tweener switches from a paused state to a playing state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnPlay(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPlayWParms = p_function;
            onPlayParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call each time the Tweener is rewinded from a non-rewinded state
        /// (either because of a direct call to Rewind,
        /// or because the tween's virtual playehead reached the start due to a playing backwards behaviour).
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnRewinded(TweenDelegate.TweenCallback p_function)
        {
            onRewinded = p_function;
            return this;
        }

        /// <summary>
        /// Function to call each time the Tweener is rewinded from a non-rewinded state
        /// (either because of a direct call to Rewind,
        /// or because the tween's virtual playehead reached the start due to a playing backwards behaviour).
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnRewinded(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onRewindedWParms = p_function;
            onRewindedParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call each time a single loop of the Tweener is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnStepComplete(TweenDelegate.TweenCallback p_function)
        {
            onStepComplete = p_function;
            return this;
        }

        /// <summary>
        /// Function to call each time a single loop of the Tweener is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnStepComplete(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onStepCompleteWParms = p_function;
            onStepCompleteParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Uses sendMessage to call the method named p_methodName
        /// on every MonoBehaviour in the p_sendMessageTarget GameObject.
        /// </summary>
        /// <param name="p_sendMessageTarget">GameObject to target for sendMessage</param>
        /// <param name="p_methodName">Name of the method to call</param>
        /// <param name="p_value">Eventual additional parameter</param>
        /// <param name="p_options">SendMessageOptions</param>
        public TweenParms OnStepComplete(
            GameObject p_sendMessageTarget,
            string p_methodName,
            object p_value = null,
            SendMessageOptions p_options = SendMessageOptions.RequireReceiver)
        {
            onStepCompleteWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
            onStepCompleteParms = new object[4]
            {
                p_sendMessageTarget,
                p_methodName,
                p_value,
                p_options
            };
            return this;
        }

        /// <summary>
        /// Function to call when the full Tweener, loops included, is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnComplete(TweenDelegate.TweenCallback p_function)
        {
            onComplete = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the full Tweener, loops included, is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnComplete(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onCompleteWParms = p_function;
            onCompleteParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Uses sendMessage to call the method named p_methodName
        /// on every MonoBehaviour in the p_sendMessageTarget GameObject.
        /// </summary>
        /// <param name="p_sendMessageTarget">GameObject to target for sendMessage</param>
        /// <param name="p_methodName">Name of the method to call</param>
        /// <param name="p_value">Eventual additional parameter</param>
        /// <param name="p_options">SendMessageOptions</param>
        public TweenParms OnComplete(
            GameObject p_sendMessageTarget,
            string p_methodName,
            object p_value = null,
            SendMessageOptions p_options = SendMessageOptions.RequireReceiver)
        {
            onCompleteWParms = new TweenDelegate.TweenCallbackWParms(HOTween.DoSendMessage);
            onCompleteParms = new object[4]
            {
                p_sendMessageTarget,
                p_methodName,
                p_value,
                p_options
            };
            return this;
        }

        /// <summary>
        /// Function to call when one of the plugins used in the tween gets overwritten
        /// (available only if OverwriteManager is active).
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public TweenParms OnPluginOverwritten(TweenDelegate.TweenCallback p_function)
        {
            onPluginOverwritten = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when one of the plugins used in the tween gets overwritten
        /// (available only if OverwriteManager is active).
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public TweenParms OnPluginOverwritten(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPluginOverwrittenWParms = p_function;
            onPluginOverwrittenParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Keeps the given component enabled while the tween is playing
        /// </summary>
        public TweenParms KeepEnabled(Behaviour p_target)
        {
            if (p_target == null)
            {
                manageBehaviours = false;
                return this;
            }

            return KeepEnabled(new Behaviour[1] {p_target}, true);
        }

        /// <summary>
        /// Keeps the given gameObject activated while the tween is playing
        /// </summary>
        public TweenParms KeepEnabled(GameObject p_target)
        {
            if (p_target == null)
            {
                manageGameObjects = false;
                return this;
            }

            return KeepEnabled(new GameObject[1] {p_target}, true);
        }

        /// <summary>
        /// Keeps the given components enabled while the tween is playing
        /// </summary>
        public TweenParms KeepEnabled(Behaviour[] p_targets) => KeepEnabled(p_targets, true);

        /// <summary>
        /// Keeps the given GameObject activated while the tween is playing
        /// </summary>
        public TweenParms KeepEnabled(GameObject[] p_targets) => KeepEnabled(p_targets, true);

        /// <summary>
        /// Keeps the given component disabled while the tween is playing
        /// </summary>
        public TweenParms KeepDisabled(Behaviour p_target)
        {
            if (p_target == null)
            {
                manageBehaviours = false;
                return this;
            }

            return KeepEnabled(new Behaviour[1] {p_target}, false);
        }

        /// <summary>
        /// Keeps the given GameObject disabled while the tween is playing
        /// </summary>
        public TweenParms KeepDisabled(GameObject p_target)
        {
            if (p_target == null)
            {
                manageGameObjects = false;
                return this;
            }

            return KeepEnabled(new GameObject[1] {p_target}, false);
        }

        /// <summary>
        /// Keeps the given components disabled while the tween is playing
        /// </summary>
        public TweenParms KeepDisabled(Behaviour[] p_targets) => KeepEnabled(p_targets, false);

        /// <summary>
        /// Keeps the given GameObject disabled while the tween is playing
        /// </summary>
        public TweenParms KeepDisabled(GameObject[] p_targets) => KeepEnabled(p_targets, false);

        private TweenParms KeepEnabled(Behaviour[] p_targets, bool p_enabled)
        {
            manageBehaviours = true;
            if (p_enabled)
                managedBehavioursOn = p_targets;
            else
                managedBehavioursOff = p_targets;
            return this;
        }

        private TweenParms KeepEnabled(GameObject[] p_targets, bool p_enabled)
        {
            manageGameObjects = true;
            if (p_enabled)
                managedGameObjectsOn = p_targets;
            else
                managedGameObjectsOff = p_targets;
            return this;
        }

        /// <summary>Used by HOTween.From to set isFrom property.</summary>
        /// <returns>
        /// A <see cref="T:Holoville.HOTween.TweenParms" />
        /// </returns>
        internal TweenParms IsFrom()
        {
            isFrom = true;
            return this;
        }

        private static bool ValidateValue(object p_val, Type[] p_validVals) =>
            Array.IndexOf<Type>(p_validVals, p_val.GetType()) != -1;

        private class HOTPropData
        {
            public readonly string propName;
            public readonly object endValOrPlugin;
            public readonly bool isRelative;

            public HOTPropData(string p_propName, object p_endValOrPlugin, bool p_isRelative)
            {
                propName = p_propName;
                endValOrPlugin = p_endValOrPlugin;
                isRelative = p_isRelative;
            }
        }
    }
}