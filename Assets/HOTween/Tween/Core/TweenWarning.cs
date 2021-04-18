using UnityEngine;

namespace Holoville.HOTween.Core {

internal static class TweenWarning
{
    internal static void Log(string p_message) =>
        Log(p_message, false);

    internal static void Log(string p_message, bool p_verbose)
    {
        Debug.LogWarning("HOTween : " + p_message);
    }
}

}