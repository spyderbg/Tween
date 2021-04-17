using System;

namespace FastDynamicMemberAccessor
{
    /// <summary>PropertyAccessorException class.</summary>
    internal class MemberAccessorException : Exception
    {
        internal MemberAccessorException(string message)
            : base(message)
        {
        }
    }
}