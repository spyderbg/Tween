using Holoville.HOTween.Core;
using UnityEngine;

namespace Holoville.HOTween
{
    /// <summary>
    /// Method chaining parameters for a <see cref="T:Holoville.HOTween.Sequence" />.
    /// </summary>
    public class SequenceParms : ABSTweenComponentParms
    {
        /// <summary>
        /// Initializes the given <see cref="T:Holoville.HOTween.Sequence" /> with the stored parameters.
        /// </summary>
        /// <param name="p_sequence">
        /// The <see cref="T:Holoville.HOTween.Sequence" /> to initialize.
        /// </param>
        internal void InitializeSequence(Sequence p_sequence) => InitializeOwner(p_sequence);

        /// <summary>
        /// Sets the ID of this Sequence (default = "").
        /// The same ID can be applied to multiple Sequences, thus allowing for group operations.
        /// You can also use <c>IntId</c> instead of <c>Id</c> for faster operations.
        /// </summary>
        /// <param name="p_id">The ID for this Sequence.</param>
        public SequenceParms Id(string p_id)
        {
            base.Id = p_id;
            return this;
        }

        /// <summary>
        /// Sets the int ID of this Tweener (default = 0).
        /// The same intId can be applied to multiple Tweeners, thus allowing for group operations.
        /// The main difference from <c>Id</c> is that while <c>Id</c> is more legible, <c>IntId</c> allows for faster operations.
        /// </summary>
        /// <param name="p_intId">The int ID for this Tweener.</param>
        public SequenceParms IntId(int p_intId)
        {
            base.IntId = p_intId;
            return this;
        }

        /// <summary>
        /// Sets auto-kill behaviour for when the Sequence reaches its end (default = <c>false</c>).
        /// </summary>
        /// <param name="p_active">
        /// If <c>true</c> the Sequence is killed and removed from HOTween as soon as it's completed.
        /// If <c>false</c> doesn't remove this Sequence from HOTween when it is completed,
        /// and you will need to call an <c>HOTween.Kill</c> to remove this Sequence.
        /// </param>
        public SequenceParms AutoKill(bool p_active)
        {
            AutoKillOnComplete = p_active;
            return this;
        }

        /// <summary>
        /// Sets the type of update to use for this Sequence (default = <see cref="M:Holoville.HOTween.SequenceParms.UpdateType(Holoville.HOTween.UpdateType)" /><c>.Update</c>).
        /// </summary>
        /// <param name="p_updateType">The type of update to use.</param>
        public SequenceParms UpdateType(UpdateType p_updateType)
        {
            base.UpdateType = p_updateType;
            return this;
        }

        /// <summary>
        /// Sets the time scale that will be used by this Sequence.
        /// </summary>
        /// <param name="p_timeScale">The time scale to use.</param>
        public SequenceParms TimeScale(float p_timeScale)
        {
            base.TimeScale = p_timeScale;
            return this;
        }

        /// <summary>
        /// Sets the number of times the Sequence will run (default = <c>1</c>, meaning only one go and no other loops).
        /// </summary>
        /// <param name="p_loops">
        /// Number of loops (set it to <c>-1</c> or <see cref="F:System.Single.PositiveInfinity" /> to apply infinite loops).
        /// </param>
        public SequenceParms Loops(int p_loops) => Loops(p_loops, HOTween.kDefLoopType);

