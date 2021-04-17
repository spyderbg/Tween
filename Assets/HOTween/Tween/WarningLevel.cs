namespace Holoville.HOTween
{
    /// <summary>
    /// Enumeration of the levels of warning that should be used to output messages in case of auto-resolved errors.
    /// </summary>
    public enum WarningLevel
    {
        /// <summary>No messages will be logged.</summary>
        None,

        /// <summary>
        /// Only important messages will be logged
        /// (this will exclude warnings when a tween is overwritten).
        /// </summary>
        Important,

        /// <summary>All messages will be logged.</summary>
        Verbose,
    }
}