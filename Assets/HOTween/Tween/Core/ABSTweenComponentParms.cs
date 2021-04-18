using UnityEngine;

namespace Holoville.HOTween.Core
{
    /// <summary>Base class for all HOTParms.</summary>
    public abstract class ABSTweenComponentParms
    {
        /// <summary>ID.</summary>
        protected string Id = "";

        /// <summary>Int ID.</summary>
        protected int IntId = -1;

        /// <summary>Auto kill on complete.</summary>
        protected bool AutoKillOnComplete = true;

        /// <summary>Update type.</summary>
        protected UpdateType UpdateType = HOTween.kDefUpdateType;

        /// <summary>Time scale.</summary>
        protected float TimeScale = HOTween.kDefTimeScale;

        /// <summary>Loops</summary>
        protected int loops = 1;

        /// <summary>Loop type.</summary>
        protected LoopType loopType = HOTween.kDefLoopType;

        /// <summary>Paused.</summary>
        protected bool isPaused;

        /// <summary>On start.</summary>
        protected TweenDelegate.TweenCallback onStart;

        /// <summary>On start with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onStartWParms;

        /// <summary>On start parameters.</summary>
        protected object[] onStartParms;

        /// <summary>On update.</summary>
        protected TweenDelegate.TweenCallback onUpdate;

        /// <summary>On update with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onUpdateWParms;

        /// <summary>On update parameters.</summary>
        protected object[] onUpdateParms;

        /// <summary>On plugin results.</summary>
        protected TweenDelegate.TweenCallback onPluginUpdated;

        /// <summary>On plugin results with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onPluginUpdatedWParms;

        /// <summary>On plugin results parameters.</summary>
        protected object[] onPluginUpdatedParms;

        /// <summary>On pause.</summary>
        protected TweenDelegate.TweenCallback onPause;

        /// <summary>On pause with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onPauseWParms;

        /// <summary>On pause parameters.</summary>
        protected object[] onPauseParms;

        /// <summary>On play.</summary>
        protected TweenDelegate.TweenCallback onPlay;

        /// <summary>On play with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onPlayWParms;

        /// <summary>On play parameters.</summary>
        protected object[] onPlayParms;

        /// <summary>On rewinded.</summary>
        protected TweenDelegate.TweenCallback onRewinded;

        /// <summary>On rewinded with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onRewindedWParms;

        /// <summary>On rewinded parameters.</summary>
        protected object[] onRewindedParms;

        /// <summary>On step complete.</summary>
        protected TweenDelegate.TweenCallback onStepComplete;

        /// <summary>On step complete with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onStepCompleteWParms;

        /// <summary>On step complete parameters.</summary>
        protected object[] onStepCompleteParms;

        /// <summary>On complete.</summary>
        protected TweenDelegate.TweenCallback onComplete;

        /// <summary>On complete with parms.</summary>
        protected TweenDelegate.TweenCallbackWParms onCompleteWParms;

        /// <summary>On complete parameters.</summary>
        protected object[] onCompleteParms;

        /// <summary>True if there are behaviours to manage</summary>
        protected bool manageBehaviours;

        /// <summary>True if there are gameObject to manage</summary>
        protected bool manageGameObjects;

        /// <summary>Behaviours to activate</summary>
        protected Behaviour[] managedBehavioursOn;

        /// <summary>Behaviours to deactivate</summary>
        protected Behaviour[] managedBehavioursOff;

        /// <summary>GameObjects to activate</summary>
        protected GameObject[] managedGameObjectsOn;

        /// <summary>GameObejcts to deactivate</summary>
        protected GameObject[] managedGameObjectsOff;

        /// <summary>
        /// Initializes the given owner with the stored parameters.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="T:Holoville.HOTween.Core.ABSTweenComponent" /> to initialize.
        /// </param>
        protected void InitializeOwner(ABSTweenComponent owner)
        {
            owner.Id = Id;
            owner.IntId = IntId;
            owner.AutoKillOnComplete = AutoKillOnComplete;
            owner.UpdateType = UpdateType;
            owner.TimeScale = TimeScale;
            owner.LoopsVal = loops;
            owner.LoopType = loopType;
            owner.IsPaused = isPaused;
            owner.onStart = onStart;
            owner.onStartWParms = onStartWParms;
            owner.onStartParms = onStartParms;
            owner.onUpdate = onUpdate;
            owner.onUpdateWParms = onUpdateWParms;
            owner.onUpdateParms = onUpdateParms;
            owner.onPluginUpdated = onPluginUpdated;
            owner.onPluginUpdatedWParms = onPluginUpdatedWParms;
            owner.onPluginUpdatedParms = onPluginUpdatedParms;
            owner.onPause = onPause;
            owner.onPauseWParms = onPauseWParms;
            owner.onPauseParms = onPauseParms;
            owner.onPlay = onPlay;
            owner.onPlayWParms = onPlayWParms;
            owner.onPlayParms = onPlayParms;
            owner.onRewinded = onRewinded;
            owner.onRewindedWParms = onRewindedWParms;
            owner.onRewindedParms = onRewindedParms;
            owner.onStepComplete = onStepComplete;
            owner.onStepCompleteWParms = onStepCompleteWParms;
            owner.onStepCompleteParms = onStepCompleteParms;
            owner.onComplete = onComplete;
            owner.onCompleteWParms = onCompleteWParms;
            owner.onCompleteParms = onCompleteParms;
            owner.ManageBehaviours = manageBehaviours;
            owner.ManageGameObjects = manageGameObjects;
            owner.ManagedBehavioursOn = managedBehavioursOn;
            owner.ManagedBehavioursOff = managedBehavioursOff;
            owner.ManagedGameObjectsOn = managedGameObjectsOn;
            owner.ManagedGameObjectsOff = managedGameObjectsOff;
            if (manageBehaviours)
            {
                var length = (managedBehavioursOn != null ? managedBehavioursOn.Length : 0) +
                             (managedBehavioursOff != null ? managedBehavioursOff.Length : 0);
                owner.ManagedBehavioursOriginalState = new bool[length];
            }

            if (!manageGameObjects)
                return;
            var length1 = (managedGameObjectsOn != null ? managedGameObjectsOn.Length : 0) +
                          (managedGameObjectsOff != null ? managedGameObjectsOff.Length : 0);
            owner.ManagedGameObjectsOriginalState = new bool[length1];
        }
    }
}