using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastDynamicMemberAccessor
{
    /// <summary>
    /// The PropertyAccessor class provides fast dynamic access
    /// to a property of a specified target class.
    /// </summary>
    internal class PropertyAccessor : MemberAccessor
    {
        private readonly Type _propertyType;
        private readonly bool _canRead;
        private readonly bool _canWrite;

        /// <summary>Creates a new property accessor.</summary>
        internal PropertyAccessor(PropertyInfo info)
            : base(info)
        {
            _canRead = info.CanRead;
            _canWrite = info.CanWrite;
            _propertyType = info.PropertyType;
        }

        /// <summary>The Type of the Property being accessed.</summary>
        internal override Type MemberType => _propertyType;

        /// <summary>Whether or not the Property supports read access.</summary>
        internal override bool CanRead => _canRead;

        /// <summary>Whether or not the Property supports write access.</summary>
        internal override bool CanWrite => _canWrite;

        protected override void _EmitSetter(TypeBuilder myType)
        {
            var parameterTypes = new Type[2]
            {
                typeof(object),
                typeof(object)
            };
            Type returnType = null;
            var ilGenerator = myType
                .DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes)
                .GetILGenerator();
            var method = _targetType.GetMethod("set_" + _fieldName);
            if (method != null)
            {
                var parameterType = method.GetParameters()[0].ParameterType;
                ilGenerator.DeclareLocal(parameterType);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, _targetType);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                if (parameterType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Unbox, parameterType);
                    if (s_TypeHash[parameterType] != null)
                    {
                        var opcode = (OpCode)s_TypeHash[parameterType];
                        ilGenerator.Emit(opcode);
                    }
                    else
                        ilGenerator.Emit(OpCodes.Ldobj, parameterType);
                }
                else
                    ilGenerator.Emit(OpCodes.Castclass, parameterType);

                ilGenerator.EmitCall(OpCodes.Callvirt, method, null);
            }
            else
                ilGenerator.ThrowException(typeof(MissingMethodException));

            ilGenerator.Emit(OpCodes.Ret);
        }

        protected override void _EmitGetter(TypeBuilder myType)
        {
            var parameterTypes = new Type[1]
            {
                typeof(object)
            };
            var returnType = typeof(object);
            var ilGenerator = myType
                .DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes)
                .GetILGenerator();
            var method = _targetType.GetMethod("get_" + _fieldName);
            if (method != null)
            {
                ilGenerator.DeclareLocal(typeof(object));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, _targetType);
                ilGenerator.EmitCall(OpCodes.Call, method, null);
                if (method.ReturnType.IsValueType)
                    ilGenerator.Emit(OpCodes.Box, method.ReturnType);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
            }
            else
                ilGenerator.ThrowException(typeof(MissingMethodException));

            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}