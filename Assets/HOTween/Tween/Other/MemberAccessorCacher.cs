using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastDynamicMemberAccessor
{
    /// <summary>Cache manager for James Nies' MemberAccessor classes</summary>
    internal static class MemberAccessorCacher
    {
        private static Dictionary<Type, Dictionary<string, MemberAccessor>> dcMemberAccessors;

        /// <summary>
        /// Returns the cached memberAccessor if it alread exists,
        /// or calls MemberAccessor.Make and caches and returns the newly created MemberAccessor.
        /// </summary>
        internal static MemberAccessor Make(Type p_targetType, string p_propName, PropertyInfo p_propertyInfo, FieldInfo p_fieldInfo)
        {
            if (dcMemberAccessors != null && dcMemberAccessors.ContainsKey(p_targetType) &&
                dcMemberAccessors[p_targetType].ContainsKey(p_propName))
                return dcMemberAccessors[p_targetType][p_propName];
            if (dcMemberAccessors == null)
                dcMemberAccessors = new Dictionary<Type, Dictionary<string, MemberAccessor>>();
            if (!dcMemberAccessors.ContainsKey(p_targetType))
                dcMemberAccessors.Add(p_targetType, new Dictionary<string, MemberAccessor>());
            var dcMemberAccessor = dcMemberAccessors[p_targetType];
            var memberAccessor = MemberAccessor.Make(p_propertyInfo, p_fieldInfo);
            dcMemberAccessor.Add(p_propName, memberAccessor);
            return memberAccessor;
        }

        /// <summary>Clears the cache.</summary>
        internal static void Clear() => dcMemberAccessors = null;
    }
}