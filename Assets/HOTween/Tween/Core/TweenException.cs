using System;

namespace Holoville.HOTween.Core
{
    /// <summary>
    /// HOTweenException class.
    /// A new <c>HOTweenException</c> is thrown each time HOTween encounters an error.
    /// </summary>
    public class TweenException : Exception
    {
        /// <summary>
        /// Creates a new HOTweenException with the given message.
        /// </summary>
        /// <param name="p_message">The exception message.</param>
        public TweenException(string p_message)
            : base(p_message)
        {
        }
    }
}