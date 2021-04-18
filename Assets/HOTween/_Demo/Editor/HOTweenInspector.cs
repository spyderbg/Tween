using Holoville.HOTween.Core;
using Holoville.HOTween.Editor.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Holoville.HOTween.Editor {

[CustomEditor(typeof(HOTween))]
public class HOTweenInspector : UnityEditor.Editor
{
    private const int kLabelsWidth = 150;
    private const int kFieldsWidth = 60;

    public override void OnInspectorGUI()
    {
        HOGUIStyle.InitGUI();
        EditorGUIUtility.LookLikeControls((float)kLabelsWidth, (float)kFieldsWidth);
        GUILayout.Space(4f);
        GUILayout.Label("HOTween v" + HOTween.kVersion);
        GUILayout.Space(4f);

        var tweenInfos = HOTween.GetTweenInfos();
        if (tweenInfos == null)
            GUILayout.Label("No tweens");
        else
            DrawTweens(tweenInfos);
    }

    private void DrawTweens(TweenInfo[] tweenInfos)
    {
        var length = tweenInfos.Length;
        var tweenInfoList1 = new List<TweenInfo>();
        var tweenInfoList2 = new List<TweenInfo>();
        var tweenInfoList3 = new List<TweenInfo>();
        var tweenInfoList4 = new List<TweenInfo>();
        foreach (var tweenInfo in tweenInfos)
        {
            if (!tweenInfo.isEnabled)
                tweenInfoList4.Add(tweenInfo);
            else if (tweenInfo.isComplete)
                tweenInfoList3.Add(tweenInfo);
            else if (tweenInfo.isPaused)
                tweenInfoList2.Add(tweenInfo);
            else
                tweenInfoList1.Add(tweenInfo);
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Tweens (tot - running/paused/completed/disabled):\n" + length + " - " +
                        tweenInfoList1.Count + "//" + tweenInfoList2.Count + "/" +
                        tweenInfoList3.Count + "/" + tweenInfoList4.Count);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(4f);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Play All", HOGUIStyle.BtTinyStyle, GUILayout.Width(76f)))
            HOTween.Play();
        if (GUILayout.Button("Pause All", HOGUIStyle.BtTinyStyle, GUILayout.Width(76f)))
            HOTween.Pause();
        if (GUILayout.Button("Complete All", HOGUIStyle.BtTinyStyle, GUILayout.Width(86f)))
            HOTween.Complete();
        if (GUILayout.Button("Kill All", HOGUIStyle.BtTinyStyle, GUILayout.Width(76f)))
            HOTween.Kill();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        for (var index = 0; index < 4; ++index)
        {
            TweenGroup twGroup;
            List<TweenInfo> tweenInfoList5;
            string str;
            switch (index)
            {
                case 0:
                    twGroup = TweenGroup.Running;
                    tweenInfoList5 = tweenInfoList1;
                    str = "Running";
                    break;
                case 1:
                    twGroup = TweenGroup.Paused;
                    tweenInfoList5 = tweenInfoList2;
                    str = "Paused";
                    break;
                case 2:
                    twGroup = TweenGroup.Completed;
                    tweenInfoList5 = tweenInfoList3;
                    str = "Completed but not killed";
                    break;
                default:
                    twGroup = TweenGroup.Disabled;
                    tweenInfoList5 = tweenInfoList4;
                    str = "Disabled";
                    break;
            }

            if (tweenInfoList5.Count == 0) continue;
            
            GUILayout.Space(8f);
            
            GUILayout.BeginVertical(HOGUIStyle.BoxStyleRegular);
            GUILayout.BeginHorizontal();
            GUILayout.Label(str + " Tweens (" + (object)tweenInfoList5.Count + ")", HOGUIStyle.TitleStyle, GUILayout.ExpandWidth(false));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Click a target to select it");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(6f);
            
            foreach (var twInfo in tweenInfoList5)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                if (twInfo.isSequence)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("[Sequence]", HOGUIStyle.LabelSmallStyle);
                    if (twGroup != TweenGroup.Disabled)
                        DrawTargetButtons(twInfo, twGroup);
                    GUILayout.EndHorizontal();
                    
                    DrawInfo(twInfo);
                    
                    foreach (var target in twInfo.targets)
                        DrawTarget(twInfo, target, twGroup, true);
                }
                else
                {
                    DrawTarget(twInfo, twInfo.targets[0], twGroup, false);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }

    private void DrawTarget(TweenInfo twInfo, object twTarget, TweenGroup twGroup, bool isSequenced)
    {
        GUILayout.BeginHorizontal();
        if (isSequenced)
            GUILayout.Space(9f);
        if (GUILayout.Button(twTarget.ToString(), HOGUIStyle.BtLabelStyle))
            SelectTargetGameObject(twTarget);
        if (!isSequenced && twGroup != TweenGroup.Disabled)
            DrawTargetButtons(twInfo, twGroup);
        GUILayout.EndHorizontal();
        
        if (isSequenced || twGroup == TweenGroup.Disabled) return;
        
        DrawInfo(twInfo);
    }

    private void DrawInfo(TweenInfo twInfo) => GUILayout.Label(
        "Id: " + (twInfo.tween.Id != "" ? twInfo.tween.Id : (object)"-") + ", Loops: " +
        twInfo.tween.CompletedLoops + "/" + twInfo.tween.Loops, HOGUIStyle.LabelSmallStyle);

    private void DrawTargetButtons(TweenInfo twInfo, TweenGroup twGroup)
    {
        if (GUILayout.Button(twGroup == TweenGroup.Running ? "Pause" : "Play", HOGUIStyle.BtTinyStyle, GUILayout.Width(45f)))
        {
            if (twGroup == TweenGroup.Running)
                twInfo.tween.Pause();
            else
                twInfo.tween.Play();
        }

        if (!GUILayout.Button("Kill", HOGUIStyle.BtTinyStyle, GUILayout.Width(32f))) return;
        
        twInfo.tween.Kill();
    }

    private void SelectTargetGameObject(object obj)
    {
        var gameObject = (GameObject)null;
        var monoBehaviour = obj as MonoBehaviour;
        if (monoBehaviour != null)
        {
            gameObject = monoBehaviour.gameObject;
        }
        else
        {
            var component = obj as Component;
            if (component != null)
                gameObject = component.gameObject;
        }

        if (!(gameObject != null)) return;
        
        Selection.activeGameObject = gameObject;
    }

    private enum TweenGroup
    {
        Running,
        Paused,
        Completed,
        Disabled,
    }
}

}