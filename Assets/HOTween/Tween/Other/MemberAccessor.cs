using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FastDynamicMemberAccessor
{
    internal abstract class MemberAccessor : IMemberAccessor
    {
        private const string emmitedTypeName = "Member";
        protected readonly Type _targetType;
        protected readonly string _fieldName;
        protected static readonly Hashtable s_TypeHash = new Hashtable();
        private IMemberAccessor _emittedMemberAccessor;

        /// <summary>Creates a new member accessor.</summary>
        /// <param name="member">Member</param>
        protected MemberAccessor(MemberInfo member)
        {
            _targetType = member.ReflectedType;
            _fieldName = member.Name;
        }

        /// <summary>
        /// Added by Daniele Giardini for HOTween,
        /// because if a Make is called we already know that a PropertyInfo or FieldInfo exist,
        /// and we can directly pass them as parameters.
        /// </summary>
        internal static MemberAccessor Make(PropertyInfo p_propertyInfo, FieldInfo p_fieldInfo)
        {
            return p_propertyInfo != null
                ? new PropertyAccessor(p_propertyInfo)
                : (MemberAccessor)new FieldAccessor(p_fieldInfo);
        }

        /// <summary>
        /// Thanks to Ben Ratzlaff for this snippet of code
        /// http://www.codeproject.com/cs/miscctrl/CustomPropGrid.asp
        /// 
        /// "Initialize a private hashtable with type-opCode pairs
        /// so i dont have to write a long if/else statement when outputting msil"
        /// </summary>
        static MemberAccessor()
        {
            s_TypeHash[typeof(sbyte)] = OpCodes.Ldind_I1;
            s_TypeHash[typeof(byte)] = OpCodes.Ldind_U1;
            s_TypeHash[typeof(char)] = OpCodes.Ldind_U2;
            s_TypeHash[typeof(short)] = OpCodes.Ldind_I2;
            s_TypeHash[typeof(ushort)] = OpCodes.Ldind_U2;
            s_TypeHash[typeof(int)] = OpCodes.Ldind_I4;
            s_TypeHash[typeof(uint)] = OpCodes.Ldind_U4;
            s_TypeHash[typeof(long)] = OpCodes.Ldind_I8;
            s_TypeHash[typeof(ulong)] = OpCodes.Ldind_I8;
            s_TypeHash[typeof(bool)] = OpCodes.Ldind_I1;
            s_TypeHash[typeof(double)] = OpCodes.Ldind_R8;
            s_TypeHash[typeof(float)] = OpCodes.Ldind_R4;
        }

        /// <summary>Gets the member value from the specified target.</summary>
        /// <param name="target">Target object.</param>
        /// <returns>Member value.</returns>
        public object Get(object target)
        {
            if (!CanRead)
                throw new MemberAccessorException(string.Format("Member \"{0}\" does not have a get method.", _fieldName));
            EnsureInit();
            return _emittedMemberAccessor.Get(target);
        }

        /// <summary>Sets the member for the specified target.</summary>
        /// <param name="target">Target object.</param>
        /// <param name="value">Value to set.</param>
        public void Set(object target, object value)
        {
            if (!CanWrite)
                throw new MemberAccessorException(string.Format("Member \"{0}\" does not have a set method.", _fieldName));
            EnsureInit();
            _emittedMemberAccessor.Set(target, value);
        }

        /// <summary>Whether or not the Member supports read access.</summary>
        internal abstract bool CanRead { get; }

        /// <summary>Whether or not the Member supports write access.</summary>
        internal abstract bool CanWrite { get; }

        /// <summary>
        /// The Type of object this member accessor was
        /// created for.
        /// </summary>
        internal Type TargetType => _targetType;

        /// <summary>The Type of the Member being accessed.</summary>
        internal abstract Type MemberType { get; }

        /// <summary>
        /// This method generates creates a new assembly containing
        /// the Type that will provide dynamic access.
        /// </summary>
        private void EnsureInit()
        {
            if (_emittedMemberAccessor != null) return;
            
            _emittedMemberAccessor = EmitAssembly().CreateInstance("Member") as IMemberAccessor;
            if (_emittedMemberAccessor == null)
                throw new Exception("Unable to create member accessor.");
        }

        /// <summary>
        /// Create an assembly that will provide the get and set methods.
        /// </summary>
        private Assembly EmitAssembly()
        {
            var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName()
            {
                Name = "PropertyAccessorAssembly"
            }, AssemblyBuilderAccess.Run);
            var type = assemblyBuilder.DefineDynamicModule("Module")
                .DefineType("Member", TypeAttributes.Public | TypeAttributes.Sealed);
            type.AddInterfaceImplementation(typeof(IMemberAccessor));
            type.DefineDefaultConstructor(MethodAttributes.Public);
            _EmitGetter(type);
            _EmitSetter(type);
            type.CreateType();
            return assemblyBuilder;
        }

        protected abstract void _EmitGetter(TypeBuilder type);

        protected abstract void _EmitSetter(TypeBuilder type);
    }
}