        /// <summary>
        /// Sets the number of times the Sequence will run,
        /// and the type of loop behaviour to apply
        /// (default = <c>1</c>, <c>LoopType.Restart</c>).
        /// </summary>
        /// <param name="p_loops">
        /// Number of loops (set it to <c>-1</c> or <see cref="F:System.Single.PositiveInfinity" /> to apply infinite loops).
        /// </param>
        /// <param name="p_loopType">
        /// The <see cref="T:Holoville.HOTween.LoopType" /> behaviour to use.
        /// Note the <see cref="F:Holoville.HOTween.LoopType.Incremental" /> is available, but as an experimental feature.
        /// It works with simple Sequences, but you should check that your animation
        /// works as intended with more complex Sequences.
        /// </param>
        public SequenceParms Loops(int p_loops, LoopType p_loopType)
        {
            loops = p_loops;
            loopType = p_loopType;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence is started for the very first time.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnStart(TweenDelegate.TweenCallback p_function)
        {
            onStart = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence is started for the very first time.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnStart(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onStartWParms = p_function;
            onStartParms = p_funcParms;
            return this;
        }

        /// <summary>Function to call each time the Sequence is updated.</summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnUpdate(TweenDelegate.TweenCallback p_function)
        {
            onUpdate = p_function;
            return this;
        }

        /// <summary>Function to call each time the Sequence is updated.</summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnUpdate(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onUpdateWParms = p_function;
            onUpdateParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence switches from a playing state to a paused state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnPause(TweenDelegate.TweenCallback p_function)
        {
            onPause = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence switches from a playing state to a paused state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnPause(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPauseWParms = p_function;
            onPauseParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence switches from a paused state to a playing state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnPlay(TweenDelegate.TweenCallback p_function)
        {
            onPlay = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the Sequence switches from a paused state to a playing state.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnPlay(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onPlayWParms = p_function;
            onPlayParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call each time the sequence is rewinded from a non-rewinded state
        /// (either because of a direct call to Rewind,
        /// or because the tween's virtual playehead reached the start due to a playing backwards behaviour).
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnRewinded(TweenDelegate.TweenCallback p_function)
        {
            onRewinded = p_function;
            return this;
        }

        /// <summary>
        /// Function to call each time the sequence is rewinded from a non-rewinded state
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
        public SequenceParms OnRewinded(
            TweenDelegate.TweenCallbackWParms p_function,
            params object[] p_funcParms)
        {
            onRewindedWParms = p_function;
            onRewindedParms = p_funcParms;
            return this;
        }

        /// <summary>
        /// Function to call each time a single loop of the Sequence is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnStepComplete(TweenDelegate.TweenCallback p_function)
        {
            onStepComplete = p_function;
            return this;
        }

        /// <summary>
        /// Function to call each time a single loop of the Sequence is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnStepComplete(
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
        public SequenceParms OnStepComplete(
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
        /// Function to call when the full Sequence, loops included, is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call, who must return <c>void</c> and accept no parameters.
        /// </param>
        public SequenceParms OnComplete(TweenDelegate.TweenCallback p_function)
        {
            onComplete = p_function;
            return this;
        }

        /// <summary>
        /// Function to call when the full Sequence, loops included, is completed.
        /// </summary>
        /// <param name="p_function">
        /// The function to call.
        /// It must return <c>void</c> and has to accept a single parameter of type <see cref="T:Holoville.HOTween.TweenEvent" />.
        /// </param>
        /// <param name="p_funcParms">
        /// Additional comma separated parameters to pass to the function.
        /// </param>
        public SequenceParms OnComplete(
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
        public SequenceParms OnComplete(
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
        /// Keeps the given component enabled while the tween is playing
        /// </summary>
        public SequenceParms KeepEnabled(Behaviour p_target)
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
        public SequenceParms KeepEnabled(GameObject p_target)
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
        public SequenceParms KeepEnabled(Behaviour[] p_targets) => KeepEnabled(p_targets, true);

        /// <summary>
        /// Keeps the given GameObject activated while the tween is playing
        /// </summary>
        public SequenceParms KeepEnabled(GameObject[] p_targets) => KeepEnabled(p_targets, true);

        /// <summary>
        /// Keeps the given component disabled while the tween is playing
        /// </summary>
        public SequenceParms KeepDisabled(Behaviour p_target)
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
        public SequenceParms KeepDisabled(GameObject p_target)
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
        public SequenceParms KeepDisabled(Behaviour[] p_targets) => KeepEnabled(p_targets, false);

        /// <summary>
        /// Keeps the given GameObject disabled while the tween is playing
        /// </summary>
        public SequenceParms KeepDisabled(GameObject[] p_targets) => KeepEnabled(p_targets, false);

        private SequenceParms KeepEnabled(Behaviour[] p_targets, bool p_enabled)
        {
            manageBehaviours = true;
            if (p_enabled)
                managedBehavioursOn = p_targets;
            else
                managedBehavioursOff = p_targets;
            return this;
        }

        private SequenceParms KeepEnabled(GameObject[] p_targets, bool p_enabled)
        {
            manageGameObjects = true;
            if (p_enabled)
                managedGameObjectsOn = p_targets;
            else
                managedGameObjectsOff = p_targets;
            return this;
        }
    }
}