using UnityEngine;

namespace Holoville.HOTween.Core
{
    /// <summary>
    /// Used internally to generate warnings that are managed without throwing exceptions.
    /// </summary>
    internal static class TweenWarning
    {
        internal static void Log(string p_message) => Log(p_message, false);

        internal static void Log(string p_message, bool p_verbose)
        {
            if (HOTween.warningLevel == WarningLevel.None || p_verbose && HOTween.warningLevel != WarningLevel.Verbose)
                return;
            Debug.LogWarning("HOTween : " + p_message);
        }
    }
}