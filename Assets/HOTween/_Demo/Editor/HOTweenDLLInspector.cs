using UnityEditor;
using UnityEngine;

namespace Holoville.HOTween.Editor {

[CustomEditor(typeof(Object))]
public class HOTweenDLLInspector : UnityEditor.Editor
{
    private const string kLibraryName     = "HOTween";
    private const string kLibraryFullName = "HOTween.dll";
    
    private bool _stylesSet;
    private GUIStyle _wordWrapStyle;

    public override void OnInspectorGUI()
    {
        if (target.name == kLibraryName || target.name == kLibraryFullName)
        {
            GUI.enabled = true;
            
            if (!_stylesSet)
            {
                _stylesSet = true;
                _wordWrapStyle = new GUIStyle(GUI.skin.label);
                _wordWrapStyle.wordWrap = true;
            }

            GUILayout.Label(
                "HOTween v" + HOTween.kVersion +
                "\n\nThis is the default version of HOTween. If you need compatibility with Windows 8 Store/Phone or iOS with max stripping level, download HOTweenMicro from HOTween's website.",
                _wordWrapStyle);
        }
        else
        {
            DrawDefaultInspector();
        }
    }
}

}