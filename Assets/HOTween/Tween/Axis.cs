using System;

namespace Holoville.HOTween
{
    /// <summary>Enumeration of axis.</summary>
    [Flags]
    public enum Axis
    {
        /// <summary>No axis.</summary>
        None = 0,

        /// <summary>X axis.</summary>
        X = 2,

        /// <summary>Y axis.</summary>
        Y = 4,

        /// <summary>Z axis.</summary>
        Z = 8,

        /// <summary>W axis.</summary>
        W = 16, // 0x00000010
    }
}